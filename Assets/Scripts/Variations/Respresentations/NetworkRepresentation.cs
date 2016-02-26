using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class NetworkRepresentation : MonoBehaviour, IRepresentation {
        
        private Spock.Network _Network;
        public Spock.Network Network {
            get {
                return _Network;
            }
            set {
                _Network = value;

                transform.position = ObjectController.I.NetworkOrigin;

            }
        }

        public GameObject GetGameObject() {
            return gameObject;
        }

    }

}
