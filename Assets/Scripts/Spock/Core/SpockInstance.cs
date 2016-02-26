using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Spock {

    public class SpockInstance {

        private readonly object _lock;

        public readonly Dictionary<string, Environment> EnvironmentsByID;
        public readonly Dictionary<string, NodeComponentTemplate> NodeComponentTemplatesByID;
        public readonly Dictionary<string, ConnectionComponentTemplate> ConnectionComponentTemplatesByID;
        public readonly Dictionary<string, SignalComponentTemplate> SignalComponentTemplatesByID;
        public readonly Dictionary<string, Network> NetworksByID;
        public readonly Dictionary<string, ComponentInterface> CIsByID;
        public readonly Dictionary<ISpockComponent, List<ComponentInterface>> CIsByContainingComponent;
        public readonly Dictionary<ISpockComponent, IRepresentation> RepresentationsByRelatedComponent;
        public readonly Dictionary<Type, IController> Controllers;
        public readonly IEffectFactory EffectFactory;

        private long nodeCounter;
        private long connectionCounter;
        private long signalCounter;
        private long interfaceCounter;

        public SpockInstance(Dictionary<Type, IController> Controllers, IEffectFactory EffectFactory) {

            _lock = new object();

            this.Controllers = Controllers;
            this.EffectFactory = EffectFactory;

            EnvironmentsByID = new Dictionary<string, Environment>();
            NodeComponentTemplatesByID = new Dictionary<string, NodeComponentTemplate>();
            ConnectionComponentTemplatesByID = new Dictionary<string, ConnectionComponentTemplate>();
            SignalComponentTemplatesByID = new Dictionary<string, SignalComponentTemplate>();
            NetworksByID = new Dictionary<string, Network>();
            CIsByID = new Dictionary<string, ComponentInterface>();
            CIsByContainingComponent = new Dictionary<ISpockComponent, List<ComponentInterface>>();
            RepresentationsByRelatedComponent = new Dictionary<ISpockComponent, IRepresentation>();

            nodeCounter = 0;
            connectionCounter = 0;
            signalCounter = 0;
            interfaceCounter = 0;

        }

        // Get controller function
        public C Get<C>() where C : IController {
            IController controller;

            if (!Controllers.TryGetValue(typeof(C), out controller))
                Assert.Never("There is no controller \"" + typeof(C).Name + "\" registered with the Abstraction Engine.");

            return (C) controller;
        }
        
        public Environment RegisterEnvironment(string id, Environment environment) {
            Assert.False("Environment not yet registered", EnvironmentsByID.ContainsKey(id));
            EnvironmentsByID.Add(id, environment);
            return environment;
        }

        public NodeComponentTemplate RegisterNodeComponentTemplate(string id, NodeComponentTemplate template) {
            Assert.False("NodeComponentTemplate not yet registered", NodeComponentTemplatesByID.ContainsKey(id));
            NodeComponentTemplatesByID.Add(id, template);
            //NodeComponentTemplatesByID.Add(template.GetRelatedComponent(), template); // TODO
            return template;
        }

        public ConnectionComponentTemplate RegisterConnectionComponentTemplate(string id, ConnectionComponentTemplate template) {
            Assert.False("ConnectionComponentTemplate not yet registered", ConnectionComponentTemplatesByID.ContainsKey(id));
            ConnectionComponentTemplatesByID.Add(id, template);
            //NodeComponentTemplatesByID.Add(template.GetRelatedComponent(), template); // TODO
            return template;
        }
        
        public SignalComponentTemplate RegisterSignalComponentTemplate(string id, SignalComponentTemplate template) {
            Assert.False("SignalComponentTemplate not yet registered", SignalComponentTemplatesByID.ContainsKey(id));
            SignalComponentTemplatesByID.Add(id, template);
            //SignalComponentTemplatesByID.Add(template.GetRelatedComponent(), template); // TODO
            return template;
        }

        public ComponentInterface RegisterComponentInterface(string id, ComponentInterface ci) {
            Assert.False("Environment not yet registered", CIsByID.ContainsKey(id));
            CIsByID.Add(id, ci);

            List<ComponentInterface> interfacesForComponent;
            if (!CIsByContainingComponent.TryGetValue(ci.GetContainingComponent(), out interfacesForComponent)) {
                interfacesForComponent = new List<ComponentInterface>();
                CIsByContainingComponent.Add(ci.GetContainingComponent(), interfacesForComponent);
            }
            interfacesForComponent.Add(ci);

            return ci;
        }

        public Network NewNetwork(string id) {
            Assert.False("Network not yet registered", NetworksByID.ContainsKey(id));
            Network N = new Network(this, id);
            NetworksByID.Add(id, N);
            return N;
        }
        
        public Node NewNode(string networkID, NodeComponentTemplate template) {
            Network N;
            Assert.True("Network ID is valid", NetworksByID.TryGetValue(networkID, out N));
            return N.NewNode(template);
        }

        public ID<C> NewID<C>(C component) where C : INetworkComponent {

            long count = 0;
            Type componentType = typeof(C);

            if (componentType == typeof(Node)) {
                lock (_lock) count = nodeCounter++;
            } else if (componentType == typeof(Connection)) {
                lock (_lock) count = connectionCounter++;
            } else if (componentType == typeof(Signal)) {
                lock (_lock) count = signalCounter++;
            } else if (componentType == typeof(ComponentInterface)) {
                lock (_lock) count = interfaceCounter++;
            }

            return new ID<C>(count, component);
        }

    }

}
