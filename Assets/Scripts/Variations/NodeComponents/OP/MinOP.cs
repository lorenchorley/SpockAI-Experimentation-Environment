using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Spock {

    public class MinOP : OutputProcess {
        
        public override void Init() {

        }

        protected override NodeComponent NewComponentInstance() {
            return new MinOP();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

        public override bool IterationRequired() {
            return Node.Get<ActivationController>().RequestActivation() && Node.Get<TargetSelection>().CanTarget() && Node.Get<TransmissionContent>().HasContent();
        }

        public override void OutputIteration(bool NewThreadOnSend, Signal signalToSend = null) {
            SelectiveDebug.LogSignalProgress("OutputProcess.OutputIteration");

            if (ThreadController.I.stopThreads)
                return;

            if (Node.Get<ActivationController>().RequestActivation()) {

                if (signalToSend == null) {
                    signalToSend = Node.Get<TransmissionContent>().GetContent();
                    if (signalToSend == null)
                        return;
                }

                ISignalAccepting target = Node.Get<TargetSelection>().SelectTarget(signalToSend);
                if (target == null) {
                    Node.Get<TransmissionContent>().ReplaceContent(signalToSend);
                    return;
                }

                // If further iteration is required, start a new thread to take care of it
                if (IterationRequired())
                    ThreadController.AddJobToQueue(JobFunctions.ProcessEverythingOnNode, Node);

                Node.N.S.Get<EffectController>().EnqueueEffect_TS(Node.N.S.EffectFactory.NewSentSignalEffect(Node, (Connection) target));

                if (NewThreadOnSend)
                    ThreadController.AddJobToQueue(JobFunctions.ProcessAcceptSignal, new object[] { Node, signalToSend, target });
                else
                    target.AcceptSignal(signalToSend, Node);

            }
        }

    }

}