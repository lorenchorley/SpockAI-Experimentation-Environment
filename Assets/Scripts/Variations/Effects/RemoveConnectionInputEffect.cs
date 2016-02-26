using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class RemoveConnectionInputEffect : IRemoveConnectionInputEffect {

        private Connection connection;
        private Node node;

        public RemoveConnectionInputEffect(Connection connection, Node node) {
            this.connection = connection;
            this.node = node;
        }

        public override void Apply() {
            connection.GetRepresentation<ConnectionRepresentation>().RemoveNode(node, true);
        }

    }

}
