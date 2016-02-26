using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    [RequireComponent(typeof(Rigidbody))]
    public class ComponentInterfaceRepresentation : MonoBehaviour, IComponentInterfaceRepresentation {

        private ComponentInterface _CI;
        public ComponentInterface CI {
            get {
                return _CI;
            }
            set {
                _CI = value;

                transform.SetParent(CI.GetContainingComponent().GetRepresentation().GetGameObject().transform);

                if (CI.N is Network) {
                    transform.localPosition = Vector3.zero;

                    if (value is EntryComponentInterface) {
                        transform.localPosition -= ObjectController.I.NetworkCIOffsets;
                    } else if (value is ExitComponentInterface) {
                        transform.localPosition += ObjectController.I.NetworkCIOffsets;
                    }

                } else if (CI.GetContainingComponent() is Environment) {
                    transform.position = ObjectController.I.EnvironmentOrigin;

                    if (value is EntryComponentInterface) {
                        transform.position += ObjectController.I.EnvironmentCIOffsets;
                    } else if (value is ExitComponentInterface) {
                        transform.position -= ObjectController.I.EnvironmentCIOffsets;
                    }

                }

            }
        }
        
        void Awake() {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }

        public void AddConnection(Connection connection, bool input) {
            connection.GetRepresentation<ConnectionRepresentation>().AddCI(CI, input);
        }

        public void SetColour(Color color) {
            GetComponent<Renderer>().material.color = color;
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
