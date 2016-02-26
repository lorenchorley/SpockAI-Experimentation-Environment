using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewConnectionOutputEffect : INewConnectionOutputEffect {

        private Connection connection;
        private ISignalAccepting obj;

        public NewConnectionOutputEffect(Connection connection, ISignalAccepting obj) {
            this.connection = connection;
            this.obj = obj;
        }

        public override void Apply() {
            if (obj is Node) {
                connection.GetRepresentation<ConnectionRepresentation>().AddNode(obj as Node, false);
            } else if (obj is ComponentInterface) {
                connection.GetRepresentation<ConnectionRepresentation>().AddCI(obj as ComponentInterface, false);
            }
        }

    }

}
