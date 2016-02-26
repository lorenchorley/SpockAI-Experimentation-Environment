using UnityEngine;
using System.Collections;
using System;
    
namespace Spock {

    [Serializable]
    public class EnvironmentExitCI : ExitComponentInterface {

        public EnvironmentExitCI(SpockInstance S, Environment E) : base (S) {
            this.E = E;
        }

        public override void AcceptSignal(Signal signal, ISignalSending sendingObj) {
            SelectiveDebug.LogSignalProgress("EnvironmentExitCI.AcceptSignal: " + signal);

            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewReceivedSignalEffect((INetworkComponent) sendingObj, this));

            // Register this ci with the signal
            signal.CR.VisitComponentInterface(this);

            ThreadController.AddJobToQueue(JobFunctions.ProcessAcceptSignal, new object[] { this, signal, eci });
            
        }
        
    }

}