using UnityEngine;
using System.Collections;
using System;

namespace Spock {
    
    public class NetworkEntryCI : EntryComponentInterface {

        private Connection connection;

        public NetworkEntryCI(SpockInstance S, Network N) : base (S) {
            this.N = N;
        }

        public override void AcceptSignal(Signal signal, ISignalSending sendingObj) {
            SelectiveDebug.LogSignalProgress("NetworkEntryCI.AcceptSignal: " + signal);

            JobFunctions.WaitThreadAMoment();

            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewReceivedSignalEffect((INetworkComponent) sendingObj, this));

            // Register this ci with the signal
            signal.CR.VisitComponentInterface(this);

            connection.AcceptSignal(signal, this);
        }
        
        public NetworkEntryCI SetNetworkConnection(Connection connection) {
            this.connection = connection;

            connection.AddInputs(this);

            return this;
        }

    }

}