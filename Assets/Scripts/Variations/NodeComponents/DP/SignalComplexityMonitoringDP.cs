using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class SignalComplexityMonitoringDP : DataProcessing {

        // TODO Weight connections according to whether the signal content is appropriate or not, using the first degree backlink in the signal
        // TODO make data processing more modular by just running a series of delegates that are registered in the node template

        public override void Init() {
        }

        protected override NodeComponent NewComponentInstance() {
            return new SignalComplexityMonitoringDP();
        }
        
        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
            // Don't duplicate traffic statistics
        }

        public override Signal ProcessData(Signal signal) {
            SelectiveDebug.LogSignalProgress("SignalComplexityMonitoringDP.ProcessData: " + signal);

            return signal;
        }

    }

}