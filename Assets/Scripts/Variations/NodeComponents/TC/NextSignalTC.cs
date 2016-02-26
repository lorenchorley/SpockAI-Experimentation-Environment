using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class NextSignalTC : TransmissionContent {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new NextSignalTC();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

        public override Signal GetContent() {
            Signal signal = Node.SP.PopNextSignal();
            SelectiveDebug.LogSignalProgress("NextSignalTC.GetContent: " + signal);
            return signal;
        }

        public override void ReplaceContent(Signal signal) {
            Node.SP.PushNextSignal(signal);
        }

        public override bool HasContent() {
            return Node.SP.HasSignals();
        }

        public override bool CanUseStraightAway(Signal signal) {
            return true;
        }

    }

}