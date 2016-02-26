using UnityEngine;
using System.Collections;
using System;

namespace Spock {
    
    public class NetworkExitCI : ExitComponentInterface {

        //private Connection connection;

        public NetworkExitCI(SpockInstance S, Network N) : base (S) {
            this.N = N;
        }

        public override void AcceptSignal(Signal signal, ISignalSending sendingObj) {
            SelectiveDebug.LogSignalProgress("NetworkExitCI.AcceptSignal: " + signal);

            JobFunctions.WaitThreadAMoment();

            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewReceivedSignalEffect((INetworkComponent) sendingObj, this));

            // Register this ci with the signal
            signal.CR.VisitComponentInterface(this);

            eci.AcceptSignal(signal, this);
        }
        
        public NetworkExitCI SetNetworkConnection(Connection connection) {
            //this.connection = connection;

            connection.AddInputs(this);

            return this;
        }

    }

}