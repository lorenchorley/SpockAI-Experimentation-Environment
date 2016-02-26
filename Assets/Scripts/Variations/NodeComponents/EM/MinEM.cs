using UnityEngine;
using System.Collections;

namespace Spock {

    public class MinEM : EnergyManagement {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new MinEM();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

    }

}