using UnityEngine;
using System.Collections;

namespace Spock {

    public abstract class ComponentRegistration : SignalComponent {

        public abstract void VisitComponentInterface(ComponentInterface ci);
        public abstract void VisitNode(Node node);
        public abstract void VisitConnection(Connection connection);
        public abstract void RewardForSignalPath(float reward);

    }

}