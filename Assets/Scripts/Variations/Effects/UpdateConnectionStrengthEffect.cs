using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class UpdatePathwayStrengthEffect : IUpdatePathwayStrengthEffect {

        private INetworkComponent fromComponent;
        private int fromHash;
        private float fromStrength;
        private Connection connection;
        private INetworkComponent toComponent;
        private int toHash;
        private float toStrength;

        public UpdatePathwayStrengthEffect(INetworkComponent fromComponent, int fromHash, float fromStrength, Connection connection, INetworkComponent toComponent, int toHash, float toStrength) {
            this.fromComponent = fromComponent;
            this.fromHash = fromHash;
            this.fromStrength = fromStrength;
            this.connection = connection;
            this.toComponent = toComponent;
            this.toHash = toHash;
            this.toStrength = toStrength;
        }

        public override void Apply() {

            //Debug.Log("Update link: " + fromStrength + " for hash " + fromHash);

            // Want to replace obj with the appropriate child object of the connection representation
            ConnectionRepresentation connectionRep = connection.GetRepresentation<ConnectionRepresentation>();

            if (fromComponent != null) {
                LineRenderer fromLine = connectionRep.GetLineForInput(fromComponent);
                Assert.NotNull("The line from the sending object to the receiving connection", fromLine);
                ConnectionLink.GetConnectionLink(fromLine.gameObject).strength[fromHash] = fromStrength;
            }

            if (toComponent != null) {
                LineRenderer toLine = connectionRep.GetLineForOutput(toComponent);
                Assert.NotNull("The line from the sending connection to the receiving object", toLine);
                ConnectionLink.GetConnectionLink(toLine.gameObject).strength[toHash] = toStrength;
            }

        }

    }

}
