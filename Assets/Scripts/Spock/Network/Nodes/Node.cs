using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Spock {

    [Serializable]
    public class Node : INetworkComponent {

        private readonly ReaderWriterLock _rwLock;

        public readonly Network N;
        public readonly ID<Node> ID;

        private IRepresentation representation;
        public Transform transform { get { return representation.GetGameObject().transform; } }

        //public ActivationController AC;
        //public TargetSelection TS;
        //public DataProcessing DP;
        //public TransmissionContent TC;
        //public LifeCycle LC;
        //public EnergyManagement EM;
        //public MetricManagement MM;
        //public PropertyManagement PM;
        public InputProcess IP;
        public OutputProcess OP;
        public StorageProcess SP;

        private readonly Dictionary<Type, NodeComponent> components;

        public Node(Network N, NodeComponentTemplate template) {

            _rwLock = new ReaderWriterLock();

            this.N = N;
            ID = N.S.NewID<Node>(this);
            N.RegisterNode(this);
            
            components = new Dictionary<Type, NodeComponent>();
            template.InstantiateEach((type, component) => components.Add(type, component));

            IP = (InputProcess) components[typeof(InputProcess)];
            OP = (OutputProcess) components[typeof(OutputProcess)];
            SP = (StorageProcess) components[typeof(StorageProcess)];

            // Initalise components
            MultiphaseInitialiser.Initialise((NodeComponent obj) => obj.SetProperties(this), components.Values);
            
            N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewNodeEffect(this));

        }

        public Node(Node Original, Action completionCallback = null) {

            _rwLock = new ReaderWriterLock();

            this.N = Original.N;
            ID = N.S.NewID<Node>(this);
            N.RegisterNode(this);

            components = new Dictionary<Type, NodeComponent>();


            // TODO there is a problem with this style
            // If a connection add this new node shell as an input/output before the components are duplicated
            // the original node with retain the opposing output/input link to the connection, while the connection still
            // reads it as the new node.
            // Could move all input output code to the node, like with the connection and leave signal content to the SP


            // Make a reference of each component from the original in this new node
            Original.ForEachComponentType((type, component) => Set(type, component));
            
            N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewNodeEffect(this));
            
            // Start a new thread to process the node duplication event
            ThreadController.AddJobToQueue(JobFunctions.ProcessDuplication, new object[] { this, completionCallback });

        }

        public void ForEachComponentType(Action<Type, NodeComponent> callback) {
            NodeComponent component;

            Type[] componentArray = new Type[components.Count];
            components.Keys.CopyTo(componentArray, 0);

            foreach (Type type in componentArray) {
                LockFunctions.AcquireReadingLock(_rwLock);
                component = components[type];
                LockFunctions.ReleaseReadingLock(_rwLock);

                callback.Invoke(type, component);
            }
        }

        public C Get<C>() where C : NodeComponent {
            LockFunctions.AcquireReadingLock(_rwLock);
            Assert.True("Getting component that exists on node", components.ContainsKey(typeof(C)));
            C c = (C) components[typeof(C)];
            LockFunctions.ReleaseReadingLock(_rwLock);

            return c;
        }

        public C Get<C>(C componentType) where C : NodeComponent {
            LockFunctions.AcquireReadingLock(_rwLock);
            Assert.True("Getting component that exists on node", components.ContainsKey(typeof(C)));
            C c = (C) components[typeof(C)];
            LockFunctions.ReleaseReadingLock(_rwLock);

            return c;
        }

        public void Set(Type type, NodeComponent component) {
            Assert.True("Type is a type of node component and of the right node component", component.GetType().IsSubclassOf(type));

            LockFunctions.AcquireWritingLock(_rwLock);
            if (components.ContainsKey(type))
                components[type] = component;
            else
                components.Add(type, component);

            if (type == typeof(InputProcess))
                IP = (InputProcess) component;

            if (type == typeof(OutputProcess))
                OP = (OutputProcess) component;

            if (type == typeof(StorageProcess))
                SP = (StorageProcess) component;

            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public void Set<C>(NodeComponent component) where C : NodeComponent {
            Type type = typeof(C);
            Assert.True("Type is a type of node component and of the right node component", component.GetType().IsSubclassOf(type));

            LockFunctions.AcquireWritingLock(_rwLock);
            if (components.ContainsKey(type))
                components[type] = component;
            else
                components.Add(type, component);

            if (type == typeof(InputProcess))
                IP = (InputProcess) component;

            if (type == typeof(OutputProcess))
                OP = (OutputProcess) component;

            if (type == typeof(StorageProcess))
                SP = (StorageProcess) component;

            LockFunctions.ReleaseWritingLock(_rwLock);
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

        public long GetID() {
            return ID.id;
        }

        public void AcceptSignal(Signal signal, ISignalSending sendingObj) {
            SelectiveDebug.LogSignalProgress("Node.AcceptSignal: " + signal);

            // Make sure that this is never run from the main thread
            ThreadController.AssertNotMainThread();

            // Wait if required
            JobFunctions.WaitThreadAMoment();

            // Exit if the application is closing
            if (ThreadController.I.stopThreads)
                return;

            // Start effect for this node accepting signal
            N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewReceivedSignalEffect((INetworkComponent) sendingObj, this));

            // Register this node with the signal
            signal.CR.VisitNode(this);

            // Send this signal to the input process
            if (IP.AcceptSignal(signal)) {

                // If the signal was stored, just run a normal iteration of the output loop
                OP.OutputIteration(OutputProcess.CONTINUE_THIS_THREAD);

            } else {

                // If the signal wasn't stored, we can use the signal again straight away
                OP.OutputIteration(OutputProcess.CONTINUE_THIS_THREAD, signal);

            }

        }

        public void AddInputs(params ISignalSending[] newInputs) {
            foreach (ISignalSending input in newInputs) {
                if (!SP.HasInput(input)) {

                    SP.AddInput(input);

                    input.AddOutputs(this);

                    // TODO replace this and all others like it with connection oriented effects (here for eg: NewConnectionOutputEffect())
                    N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewNodeInputEffect(this, input));

                }
            }
        }

        public void AddOutputs(params ISignalAccepting[] newOutputs) {
            foreach (ISignalAccepting output in newOutputs) {
                if (!SP.HasOutput(output)) {

                    SP.AddOutput(output);

                    output.AddInputs(this);

                    N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewNodeOutputEffect(this, output));

                }
            }
        }

        public override string ToString() {
            return "Node " + ID.id;
        }

    }

}