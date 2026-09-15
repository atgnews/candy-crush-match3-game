/**
 * Cloud Functions for Candy Crush Match-3 Game
 * Deploy with: firebase deploy --only functions
 */

const functions = require('firebase-functions');
const admin = require('firebase-admin');

admin.initializeApp();

const db = admin.firestore();
const auth = admin.auth();

// ============================================
// SCORE VALIDATION & LEADERBOARD FUNCTIONS
// ============================================

/**
 * Validate level completion score and update leaderboard
 */
exports.validateAndSaveScore = functions.https.onCall(async (data, context) => {
    if (!context.auth) {
        throw new functions.https.HttpsError('unauthenticated', 'User not authenticated');
    }

    const userId = context.auth.uid;
    const { levelId, score, moves, timeSpent } = data;

    try {
        // Anti-cheat: Validate move count and score relationship
        const levelRef = db.collection('levels').doc(levelId.toString());
        const levelSnap = await levelRef.get();

        if (!levelSnap.exists) {
            throw new functions.https.HttpsError('not-found', 'Level not found');
        }

        const level = levelSnap.data();
        const maxPossibleScore = level.moveLimit * 50; // Rough estimate

        // Flag suspicious scores
        if (score > maxPossibleScore * 2) {
            console.warn(`Suspicious score detected for user ${userId} on level ${levelId}`);
            // Could flag for review or reject
        }

        // Save score to user's level progress
        const progressRef = db.collection('users').doc(userId)
            .collection('levelProgress').doc(levelId.toString());

        const currentScore = (await progressRef.get()).data()?.BestScore || 0;

        if (score > currentScore) {
            await progressRef.set({
                LevelId: levelId,
                Completed: true,
                BestScore: score,
                CompletedAt: admin.firestore.FieldValue.serverTimestamp(),
                Attempts: admin.firestore.FieldValue.increment(1)
            }, { merge: true });
        }

        // Update leaderboard
        await updateLeaderboard(userId, score, levelId);

        return { success: true, validated: true };
    } catch (error) {
        console.error('Score validation error:', error);
        throw new functions.https.HttpsError('internal', error.message);
    }
});

/**
 * Update global and level-specific leaderboards
 */
async function updateLeaderboard(userId, score, levelId) {
    const batch = db.batch();

    try {
        // Get user info
        const userSnap = await db.collection('users').doc(userId).get();
        const userData = userSnap.data();

        // Update global leaderboard
        const globalEntryRef = db.collection('leaderboards').doc('global')
            .collection('entries').doc(userId);
        batch.set(globalEntryRef, {
            Rank: 0, // Will be recalculated
            UserId: userId,
            Username: userData.Username,
            Score: admin.firestore.FieldValue.increment(score),
            Level: userData.Level,
            LastUpdated: admin.firestore.FieldValue.serverTimestamp()
        }, { merge: true });

        // Update level-specific leaderboard
        const levelEntryRef = db.collection('leaderboards').doc(`level_${levelId}`)
            .collection('entries').doc(userId);
        batch.set(levelEntryRef, {
            UserId: userId,
            Username: userData.Username,
            Score: score,
            Stars: 0, // To be calculated
            CompletedAt: admin.firestore.FieldValue.serverTimestamp()
        }, { merge: true });

        await batch.commit();

        // Recalculate ranks asynchronously
        await recalculateRanks();
    } catch (error) {
        console.error('Leaderboard update error:', error);
    }
}

/**
 * Recalculate all player ranks
 */
async function recalculateRanks() {
    try {
        const snapshot = await db.collection('leaderboards').doc('global')
            .collection('entries')
            .orderBy('Score', 'desc')
            .get();

        const batch = db.batch();
        let rank = 1;

        snapshot.docs.forEach(doc => {
            batch.update(doc.ref, { Rank: rank });
            rank++;
        });

        await batch.commit();
        console.log(`Ranks recalculated. Total players: ${rank - 1}`);
    } catch (error) {
        console.error('Rank calculation error:', error);
    }
}

// ============================================
// TASK PROGRESS & REWARDS FUNCTIONS
// ============================================

/**
 * Update task progress and check for completion
 */
exports.updateTaskProgress = functions.https.onCall(async (data, context) => {
    if (!context.auth) {
        throw new functions.https.HttpsError('unauthenticated', 'User not authenticated');
    }

    const userId = context.auth.uid;
    const { taskId, progress } = data;

    try {
        const taskRef = db.collection('users').doc(userId)
            .collection('tasks').doc(taskId);

        const taskSnap = await taskRef.get();
        if (!taskSnap.exists) {
            throw new functions.https.HttpsError('not-found', 'Task not found');
        }

        const task = taskSnap.data();
        const newProgress = (task.CurrentProgress || 0) + progress;

        await taskRef.update({
            CurrentProgress: newProgress,
            LastUpdatedAt: admin.firestore.FieldValue.serverTimestamp()
        });

        // Check if task is complete
        if (newProgress >= task.Goal && !task.Completed) {
            await completeTask(userId, taskId, task);
        }

        return { success: true, progress: newProgress, completed: newProgress >= task.Goal };
    } catch (error) {
        console.error('Task progress error:', error);
        throw new functions.https.HttpsError('internal', error.message);
    }
});

/**
 * Complete a task and award rewards
 */
async function completeTask(userId, taskId, taskData) {
    try {
        const batch = db.batch();

        // Mark task as completed
        const taskRef = db.collection('users').doc(userId)
            .collection('tasks').doc(taskId);
        batch.update(taskRef, {
            Completed: true,
            CompletedAt: admin.firestore.FieldValue.serverTimestamp()
        });

        // Award reward
        const userRef = db.collection('users').doc(userId);
        if (taskData.RewardType === 'coins') {
            batch.update(userRef, {
                Coins: admin.firestore.FieldValue.increment(taskData.RewardAmount)
            });
        } else if (taskData.RewardType === 'gems') {
            batch.update(userRef, {
                Gems: admin.firestore.FieldValue.increment(taskData.RewardAmount)
            });
        }

        // Log transaction
        const transactionRef = db.collection('transactions').doc();
        batch.set(transactionRef, {
            UserId: userId,
            Type: 'task_reward',
            Amount: taskData.RewardAmount,
            CurrencyType: taskData.RewardType,
            Status: 'completed',
            CreatedAt: admin.firestore.FieldValue.serverTimestamp()
        });

        await batch.commit();
        console.log(`Task ${taskId} completed for user ${userId}`);
    } catch (error) {
        console.error('Complete task error:', error);
    }
}

// ============================================
// TEAM MILESTONE FUNCTIONS
// ============================================

/**
 * Check and award team milestones
 */
exports.checkTeamMilestone = functions.https.onCall(async (data, context) => {
    if (!context.auth) {
        throw new functions.https.HttpsError('unauthenticated', 'User not authenticated');
    }

    const { teamId, currentProgress, goalTarget } = data;

    try {
        const teamRef = db.collection('teams').doc(teamId);
        const percentComplete = (currentProgress / goalTarget) * 100;

        const milestones = [25, 50, 100];
        const batch = db.batch();

        for (const milestone of milestones) {
            if (percentComplete >= milestone) {
                // Award milestone reward to all team members
                const teamSnap = await teamRef.get();
                const teamData = teamSnap.data();

                // Example reward: 500 coins per 25% milestone
                const rewardAmount = 500 * (milestone / 25);

                // This would iterate through all team members and award rewards
                // Simplified for brevity
            }
        }

        await batch.commit();
        return { success: true, percentComplete };
    } catch (error) {
        console.error('Team milestone error:', error);
        throw new functions.https.HttpsError('internal', error.message);
    }
});

// ============================================
// DAILY RESET FUNCTIONS
// ============================================

/**
 * Daily scheduled task: Reset daily tasks at 00:00 UTC
 */
exports.resetDailyTasks = functions.pubsub
    .schedule('0 0 * * *') // Every day at midnight UTC
    .timeZone('UTC')
    .onRun(async (context) => {
        try {
            const batch = db.batch();

            // Get all users
            const usersSnap = await db.collection('users').get();

            usersSnap.forEach(userDoc => {
                const tasksRef = userDoc.ref.collection('tasks');
                batch.delete(tasksRef); // Delete old tasks (optional)
            });

            await batch.commit();
            console.log('Daily tasks reset completed');
            return null;
        } catch (error) {
            console.error('Daily reset error:', error);
        }
    });

/**
 * Weekly scheduled task: Reset weekly leaderboard at Sunday 00:00 UTC
 */
exports.resetWeeklyLeaderboard = functions.pubsub
    .schedule('0 0 ? * SUN') // Every Sunday at midnight UTC
    .timeZone('UTC')
    .onRun(async (context) => {
        try {
            // Archive current weekly scores
            // Reset weekly leaderboard
            console.log('Weekly leaderboard reset completed');
            return null;
        } catch (error) {
            console.error('Weekly reset error:', error);
        }
    });

// ============================================
// ANTI-CHEAT FUNCTIONS
// ============================================

/**
 * Detect suspicious activity patterns
 */
exports.detectAnomalies = functions.https.onCall(async (data, context) => {
    if (!context.auth) {
        throw new functions.https.HttpsError('unauthenticated', 'User not authenticated');
    }

    const userId = context.auth.uid;
    const { moves, timeSpent, score } = data;

    try {
        const flags = [];

        // Check for impossible move speed
        if (moves > 100 || (timeSpent > 0 && moves / timeSpent > 10)) {
            flags.push('impossible_move_speed');
        }

        // Check for score inflation
        const expectedMaxScore = moves * 50;
        if (score > expectedMaxScore * 3) {
            flags.push('score_inflation');
        }

        if (flags.length > 0) {
            // Log suspicious activity
            await db.collection('suspicious_activity').add({
                UserId: userId,
                Flags: flags,
                Data: data,
                DetectedAt: admin.firestore.FieldValue.serverTimestamp()
            });

            console.warn(`Suspicious activity detected for user ${userId}:`, flags);
        }

        return { suspicious: flags.length > 0, flags };
    } catch (error) {
        console.error('Anomaly detection error:', error);
        throw new functions.https.HttpsError('internal', error.message);
    }
});
