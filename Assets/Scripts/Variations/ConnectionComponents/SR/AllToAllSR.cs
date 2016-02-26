using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class AllToAllSR : SignalRouting {

        MinLS minLS;

        public override void Init() {
            Assert.True("LS is MinLS", Connection.LS is MinLS);
            minLS = Connection.LS as MinLS;
        }

        public override void RouteSignal(Signal signal) {

            minLS.ForEachOutputLastSeparately(
                
                // If there is more than one output, duplicate the signal and send it managed by a new thread
                output => new Signal(signal, Connection, output),
                
                // If there is at least one output, use this thread to process the accept
                output => output.AcceptSignal(signal, Connection)

            );

        }
        
    }

}