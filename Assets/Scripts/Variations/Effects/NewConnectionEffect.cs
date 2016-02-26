using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewConnectionEffect : INewConnectionEffect {

        private Connection connection;

        public NewConnectionEffect(Connection connection) {
            this.connection = connection;
        }

        public override void Apply() {
            RepresentationFunctions.CreateRepresentation(connection);
        }

    }

}
