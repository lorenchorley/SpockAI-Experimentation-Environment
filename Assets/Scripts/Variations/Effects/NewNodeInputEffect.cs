using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewNodeInputEffect : INewNodeInputEffect {

        private Node node;
        private ISignalSending obj;

        public NewNodeInputEffect(Node node, ISignalSending obj) {
            this.node = node;
            this.obj = obj;
        }

        public override void Apply() {
            if (obj is Connection) {
                node.GetRepresentation<NodeRepresentation>().AddConnection(obj as Connection, false);
            }
        }

    }

}
