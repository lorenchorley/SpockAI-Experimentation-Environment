using UnityEngine;
using System.Collections.Generic;
using Spock;

namespace Spock {

    public interface INetworkComponent : ISpockComponent, ISignalAccepting, ISignalSending {

        long GetID();

    }

}