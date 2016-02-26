using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Spock {

    // Selects an output in one of two ways:
    // If there are new outputs, it randomly picks one
    // If there are no new outputs, using a weighted random selection, it picks an output based on weights from the input entry of this node in its output connections
    public class WeightedTS : TargetSelection {

        // For each possible hash of signal content, maintain a list of outputs that haven't been tried
        private RandomSelectionList<ISignalAccepting>[] outputsToTry;

        public override void Init() {
            outputsToTry = new RandomSelectionList<ISignalAccepting>[WNAController.I.HashBins];
            for (int i = 0; i < WNAController.I.HashBins; i++)
                outputsToTry[i] = new RandomSelectionList<ISignalAccepting>();
        }

        protected override NodeComponent NewComponentInstance() {
            return new WeightedTS();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
        }

        public override bool CanTarget() {
            return Node.SP.HasOutputs();
        }

        public override ISignalAccepting SelectTarget(Signal signal) {
            ISignalAccepting target;

            int hash = WNAController.I.GetHashOf(signal.SC.GetContent());

            // Check signals to try before anything else
            LockFunctions.AcquireReadingLock(_rwLock);
            bool stillGoing = outputsToTry[hash].Count > 0;
            LockFunctions.ReleaseReadingLock(_rwLock);

            while (stillGoing) {

                // Try each signal to try, and if one is an output of this node, use it
                LockFunctions.AcquireWritingLock(_rwLock);
                target = outputsToTry[hash].SelectAndRemoveRandom();
                LockFunctions.ReleaseWritingLock(_rwLock);
                if (Node.SP.HasOutput(target))
                    return target;

                LockFunctions.AcquireReadingLock(_rwLock);
                stillGoing = outputsToTry[hash].Count > 0;
                LockFunctions.ReleaseReadingLock(_rwLock);

            }

            WeightedRandomSelector<ISignalAccepting> selector = new WeightedRandomSelector<ISignalAccepting>();
            
            // Collect the weights of this node as an input node in all its output connections
            Node.SP.ForEachOutput(output => { // TODO find a more efficient way to do this, only use node output weights and dont use connection input weights. Input weights are for signals going the other way or something
                Assert.True("obj is a Connection", output is Connection);
                Connection connection = (output as Connection);

                float reinforcementLevel = connection.LS.GetInputWeighting(Node, signal.SC.GetContent());

                selector.Add(connection, reinforcementLevel);
            });

            // Select a random weight
            return selector.GetRandomSelection();
        }

        public override void NewOutputRegistered(ISignalAccepting obj) {
            LockFunctions.AcquireWritingLock(_rwLock);
            for (int i = 0; i < WNAController.I.HashBins; i++)
                outputsToTry[i].Add(obj);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

    }

}