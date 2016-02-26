using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Spock {
    
    public class Network : ISpockComponent {

        private readonly ReaderWriterLock _rwLock;

        public readonly SpockInstance S;

        public readonly string name;
        public Dictionary<ID<Node>, Node> Nodes;
        public Dictionary<ID<Connection>, Connection> Connections;
        private Dictionary<ID<Signal>, Signal> Signals;
        
        public NodeComponentTemplate NodeTemplate { get; private set; }
        public ConnectionComponentTemplate ConnectionTemplate { get; private set; }
        public SignalComponentTemplate SignalTemplate { get; private set; }

        protected Dictionary<int, ComponentInterface> CIs;

        private IRepresentation representation;

        public Network(SpockInstance S, string name) {

            _rwLock = new ReaderWriterLock();

            this.S = S;
            this.name = name;

            Nodes = new Dictionary<ID<Node>, Node>();
            Connections = new Dictionary<ID<Connection>, Connection>();
            Signals = new Dictionary<ID<Signal>, Signal>();
            CIs = new Dictionary<int, ComponentInterface>();
           
            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewNetworkEffect(this));

        }

        public void SetRepresentation(IRepresentation representation) {
            this.representation = representation;
        }

        public IRepresentation GetRepresentation() {
            return representation;
        }

        public R GetRepresentation<R>() where R : IRepresentation {
            return (R) representation;
        }

        public Node NewNode() {
            return new Node(this, NodeTemplate);
        }

        public Node NewNode(NodeComponentTemplate template) {
            return new Node(this, template);
        }

        public Connection NewConnection() {
            return new Connection(this, ConnectionTemplate);
        }

        public Connection NewConnection(ConnectionComponentTemplate template) {
            return new Connection(this, template);
        }

        public Signal NewSignal() {
            return new Signal(S, this, SignalTemplate);
        }

        public Signal NewSignal(SignalComponentTemplate template) {
            return new Signal(S, this, template);
        }

        public void RegisterNodeTemplate(NodeComponentTemplate template) {
            NodeTemplate = template;
        }

        public void RegisterConnectionTemplate(ConnectionComponentTemplate template) {
            ConnectionTemplate = template;
        }

        public void RegisterSignalTemplate(SignalComponentTemplate template) {
            SignalTemplate = template;
        }
        
        public void RegisterNode(Node node) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Node is not yet registered", !Nodes.ContainsKey(node.ID));
            Nodes.Add(node.ID, node);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public void RegisterConnection(Connection connection) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Connection is not yet registered", !Connections.ContainsKey(connection.ID));
            Connections.Add(connection.ID, connection);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public void RegisterSignal(Signal signal) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Signal is not yet registered", !Signals.ContainsKey(signal.ID));
            Signals.Add(signal.ID, signal);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public void DeregisterNode(Node node) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Node is valid", Nodes.ContainsKey(node.ID));
            Nodes.Remove(node.ID);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public void DeregisterConnection(Connection connection) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Connection is valid", Connections.ContainsKey(connection.ID));
            Connections.Remove(connection.ID);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public void DeregisterSignal(Signal signal) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Signal is valid", Signals.ContainsKey(signal.ID));
            Signals.Remove(signal.ID);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public void RewardNetworkwide(float reward) {
            Debug.Log("RewardNetwork: " + reward);

            LockFunctions.AcquireReadingLock(_rwLock);
            foreach (Signal signal in Signals.Values) {
                signal.CR.RewardForSignalPath(reward);
            }

            LockFunctions.ReleaseReadingLock(_rwLock);
        }

    }

}