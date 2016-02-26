using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class ConnectionRepresentation : MonoBehaviour, IRepresentation {

        public Connection Connection;

        private Dictionary<INetworkComponent, LineRenderer> ComponentInputs;
        private Dictionary<INetworkComponent, LineRenderer> ComponentOutputs;

        private Material lineMaterial = null;
        private Color colourBase;

        public ConnectionRepresentation() {
            ComponentInputs = new Dictionary<INetworkComponent, LineRenderer>();
            ComponentOutputs = new Dictionary<INetworkComponent, LineRenderer>();
        }

        void Start() {

            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();

            rb.useGravity = false;
            rb.mass = 0.5f;

            colourBase = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));

            lineMaterial = new Material(ObjectController.I.ConnectionLineMaterialBase);
            lineMaterial.color = generateRandomColor(colourBase);
            lineMaterial.name += " copy";
            
            foreach (LineRenderer line in ComponentInputs.Values)
                line.material = lineMaterial;

            foreach (LineRenderer line in ComponentOutputs.Values)
                line.material = lineMaterial;

        }

        private Color generateRandomColor(Color mix) {
            System.Random random = new System.Random();
            
            return new Color(Mathf.Clamp01(((float) random.NextDouble() + mix.r) / 2),
                             Mathf.Clamp01(((float) random.NextDouble() + mix.g) / 2),
                             Mathf.Clamp01(((float) random.NextDouble() + mix.b) / 2));
        }

        public LineRenderer GetLineForInput(INetworkComponent component) {
            if (ComponentInputs.ContainsKey(component))
                return ComponentInputs[component];
            else
                return null;
        }

        public LineRenderer GetLineForOutput(INetworkComponent component) {
            if (ComponentOutputs.ContainsKey(component))
                return ComponentOutputs[component];
            else
                return null;
        }

        public void AddCI(ComponentInterface ci, bool input) {
            AddEndPoint(ci, input);
        }

        public void AddNode(Node node, bool input) {
            AddEndPoint(node, input);
        }

        private void AddEndPoint(INetworkComponent component, bool input) {

            if ((input && ComponentInputs.ContainsKey(component)) || (!input && ComponentOutputs.ContainsKey(component)))
                return;

            // Setup and register new line
            LineRenderer newLine = Instantiate(ObjectController.I.ConnectionTemplate);
            newLine.transform.SetParent(transform);
            newLine.useWorldSpace = true;
            if (lineMaterial != null)
                newLine.material = lineMaterial;

            if (input) {
                ComponentInputs.Add(component, newLine);
                newLine.name = component.ToString() + " -> " + Connection.ToString();
            } else {
                ComponentOutputs.Add(component, newLine);
                newLine.name = Connection.ToString() + " -> " + component.ToString();
            }

            // Make sure that a spring is connected from this component to all other connections of the opposite type
            foreach (INetworkComponent c in (!input ? ComponentInputs : ComponentOutputs).Keys)
                RepresentationFunctions.AddSpringConnection(c.GetRepresentation().GetGameObject(), component.GetRepresentation().GetGameObject());
            
        }

        public void RemoveCI(ComponentInterface ci, bool input) {
            RemoveEndPoint(ci, input);
        }

        public void RemoveNode(Node node, bool input) {
            RemoveEndPoint(node, input);
        }

        private void RemoveEndPoint(INetworkComponent component, bool input) {

            if ((input && !ComponentInputs.ContainsKey(component)) || (!input && !ComponentOutputs.ContainsKey(component)))
                return;

            if (input) {
                Destroy(ComponentInputs[component].gameObject);
                ComponentInputs.Remove(component);
            } else {
                Destroy(ComponentOutputs[component].gameObject);
                ComponentOutputs.Remove(component);
            }

            foreach (INetworkComponent c in (!input ? ComponentInputs : ComponentOutputs).Keys)
                RepresentationFunctions.RemoveSpringConnection(c.GetRepresentation().GetGameObject(), component.GetRepresentation().GetGameObject());

        }

        void Update() {

            Vector3 inputAverage = Vector3.zero;
            Vector3 outputAverage = Vector3.zero;

            foreach (INetworkComponent component in ComponentInputs.Keys) {
                inputAverage += component.GetRepresentation().GetGameObject().transform.position;
            }

            foreach (INetworkComponent component in ComponentOutputs.Keys) {
                outputAverage += component.GetRepresentation().GetGameObject().transform.position;
            }

            if (ComponentInputs.Keys.Count != 0) {
                inputAverage = inputAverage * (1f / ComponentInputs.Keys.Count);

                if (ComponentOutputs.Keys.Count != 0) {
                    transform.position = 0.5f * (inputAverage + outputAverage * (1f / ComponentOutputs.Keys.Count));
                } else {
                    transform.position = inputAverage;
                }

            } else if (ComponentOutputs.Keys.Count != 0) {
                transform.position = outputAverage * (1f / ComponentOutputs.Keys.Count);
            }

            // Draw lines
            foreach (INetworkComponent component in ComponentInputs.Keys) {
                LineRenderer line = ComponentInputs[component];
                line.SetPosition(0, transform.position);
                line.SetPosition(1, component.GetRepresentation().GetGameObject().transform.position);
            }
            foreach (INetworkComponent component in ComponentOutputs.Keys) {
                LineRenderer line = ComponentOutputs[component];
                line.SetPosition(0, transform.position);
                line.SetPosition(1, component.GetRepresentation().GetGameObject().transform.position);
            }

        }

        public GameObject GetGameObject() {
            return gameObject;
        }

    }

}
