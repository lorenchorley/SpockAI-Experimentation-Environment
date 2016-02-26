using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class SentSignalEffect : ISentSignalEffect {

        //private static readonly Vector3 one = new Vector3(1, 1, 1);

        private INetworkComponent fromComponent;
        private INetworkComponent toComponent;

        public SentSignalEffect(INetworkComponent fromComponent, INetworkComponent toComponent) {
            this.fromComponent = fromComponent;
            this.toComponent = toComponent;
        }

        public override void Apply() {

            if (fromComponent is Connection) {

                // Want to replace obj with the appropriate child object of the connection representation
                ConnectionRepresentation connectionRep = fromComponent.GetRepresentation<ConnectionRepresentation>();
                LineRenderer line = connectionRep.GetLineForOutput(toComponent);

                Assert.NotNull("The line from the sender to the receiving connection", line);

                BriefEnlargement.ApplyBriefEnlargement(line.gameObject, 0.1f);

            }
            //} else {

            //    BriefEnlargement.ApplyBriefEnlargement(fromComponent.GetRepresentation().GetGameObject(), 0.1f, () => {
            //        if (fromComponent is Node) {
            //            int multiplier = (fromComponent as Node).SP.SignalCount();
            //            NodeRepresentation NodeRep = fromComponent.GetRepresentation<NodeRepresentation>();
            //            NodeRep.GetGameObject().transform.localScale = 0.1f * multiplier * one + NodeRep.OriginalScale;
            //        }
            //    });
            //}

        }

    }

}
