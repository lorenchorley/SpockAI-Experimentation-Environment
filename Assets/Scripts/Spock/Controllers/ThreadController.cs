using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spock;

namespace Spock {

	public class ThreadController : MonoBehaviour, IController {

        private class JobProfile {
            public WaitCallback job;
            public object parameter;
        }

		public static ThreadController I;

        private static Queue<JobProfile> Jobs;
        private static readonly object _jobLock = new object();

        public void SetSpockInstance(SpockInstance S) { }

        public int ActiveThreads;
        public int WaitingThreads; // TODO
        public int TotalThreadsUsed;

        [NonSerialized]
        public bool stopThreads = false;
		private static Thread MainThread;
        private static int _ThreadsUsed;
        private static readonly ReaderWriterLock _rwLock = new ReaderWriterLock();
        private static List<Thread> threads = new List<Thread>();
        private static List<Thread> sleepingThreads = new List<Thread>();
        private static TimeSpan timeoutWaitingForThreads = new TimeSpan(0, 0, 3);

        public ThreadController() {
            Jobs = new Queue<JobProfile>();
        }
		
		public void SetEnabled(bool enable) {
			this.enabled = enable;
		}

		public void Preinit() {

			// Setup static instance reference
			ThreadController.I = this;

		}
		
		public void Init() {

		}

		void Awake() {
			MainThread = Thread.CurrentThread;
        }

        public static void AssertNotMainThread() {
            Assert.True("Not main thread", Thread.CurrentThread != MainThread);
        }

        public static void AddJobToQueue(WaitCallback job, object parameter) {

            JobProfile profile = new JobProfile() {
                job = job,
                parameter = parameter
            };

            lock (_jobLock) {
                Jobs.Enqueue(profile);
            }

        }

        private static void StartThreads() {
            JobProfile profile;

            if (Jobs.Count > 0) {
                lock (_jobLock) {
                    do {
                        profile = Jobs.Dequeue();
                        ThreadPool.QueueUserWorkItem(JobFunctions.ThreadHandler, new object[] { profile.job, profile.parameter });
                    } while (Jobs.Count > 0);
                }
            }

        }

        void Update() {

            // Update thread count in the editor (Always main thread)
            LockFunctions.AcquireReadingLock(_rwLock);
            ActiveThreads = threads.Count;
            TotalThreadsUsed = _ThreadsUsed;
            LockFunctions.ReleaseReadingLock(_rwLock);

            // If there is a need for new threads, start them
            StartThreads();

        }

        public static void RaiseEventThreadStarted() {
            LockFunctions.AcquireWritingLock(_rwLock);

            if (threads.Count == 0)
                    SelectiveDebug.StartTimer("Thread activity");
                 
            threads.Add(Thread.CurrentThread);

            _ThreadsUsed++;
            
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public static void RaiseEventThreadFinished() {
            LockFunctions.AcquireWritingLock(_rwLock);

            threads.Remove(Thread.CurrentThread);

            if (threads.Count == 0)
                SelectiveDebug.StopTimer("Thread activity");

            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        void OnApplicationQuit() {

            // Signal the threads to safely close
            stopThreads = true;

            // Interrupt all threads to get them to continue execution if they are sleeping
            LockFunctions.AcquireReadingLock(_rwLock);
            foreach (Thread t in sleepingThreads)
                t.Interrupt();

            // then wait for them to finish
            int threadCount;
            threadCount = threads.Count;
            LockFunctions.ReleaseReadingLock(_rwLock);

            DateTime? timeout = DateTime.Now.Add(timeoutWaitingForThreads);

            while (threadCount > 0) {

                // After a certain period of time, give up and try to kill all the threads
                // Note that saving is not an option after this as the network may no longer be intact
                if (timeout.HasValue && DateTime.Now.CompareTo(timeout.Value) >= 0) {
                    timeout = null;

                    Debug.LogWarning("Threads were aborted, the network state may not be complete");

                    LockFunctions.AcquireReadingLock(_rwLock);

                    // Try to kill all the threads as viciously as possible
                    foreach (Thread t in threads) {
                        t.Interrupt(); // To try to break a possible deadlock
                        t.Abort(); // Try to stop a thread in an infinite loop that does not perform a stopThreads check
                    }

                    LockFunctions.ReleaseReadingLock(_rwLock);
                }

                // Wait a bit for the threads
                Thread.Sleep(100);

                LockFunctions.AcquireReadingLock(_rwLock);

                // Get the thread count
                threadCount = threads.Count;

                LockFunctions.ReleaseReadingLock(_rwLock);

            }

            // TODO do saving here, check that threads weren't aborted

        }

        public static void SleepThread(int duration) {
            LockFunctions.AcquireWritingLock(_rwLock);
            sleepingThreads.Add(Thread.CurrentThread);
            LockFunctions.ReleaseWritingLock(_rwLock);

            try {
                Thread.Sleep(duration);
            } catch (ThreadInterruptedException) { /* Continue if interrupted */ }

            LockFunctions.AcquireWritingLock(_rwLock);
            sleepingThreads.Remove(Thread.CurrentThread);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

    }

}
