//#define USE_PAUSES

using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Spock {

    public class LockFunctions {

        public static int timeout = -1;

        public static void AcquireReadingLock(ReaderWriterLock rwlock) {
            //try {
                rwlock.AcquireReaderLock(timeout);
            //} catch (ApplicationException e) {
            //    UnityEngine.Debug.LogError("Lock request expired");
            //    throw e;
            //}
        }

        public static void AcquireWritingLock(ReaderWriterLock rwlock) {
            //try {
                rwlock.AcquireWriterLock(timeout);
            //} catch (ApplicationException e) {
            //    UnityEngine.Debug.LogError("Lock request expired");
            //    throw e;
            //}
        }

        public static void UpgradeToWritingLock(ReaderWriterLock rwlock) {
            //try {
                rwlock.UpgradeToWriterLock(timeout);
            //} catch (ApplicationException e) {
            //    UnityEngine.Debug.LogError("Lock request expired");
            //    throw e;
            //}
        }

        public static void ReleaseReadingLock(ReaderWriterLock rwlock) {
            rwlock.ReleaseReaderLock();
        }

        public static void ReleaseWritingLock(ReaderWriterLock rwlock) {
            rwlock.ReleaseWriterLock();
        }

        public static void ReleaseAllLocks(ReaderWriterLock rwlock) {
            rwlock.ReleaseLock();
        }

    }

}
