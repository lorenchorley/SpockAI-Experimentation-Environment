using UnityEngine;

namespace Spock {

    public abstract class HyperCubeIndex {

        public abstract bool Is(HyperCubeIndex position);
        public abstract int[] Get();
        public abstract int Get(int index);

        public new abstract bool Equals(object obj);
        public new abstract int GetHashCode();

    }

}