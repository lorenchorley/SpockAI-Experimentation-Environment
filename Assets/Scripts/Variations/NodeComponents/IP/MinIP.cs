using UnityEngine;
using System.Collections;

namespace Spock {

    public class MinIP : InputProcess {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new MinIP();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

        public override bool AcceptSignal(Signal signal) {
            SelectiveDebug.LogSignalProgress("MinIP.AcceptSignal: " + signal);
            signal = Node.Get<DataProcessing>().ProcessData(signal);
            if (Node.Get<TransmissionContent>().CanUseStraightAway(signal)) { 
                return false;
            } else {
                Node.SP.StoreSignal(signal);
                return true;
            }
        }

    }

}