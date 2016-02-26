using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class AlwaysActivatedAC : ActivationController {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new AlwaysActivatedAC();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

        public override bool RequestActivation() {
            SelectiveDebug.LogSignalProgress("AlwaysActivatedAC.RequestActivation: true");
            return true;
        }

    }

}