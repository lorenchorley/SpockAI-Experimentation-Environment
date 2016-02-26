using UnityEngine;
using System.Collections.Generic;
using Spock;

namespace Spock {

    public interface ISignalSending {

        void AddOutputs(params ISignalAccepting[] newOutputs);
        
    }

}