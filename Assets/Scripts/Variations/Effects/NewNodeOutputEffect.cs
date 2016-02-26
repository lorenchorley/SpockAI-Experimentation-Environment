using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewNodeOutputEffect : INewNodeOutputEffect {

        private Node node;
        private ISignalAccepting obj;

        public NewNodeOutputEffect(Node node, ISignalAccepting obj) {
            this.node = node;
            this.obj = obj;
        }

        public override void Apply() {
            if (obj is Connection) {
                node.GetRepresentation<NodeRepresentation>().AddConnection(obj as Connection, true);
            }
        }

    }

}
