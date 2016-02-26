using UnityEngine;
using System.Collections;

namespace Spock {

    public abstract class SignalRouting : ConnectionComponent {

        public abstract void RouteSignal(Signal signal);
        
    }

}