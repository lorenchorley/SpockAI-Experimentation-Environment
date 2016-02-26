using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class MinCR : ComponentRegistration {
        
        public override void Init() {

        }

        protected override SignalComponent NewComponentInstance() {
            return new MinCR();
        }

        protected override void DuplicateComponentProperties(SignalComponent OriginalComponent) {
        }

        public override void VisitComponentInterface(ComponentInterface ci) {
            if (ci is EntryComponentInterface)
                Signal.N = ci.N;
        }

        public override void VisitConnection(Connection connection) {
        }

        public override void VisitNode(Node node) {
        }
        
        public override void RewardForSignalPath(float reward) {
        }

    }

}