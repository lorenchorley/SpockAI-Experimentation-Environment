using System;
using System.Threading;
using UnityEngine;

namespace Spock {

    public abstract class HyperCubeWalker<S, I> where S : HyperCubeSlot where I : HyperCubeIndex {

        public HyperCubeWalkProfile<S, I> LastProfile;

        public abstract I Step(HyperCube<S, I> cube, I currentIndex, S currentSlot);

    }

}