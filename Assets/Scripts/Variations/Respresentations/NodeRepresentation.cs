using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class NodeRepresentation : MonoBehaviour, IRepresentation {

        public Node Node;
        public Vector3 OriginalScale { get; private set; }

        void Awake() {
            transform.position = 5 * UnityEngine.Random.insideUnitSphere + ObjectController.I.NetworkOrigin;
            OriginalScale = transform.localScale;
        }

        public void AddConnection(Connection connection, bool input) {
            connection.GetRepresentation<ConnectionRepresentation>().AddNode(Node, input);
        }
        
        public GameObject GetGameObject() {
            return gameObject;
        }


        // Drag code
        private Vector3 offset;
        private Vector3 screenPoint;
        private float _lockedZPosition;

        void OnMouseDown() {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            _lockedZPosition = gameObject.transform.position.z;
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }

        void OnMouseDrag() {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            curPosition.z = _lockedZPosition;
            transform.position = curPosition;
        }

    }

}
