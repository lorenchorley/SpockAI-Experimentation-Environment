using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class RemoveConnectionOutputEffect : IRemoveConnectionOutputEffect {

        private Connection connection;
        private Node node;

        public RemoveConnectionOutputEffect(Connection connection, Node node) {
            this.connection = connection;
            this.node = node;
        }

        public override void Apply() {
            connection.GetRepresentation<ConnectionRepresentation>().RemoveNode(node, false);
        }

    }

}
