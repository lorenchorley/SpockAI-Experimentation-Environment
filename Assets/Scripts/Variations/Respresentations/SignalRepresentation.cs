using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class SignalRepresentation : MonoBehaviour, IRepresentation {

        public Signal Signal;

        public GameObject GetGameObject() {
            return gameObject;
        }

    }

}
