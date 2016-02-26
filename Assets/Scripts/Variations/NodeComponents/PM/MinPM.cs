using UnityEngine;
using System.Collections;

namespace Spock {

    public class MinPM : PropertyManagement {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new MinPM();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

    }

}