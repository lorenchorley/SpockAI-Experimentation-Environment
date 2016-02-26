using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewSignalEffect : INewSignalEffect {

        private Signal signal;

        public NewSignalEffect(Signal signal) {
            this.signal = signal;
        }

        public override void Apply() {
            RepresentationFunctions.CreateRepresentation(signal);
        }

    }

}
