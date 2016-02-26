using System;
using System.Threading;
using UnityEngine;

namespace Spock {

    public class ThreeCube<S> : HyperCube<S, ThreeIndex<S>> where S : HyperCubeSlot {

        private S[,,] matrix;
        private int sidelength;

        public ThreeCube(int sidelength, Func<int, int, int, HyperCube<S, ThreeIndex<S>>, S> slotSetter) {
            matrix = new S[sidelength, sidelength, sidelength];
            this.sidelength = sidelength;

            for (int x = 0; x < sidelength; x++)
                for (int y = 0; y < sidelength; y++)
                    for (int z = 0; z < sidelength; z++)
                        matrix[x, y, z] = slotSetter(x, y, z, this);

        }
        
        public override S GetSlot(ThreeIndex<S> index) {
            int[] pos = index.Get();
            l.AcquireReaderLock(0);
            S s = matrix[pos[0], pos[1], pos[2]];
            l.ReleaseReaderLock();
            return s;
        }
        
        public override bool IsValidIndex(ThreeIndex<S> index) {
            int[] pos = index.Get();
            return pos[0] >= 0 && pos[1] >= 0 && pos[2] >= 0 && pos[0] < sidelength && pos[1] < sidelength && pos[2] < sidelength;
        }
        
    }

}