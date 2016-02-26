using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class RandomSR : SignalRouting {

        //WeightedLS WLS;

        public override void Init() {
            Assert.Same("LS is WeightedLS", Connection.LS.GetType(), typeof(WeightedLS));
            //WLS = Connection.LS as WeightedLS;
        }

        public override void RouteSignal(Signal signal) {
            //WLS.
        }
        
    }

}