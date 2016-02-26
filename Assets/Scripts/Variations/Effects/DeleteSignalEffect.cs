using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class DeleteSignalEffect : IDeleteSignalEffect {

        private Signal signal;

        public DeleteSignalEffect(Signal signal) {
            this.signal = signal;
        }

        public override void Apply() {
            if (signal.GetRepresentation() != null)
                GameObject.Destroy(signal.GetRepresentation().GetGameObject());
        }

    }

}
