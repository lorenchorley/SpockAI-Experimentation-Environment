using UnityEngine;
using System.Collections;

namespace Spock {

    public abstract class ActivationController : NodeComponent {

        protected bool enabled;

        public abstract bool RequestActivation();
        
        public void SetEnabled(bool enabled) {
            this.enabled = enabled;

            if (enabled && Node.OP.IterationRequired()) {
                // Start a thread to start a thread for each signal that leaves this object
                ThreadController.AddJobToQueue(JobFunctions.ProcessEverythingOnNode, Node);
            }

        }

    }

}