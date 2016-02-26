using UnityEngine;
using System.Collections;
using System;
    
namespace Spock {

    [Serializable]
    public class EnvironmentEntryCI : EntryComponentInterface {

        private Environment environment;
        private int channel;

        public EnvironmentEntryCI(SpockInstance S, Environment E) : base (S) {
            this.E = E;
        }

        public override void AcceptSignal(Signal signal, ISignalSending sendingObj) {
            SelectiveDebug.LogSignalProgress("EnvironmentEntryCI.AcceptSignal: " + signal);

            JobFunctions.WaitThreadAMoment();

            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewReceivedSignalEffect((INetworkComponent) sendingObj, this));

            // Register this ci with the signal
            signal.CR.VisitComponentInterface(this);

            environment.AcceptSignal(signal, channel);
        }
        
        public EnvironmentEntryCI SetEnvironmentChannel(Environment environment, int channel) {
            this.environment = environment;
            this.channel = channel;
            return this;
        }

    }

}