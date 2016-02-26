using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Spock {

    [Serializable]
    public class Connection : INetworkComponent {

        //private readonly ReaderWriterLock _rwLock;

        public readonly Network N;
        public readonly ID<Connection> ID;

        private IRepresentation representation;
        public Transform transform { get { return representation.GetGameObject().transform; } }

        public LinkStorage LS;
        public SignalRouting SR;
        public ConnectionStrength CS;

        public Connection(Network N, ConnectionComponentTemplate template) {

            //_rwLock = new ReaderWriterLock();

            this.N = N;
            this.ID = N.S.NewID<Connection>(this);
            N.RegisterConnection(this);

            LS = template.InstantiateComponent<LinkStorage>();
            SR = template.InstantiateComponent<SignalRouting>();
            CS = template.InstantiateComponent<ConnectionStrength>();

            // Initalise components
            MultiphaseInitialiser.Initialise((ConnectionComponent obj) => obj.SetProperties(this), LS, SR, CS);

            N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewConnectionEffect(this));

        }

        public Connection(Network N, Connection original) {

            //_rwLock = new ReaderWriterLock();

            this.N = N;
            this.ID = N.S.NewID<Connection>(this);
            N.RegisterConnection(this);

            LS = original.LS;
            SR = original.SR;
            CS = original.CS;

            N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewConnectionEffect(this));

            // TODO send off duplication processing to a new thread like node, not signal
            throw new NotImplementedException();

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
            SelectiveDebug.LogSignalProgress("Connection.AcceptSignal: " + signal);

            JobFunctions.WaitThreadAMoment();

            N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewReceivedSignalEffect((INetworkComponent) sendingObj, this));

            // Register this connection with the signal
            signal.CR.VisitConnection(this);

            if (!LS.HasInput(sendingObj))
                Assert.True("Connection has node as input node", LS.HasInput(sendingObj));

            SR.RouteSignal(signal);

        }

        public void AddInputs(params ISignalSending[] newInputs) {
            foreach (ISignalSending input in newInputs) {
                if (!LS.HasInput(input)) {

                    LS.AddInput(input);
                    input.AddOutputs(this);

                    N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewConnectionInputEffect(this, input));

                }
            }
        }

        public void AddOutputs(params ISignalAccepting[] newOutputs) {
            foreach (ISignalAccepting output in newOutputs) {
                if (!LS.HasOutput(output)) {

                    LS.AddOutput(output);
                    output.AddInputs(this);

                    N.S.Get<EffectController>().EnqueueEffect_TS(N.S.EffectFactory.NewConnectionOutputEffect(this, output));

                }
            }
        }

        public override string ToString() {
            return "Connection " + ID.id;
        }

    }

}