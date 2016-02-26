using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewNetworkEffect : INewNetworkEffect {

        private Network network;

        public NewNetworkEffect(Network network) {
            this.network = network;
        }

        public override void Apply() {
            RepresentationFunctions.CreateRepresentation(network);
        }

    }

}
