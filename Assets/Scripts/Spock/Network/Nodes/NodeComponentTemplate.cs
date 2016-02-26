using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NodeComponentTemplate {

        public static readonly Type[] RequiredComponentTypes = new Type[] {
                typeof(ActivationController),
                typeof(TargetSelection),
                typeof(DataProcessing),
                typeof(TransmissionContent),
                typeof(LifeCycle),
                typeof(EnergyManagement),
                typeof(MetricManagement),
                typeof(PropertyManagement),
                typeof(InputProcess),
                typeof(OutputProcess),
                typeof(StorageProcess)
            };

        private readonly Dictionary<Type, Type> components;
        private bool validated;

        public NodeComponentTemplate() {
            components = new Dictionary<Type, Type>();
            validated = false;
        }

        public NodeComponentTemplate SetComponents(params Type[] templateTypes) { // TODO custom components
            foreach (Type template in templateTypes) {
                foreach (Type type in RequiredComponentTypes) {
                    if (template.IsSubclassOf(type)) {
                        Assert.False("A component of this type has not yet been added", components.ContainsKey(type));
                        components.Add(type, template);
                        break;
                    }
                }
            }
            return this;
        }

        public NodeComponentTemplate Finalise() {
            foreach (Type type in RequiredComponentTypes) {
                Assert.True("Network component type has been registered", components.ContainsKey(type));
            }
            validated = true;
            return this;
        }

        public C InstantiateComponent<C>() where C : NodeComponent {
            Assert.True("Node component template object not yet validated", validated);

            Type componentType;
            Assert.True("Component has been registered", components.TryGetValue(typeof(C), out componentType));

            return (C) Activator.CreateInstance(componentType);
        } 

        public void InstantiateEach(Action<Type, NodeComponent> callback) {
            foreach (Type type in components.Keys) {
                callback.Invoke(type, (NodeComponent) Activator.CreateInstance(components[type]));
            }
        }

    }

}
