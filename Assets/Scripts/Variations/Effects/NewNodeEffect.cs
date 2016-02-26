using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewNodeEffect : INewNodeEffect {

        private Node node;

        public NewNodeEffect(Node node) {
            this.node = node;
        }

        public override void Apply() {
            RepresentationFunctions.CreateRepresentation(node);
        }

    }

}
