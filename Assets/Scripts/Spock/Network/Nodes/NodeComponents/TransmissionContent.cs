using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public abstract class TransmissionContent : NodeComponent {

        public abstract Signal GetContent();
        public abstract void ReplaceContent(Signal signal);
        public abstract bool HasContent();
        public abstract bool CanUseStraightAway(Signal signal);

    }

}