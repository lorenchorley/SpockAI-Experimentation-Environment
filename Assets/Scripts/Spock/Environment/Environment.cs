using UnityEngine;
using System.Collections.Generic;
using System;

namespace Spock {

    [Serializable]
    public abstract class Environment : ISpockComponent {

        public abstract void AcceptSignal(Signal signal, int channel);
        public abstract void Start(); // Called from representation

        protected Dictionary<int, ComponentInterface> CIs;

        public SpockInstance S;
        private IRepresentation representation;

        public Environment(SpockInstance S) {
            this.S = S;
            CIs = new Dictionary<int, ComponentInterface>();

            // Raise event
            //S.RaiseEvent((int) SpockInstance.EventCode.NewEnvironment, this);
            S.Get<EffectController>().EnqueueEffect_TS(S.EffectFactory.NewEnvironmentEffect(this));

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

        public void SetOutgoingInterface(ComponentInterface CI, int channel) {
            CIs.Add(channel, CI);
        }

    }

}