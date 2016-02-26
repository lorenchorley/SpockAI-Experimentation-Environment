using System;
using UnityEngine;

namespace Spock {

    public class ThreeIndex<S> : HyperCubeIndex where S : HyperCubeSlot {

        private readonly int[] pos;

        public ThreeIndex(int x, int y, int z, ThreeCube<S> cube) {
            pos = new int[3];
            pos[0] = x;
            pos[1] = y;
            pos[2] = z;

            if (!cube.IsValidIndex(this))
                throw new Exception("Not valid index for cube " + ToString());

        }

        public override bool Is(HyperCubeIndex position) {
            int[] threeData = position.Get();

            if (threeData.Length != pos.Length)
                return false;

            return threeData[0] == pos[0] && threeData[1] == pos[1] && threeData[2] == pos[2];
        }

        public override int[] Get() {
            return pos;
        }

        public override int Get(int index) {
            if (index <= 0 || index >= pos.Length)
                throw new Exception("Index out of bounds: " + index);

            return pos[index];
        }

        public override bool Equals(object obj) {

            if (!(obj is ThreeIndex<S>))
                return false;

            int[] objPos = (obj as ThreeIndex<S>).pos;

            return pos[0] == objPos[0] && pos[1] == objPos[1] && pos[2] == objPos[2];
        }
        
        public override int GetHashCode() {
            return pos[0] * 199 + pos[1] * 463 + pos[2] * 257;
        }

        public override string ToString() {
            return "(" + pos[0] + ", " + pos[1] + ", " + pos[2] + ")";
        }

    }

}