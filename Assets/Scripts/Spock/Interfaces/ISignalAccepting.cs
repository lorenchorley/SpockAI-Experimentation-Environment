using UnityEngine;
using System.Collections.Generic;
using Spock;

namespace Spock {

    public interface ISignalAccepting {

        void AcceptSignal(Signal signal, ISignalSending sendingObj);
        void AddInputs(params ISignalSending[] newInputs);

    }

}