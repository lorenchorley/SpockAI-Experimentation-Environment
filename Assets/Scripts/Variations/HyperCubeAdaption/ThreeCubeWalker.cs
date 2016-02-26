using System;
using System.Threading;
using UnityEngine;

namespace Spock {

    public abstract class ThreeCubeWalker<S> : HyperCubeWalker<S, ThreeIndex<S>> where S : HyperCubeSlot {
        
    }

}