// Import and configure Firebase
import 'dotenv/config';
import * as admin from 'firebase-admin';

// Initialize Firebase Admin SDK
admin.initializeApp({
  projectId: process.env.FIREBASE_PROJECT_ID,
  storageBucket: process.env.FIREBASE_STORAGE_BUCKET,
});

const db = admin.firestore();

// Export services for use in other modules
export { db, admin };
