using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class ReceivedSignalEffect : IReceivedSignalEffect {

        private static readonly Vector3 one = new Vector3(1, 1, 1);

        private INetworkComponent toComponent;
        private INetworkComponent fromComponent;

        public ReceivedSignalEffect(INetworkComponent fromComponent, INetworkComponent toComponent) {
            this.fromComponent = fromComponent;
            this.toComponent = toComponent;
        }

        public override void Apply() {

            if (toComponent is Connection) {

                // Want to replace obj with the appropriate child object of the connection representation
                ConnectionRepresentation connectionRep = toComponent.GetRepresentation<ConnectionRepresentation>();
                LineRenderer line = connectionRep.GetLineForInput(fromComponent);

                Assert.NotNull("The line from the sender to the receiving connection", line);

                BriefEnlargement.ApplyBriefEnlargement(line.gameObject, 0.1f);

            } else {

                BriefEnlargement.ApplyBriefEnlargement(toComponent.GetRepresentation().GetGameObject(), 0.1f, () => {
                    if (toComponent is Node) {
                        int multiplier = (toComponent as Node).SP.SignalCount();
                        NodeRepresentation NodeRep = toComponent.GetRepresentation<NodeRepresentation>();
                        NodeRep.GetGameObject().transform.localScale = 0.1f * multiplier * one + NodeRep.OriginalScale;
                    }
                });
            }

        }

    }

}
