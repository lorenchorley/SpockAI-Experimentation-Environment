using UnityEngine;
using System.Collections;

namespace Spock {

    public abstract class InputProcess : NodeComponent {

        public abstract bool AcceptSignal(Signal signal); // Returns whether the signal was stored or not

    }

}