using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class WeightedSR : SignalRouting {
        
        public override void Init() {
            
        } 

        public override void RouteSignal(Signal signal) {
            ISignalAccepting destination;
            
            destination = Connection.LS.SelectWeightedRandomOutput(signal.SC.GetContent());

            Connection.N.S.Get<EffectController>().EnqueueEffect_TS(Connection.N.S.EffectFactory.NewSentSignalEffect(Connection, (INetworkComponent) destination));

            destination.AcceptSignal(signal, Connection);
        }
        
    }

}