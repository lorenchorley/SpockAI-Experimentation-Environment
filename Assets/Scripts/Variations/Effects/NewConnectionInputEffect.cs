using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewConnectionInputEffect : INewConnectionInputEffect {

        private Connection connection;
        private ISignalSending obj;

        public NewConnectionInputEffect(Connection connection, ISignalSending obj) {
            this.connection = connection;
            this.obj = obj;
        }

        public override void Apply() {
            if (obj is Node) {
                connection.GetRepresentation<ConnectionRepresentation>().AddNode(obj as Node, true);
            } else if (obj is ComponentInterface) {
                connection.GetRepresentation<ConnectionRepresentation>().AddCI(obj as ComponentInterface, true);
            }
        }

    }

}
