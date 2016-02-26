using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class MinMM : MetricManagement {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new MinMM();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

        public override void SetReinforcement(float level, int type, DateTime time) {
        }
    }

}