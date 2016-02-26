using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public abstract class TargetSelection : NodeComponent {

        public abstract ISignalAccepting SelectTarget(Signal signal);
        public abstract bool CanTarget();
        public abstract void NewOutputRegistered(ISignalAccepting obj);

    }

}