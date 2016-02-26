using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Spock {

    public class RandomTS : TargetSelection {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new RandomTS();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

        public override bool CanTarget() {
            return Node.SP.HasOutputs();
        }

        public override ISignalAccepting SelectTarget(Signal signal) {
            ISignalAccepting target = Node.SP.GetRandomOutput();
            SelectiveDebug.LogSignalProgress("RandomTS.SelectTarget: " + target);
            return target;
        }

        public override void NewOutputRegistered(ISignalAccepting obj) {

        }

    }

}