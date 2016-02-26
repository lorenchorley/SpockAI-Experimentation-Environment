using System;
using System.Threading;
using UnityEngine;

namespace Spock {

    public class ThreeCubeTestCase : UUnitTestCase {
        
        private ThreeCube<PointerSlot> cube;

        [UUnitTest]
        public void CreateCube() {
            cube = new ThreeCube<PointerSlot>(3, delegate (int x, int y, int z, HyperCube<PointerSlot, ThreeIndex<PointerSlot>> hc) {
                if (x == 2) {
                    int slotNumber = x;
                    return new PointerSlot(delegate {
                        Debug.Log("local " + slotNumber);
                        Debug.Log("not local " + x);
                    });
                } else {
                    return new PointerSlot(new ThreeIndex<PointerSlot>(x+1, y, z, hc as ThreeCube<PointerSlot>));
                }
            });

            PointerBasedThreeCubeWalker walker = new PointerBasedThreeCubeWalker();
            cube.Walk(walker, new ThreeIndex<PointerSlot>(0, 0, 0, cube));

            UUnitAssert.NotNull(walker.LastProfile);
            UUnitAssert.True(walker.LastProfile.GetLastWalkExited());
            UUnitAssert.True(walker.LastProfile.GetLastWalksExitIndex().Equals(new ThreeIndex<PointerSlot>(2,0,0,cube)));

        }
        
    }

}