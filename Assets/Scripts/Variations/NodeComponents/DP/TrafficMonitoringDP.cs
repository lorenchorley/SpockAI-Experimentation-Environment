using UnityEngine;
using System.Collections.Generic;
using System;

namespace Spock {

    // Records the number of signals that pass through this component per interval
    // At the end of the interval it checks the traffic count against a threshold, then discard it
    // If the count is over the threshold, it triggers a duplication in parallel event on this node
    public class TrafficMonitoringDP : DataProcessing {

        private readonly int timeIntervalms = 500;
        private readonly int trafficThresholdPerInterval = 10;
        private readonly long ticksPerInterval; 

        private long trafficThisInterval;
        private long startingTicks;

        public TrafficMonitoringDP() {
            ticksPerInterval = timeIntervalms * 10000;
        }

        public override void Init() {
            startingTicks = DateTime.Now.Ticks;
        }

        protected override NodeComponent NewComponentInstance() {
            return new TrafficMonitoringDP();
        }
        
        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
            // Don't duplicate traffic statistics
        }

        public override Signal ProcessData(Signal signal) {
            SelectiveDebug.LogSignalProgress("TrafficMonitoringDP.ProcessData: " + signal);
            RegisterASignal();
            return signal;
        }

        private void RegisterASignal() {

            // Either increment the traffic counter or runs checks and reset it
            LockFunctions.AcquireReadingLock(_rwLock);

            if (HasIntervalExpired()) {

                CheckTrafficVolume();

                LockFunctions.UpgradeToWritingLock(_rwLock);

                // Reset
                startingTicks = DateTime.Now.Ticks;
                trafficThisInterval = 1;

                LockFunctions.ReleaseAllLocks(_rwLock);

            } else {

                // Increment
                trafficThisInterval++;

                LockFunctions.ReleaseReadingLock(_rwLock);

            }

        }

        private void CheckTrafficVolume() {

            if (trafficThisInterval >= trafficThresholdPerInterval)
                NetworkFunctions.DuplicateNodeInParallel(Node);

        }

        private bool HasIntervalExpired() {
            return DateTime.Now.Ticks > startingTicks + ticksPerInterval;
        }

    }

}