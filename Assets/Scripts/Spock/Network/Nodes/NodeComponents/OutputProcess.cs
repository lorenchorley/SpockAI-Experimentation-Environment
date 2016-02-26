using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Spock {

    public abstract class OutputProcess : NodeComponent {

        public static readonly bool CONTINUE_THIS_THREAD = false;
        public static readonly bool START_NEW_THREAD = true;

        public abstract bool IterationRequired();
        public abstract void OutputIteration(bool NewThreadOnSend, Signal signalToSend = null);

    }

}