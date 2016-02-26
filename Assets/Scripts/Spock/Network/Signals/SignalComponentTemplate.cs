using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class SignalComponentTemplate {

        public static readonly Type[] RequiredComponentTypes = new Type[] {
                typeof(ComponentRegistration),
                typeof(SignalContent)
            };

        private readonly Dictionary<Type, Type> components;
        private bool validated;

        public SignalComponentTemplate() {
            components = new Dictionary<Type, Type>();
            validated = false;
        }

        public SignalComponentTemplate SetComponents(params Type[] templateTypes) { // TODO custom components
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

        public SignalComponentTemplate Finalise() {
            foreach (Type type in RequiredComponentTypes) {
                Assert.True("Network component type has been registered", components.ContainsKey(type));
            }
            validated = true;
            return this;
        }

        public C InstantiateComponent<C>() where C : SignalComponent {
            Assert.True("Signal component template object not yet validated", validated);
             
            Type componentType;
            Assert.True("Component has been registered: " + typeof(C).Name, components.TryGetValue(typeof(C), out componentType));

            return (C) Activator.CreateInstance(componentType);
        } 

    }

}
