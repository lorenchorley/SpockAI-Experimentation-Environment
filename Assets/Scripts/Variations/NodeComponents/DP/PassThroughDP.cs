using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class PassThroughDP : DataProcessing {

        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new PassThroughDP();
        }
        
        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

        public override Signal ProcessData(Signal signal) {
            SelectiveDebug.LogSignalProgress("PassThroughDP.ProcessData: " + signal);
            return signal;
        }

    }

}