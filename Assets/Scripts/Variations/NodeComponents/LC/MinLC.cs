using UnityEngine;
using System.Collections;

namespace Spock {

    public class MinLC : LifeCycle {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new MinLC();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

    }

}