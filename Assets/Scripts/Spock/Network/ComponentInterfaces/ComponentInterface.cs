using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Spock {

    [Serializable]
    public abstract class ComponentInterface : INetworkComponent {

        private readonly ReaderWriterLock _rwLock;

        public abstract void AcceptSignal(Signal signal, ISignalSending sendingObj);

        public SpockInstance S;
        public Network N;
        public Environment E;
        public readonly ID<ComponentInterface> ID;

        private IRepresentation representation;
        //private ISpockComponent ContainingComponent;

        // TODO pass these off to components
        protected readonly List<ISignalSending> InputNodes; 
        protected readonly List<ISignalAccepting> OutputNodes;

        public ComponentInterface(SpockInstance S) {

            _rwLock = new ReaderWriterLock();

            this.S = S;
            //this.ContainingComponent = ContainingComponent;
            ID = S.NewID<ComponentInterface>(this);

            InputNodes = new List<ISignalSending>();
            OutputNodes = new List<ISignalAccepting>();

            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewComponentInterfaceEffect(this));

        }

        public long GetID() {
            return ID.id;
        }

        public ISpockComponent GetContainingComponent() {
            return (N != null) ? N as ISpockComponent : E as ISpockComponent;
        }

        public bool IsNetworkInterface() {
            return N != null;
        }

        public bool IsEnvironmentInterface() {
            return E != null;
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

        public void AddInputs(params ISignalSending[] newInputs) {
            foreach (ISignalSending input in newInputs) {
                if (!InputNodes.Contains(input)) {

                    LockFunctions.AcquireWritingLock(_rwLock);
                    InputNodes.Add(input);
                    LockFunctions.ReleaseWritingLock(_rwLock);

                    input.AddOutputs(this);

                    S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewComponentInterfaceInputEffect(this, input));

                }
            }
        }

        public void AddOutputs(params ISignalAccepting[] newOutputs) {
            foreach (ISignalAccepting output in newOutputs) {
                if (!OutputNodes.Contains(output)) {

                    LockFunctions.AcquireWritingLock(_rwLock);
                    OutputNodes.Add(output);
                    LockFunctions.ReleaseWritingLock(_rwLock);

                    output.AddInputs(this);

                    S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewComponentInterfaceOutputEffect(this, output));

                }
            }
        }

    }

}