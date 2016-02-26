using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class ConnectionComponentTemplate {

        public static readonly Type[] RequiredComponentTypes = new Type[] {
                typeof(LinkStorage),
                typeof(SignalRouting),
                typeof(ConnectionStrength)
            };

        private readonly Dictionary<Type, Type> components;
        private bool validated;

        public ConnectionComponentTemplate() {
            components = new Dictionary<Type, Type>();
            validated = false;
        }

        public ConnectionComponentTemplate SetComponents(params Type[] templateTypes) { // TODO custom components
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

        public ConnectionComponentTemplate Finalise() {
            foreach (Type type in RequiredComponentTypes) {
                Assert.True("Network component type has been registered", components.ContainsKey(type));
            }
            validated = true;
            return this;
        }

        public C InstantiateComponent<C>() where C : ConnectionComponent {
            Assert.True("Connection component template object not yet validated", validated);

            Type componentType;
            Assert.True("Component has been registered", components.TryGetValue(typeof(C), out componentType));

            return (C) Activator.CreateInstance(componentType);
        } 

    }

}
