using System;
using System.Threading;
using UnityEngine;

namespace Spock {

    public class PointerBasedThreeCubeWalker : ThreeCubeWalker<PointerSlot> {

        public override ThreeIndex<PointerSlot> Step(HyperCube<PointerSlot, ThreeIndex<PointerSlot>> hc, ThreeIndex<PointerSlot> currentIndex, PointerSlot currentSlot) {
            //ThreeCube<PointerSlot> cube = hc as ThreeCube<PointerSlot>;
            
            return currentSlot.GetInternalTarget() as ThreeIndex<PointerSlot>;

        }

    }

}