using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    [Serializable]
    public class Signal : INetworkComponent {

        //private readonly object _lock;

        public SpockInstance S;
        public Network N;
        public readonly ID<Signal> ID;

        private IRepresentation representation;

        public ComponentRegistration CR;
        public SignalContent SC;

        public Signal(SpockInstance S, Network N, SignalComponentTemplate template) {

            //_lock = new object();

            this.S = S;
            this.N = N;
            this.ID = S.NewID<Signal>(this);
            if (N != null)
                N.RegisterSignal(this);

            CR = template.InstantiateComponent<ComponentRegistration>();
            SC = template.InstantiateComponent<SignalContent>();

            // Initalise components
            MultiphaseInitialiser.Initialise((SignalComponent obj) => obj.SetProperties(this), CR, SC);

            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewSignalEffect(this));

        }

        public Signal(Signal original, ISignalSending sendImmediatelyFrom = null, ISignalAccepting sendImmediatelyTo = null) {

            //_lock = new object();

            N = original.N;
            S = N.S;
            ID = S.NewID(this);

            CR = original.CR;
            SC = original.SC;

            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewSignalEffect(this));

            Action callbackInNewThread = null;
            if (sendImmediatelyFrom != null && sendImmediatelyTo != null) {
                callbackInNewThread = (Action) delegate {

                    // Once the duplication is finished, process the accept in the new thread
                    JobFunctions.ProcessAcceptSignal(new object[] { sendImmediatelyFrom, this, sendImmediatelyTo });

                };
            }

            // Process the duplication in a new thread
            ThreadController.AddJobToQueue(JobFunctions.ProcessDuplication, new object[] { this, callbackInNewThread});

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

        public override string ToString() {
            return "Signal " + ID.id + " (" + SC.GetContent() + ")";
        }

        public void AcceptSignal(Signal signal, ISignalSending sendingObj) {
            Assert.Never("");
        }

        public void AddInputs(params ISignalSending[] newInputs) {
            Assert.Never("");
        }

        public void AddOutputs(params ISignalAccepting[] newOutputs) {
            Assert.Never("");
        }

        public void DeleteSignal() {
            if (representation != null)
                S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewDeleteSignalEffect(this));
        }

    }

}