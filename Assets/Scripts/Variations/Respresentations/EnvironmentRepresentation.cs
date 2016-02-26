using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class EnvironmentRepresentation : MonoBehaviour, IRepresentation {

        public Spock.Environment Environment;

        public GameObject GetGameObject() {
            return gameObject;
        }

        void Start() {
            Environment.Start();
        }

    }

}
