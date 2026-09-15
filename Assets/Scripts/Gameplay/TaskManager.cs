using UnityEngine;
using System.Collections.Generic;
using CandyCrush.Core;
using CandyCrush.Events;
using CandyCrush.Network.Data;

namespace CandyCrush.Gameplay
{
    /// <summary>
    /// Manages individual player tasks (daily, weekly, achievements)
    /// </summary>
    public class TaskManager : Singleton<TaskManager>
    {
        private Dictionary<string, TaskProgress> activeTasks = new Dictionary<string, TaskProgress>();
        private Dictionary<string, TaskProgress> completedTasks = new Dictionary<string, TaskProgress>();

        /// <summary>
        /// Initialize tasks from data
        /// </summary>
        public void InitializeTasks(TaskProgress[] tasks)
        {
            activeTasks.Clear();
            foreach (var task in tasks)
            {
                if (!task.Completed)
                    activeTasks[task.TaskId] = task;
                else
                    completedTasks[task.TaskId] = task;
            }
        }

        /// <summary>
        /// Update task progress
        /// </summary>
        public void UpdateTaskProgress(string taskId, int amount)
        {
            if (activeTasks.TryGetValue(taskId, out var task))
            {
                task.CurrentProgress += amount;

                if (task.CurrentProgress >= task.Goal && !task.Completed)
                {
                    task.Completed = true;
                    task.CompletedAt = System.DateTime.Now;
                    GameEvents.InvokeTaskCompleted(taskId);

                    // Move to completed
                    activeTasks.Remove(taskId);
                    completedTasks[taskId] = task;
                }
                else
                {
                    GameEvents.InvokeTaskProgress(taskId);
                }
            }
        }

        /// <summary>
        /// Claim task reward
        /// </summary>
        public void ClaimReward(string taskId)
        {
            if (completedTasks.TryGetValue(taskId, out var task) && !task.Claimed)
            {
                task.Claimed = true;
                GameEvents.InvokeTaskRewarded(taskId, task.RewardAmount);

                // Award based on reward type
                switch (task.RewardType.ToLower())
                {
                    case "coins":
                        GameEvents.InvokeCoinsChanged(task.RewardAmount);
                        break;
                    case "gems":
                        GameEvents.InvokeGemsChanged(task.RewardAmount);
                        break;
                    case "life":
                        GameEvents.InvokeLivesChanged(1);
                        break;
                }
            }
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        public TaskProgress GetTask(string taskId)
        {
            if (activeTasks.TryGetValue(taskId, out var task))
                return task;
            if (completedTasks.TryGetValue(taskId, out task))
                return task;
            return null;
        }

        /// <summary>
        /// Get all active tasks
        /// </summary>
        public IEnumerable<TaskProgress> GetActiveTasks() => activeTasks.Values;

        /// <summary>
        /// Get all completed but unclaimed tasks
        /// </summary>
        public IEnumerable<TaskProgress> GetCompletedTasks() => completedTasks.Values;
    }
}
