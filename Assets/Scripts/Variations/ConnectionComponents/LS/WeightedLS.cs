using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Spock {

    public class WeightedLS : LinkStorage {

        private readonly float INITIAL_WEIGHT = 1f;

        private readonly WeightedRandomSelectionList<ISignalSending> Inputs;
        private readonly WeightedRandomSelectionList<ISignalAccepting> Outputs;

        public WeightedLS() {
            Inputs = new WeightedRandomSelectionList<ISignalSending>();
            Outputs = new WeightedRandomSelectionList<ISignalAccepting>();
        }

        public override void Init() {
        }

        public override void AddInput(ISignalSending input) {
            LockFunctions.AcquireWritingLock(_rwLock);

            if (!Inputs.ContainsKey(input)) {
                Inputs.Add(input, INITIAL_WEIGHT);
            }

            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void AddOutput(ISignalAccepting output) {
            LockFunctions.AcquireWritingLock(_rwLock);

            if (!Outputs.ContainsKey(output)) {
                Outputs.Add(output, INITIAL_WEIGHT);
            }
                
            LockFunctions.ReleaseWritingLock(_rwLock);
        }
        
        public override bool HasInput(ISignalSending input) {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Inputs.ContainsKey(input);
            LockFunctions.ReleaseReadingLock(_rwLock);
            
            return b;
        }

        public override bool HasOutput(ISignalAccepting output) {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Outputs.ContainsKey(output);
            LockFunctions.ReleaseReadingLock(_rwLock);

            return b;
        }

        public override void RemoveInput(ISignalSending input) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Removing valid input", Inputs.ContainsKey(input));
            Inputs.Remove(input);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void RemoveOutput(ISignalAccepting output) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Removing valid output", Outputs.ContainsKey(output));
            Outputs.Remove(output);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void ForEachInput(Action<ISignalSending> callback) {
            LockFunctions.AcquireReadingLock(_rwLock);
            foreach (ISignalSending s in Inputs.Keys)
                callback.Invoke(s);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }

        public override void ForEachOutput(Action<ISignalAccepting> callback) {
            LockFunctions.AcquireReadingLock(_rwLock);
            foreach (ISignalAccepting s in Outputs.Keys)
                callback.Invoke(s);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }
        
        public override ISignalSending SelectWeightedRandomInput(object obj) {
            ISignalSending s = null;

            LockFunctions.AcquireReadingLock(_rwLock);

            s = Inputs.WeightedRandomValue();

            LockFunctions.ReleaseReadingLock(_rwLock);

            Assert.NotNull("SelectWeightedRandomInput found input", s);

            return s;
        }

        public override ISignalAccepting SelectWeightedRandomOutput(object obj) {
            ISignalAccepting s = null;

            LockFunctions.AcquireReadingLock(_rwLock);

            s = Outputs.WeightedRandomValue();

            LockFunctions.ReleaseReadingLock(_rwLock);

            Assert.NotNull("SelectWeightedRandomInput found input", s);

            return s;
        }

        public void ReinforceInput(ISignalSending input, float reward) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Input is valid", Inputs.ContainsKey(input));
            Inputs[input] += reward;

            if (Inputs[input] <= 0) {
                LockFunctions.ReleaseWritingLock(_rwLock);
                RemoveInput(input);

                // TODO Effect

            } else {
                LockFunctions.ReleaseWritingLock(_rwLock);

                // TODO Effect
                Debug.Log("Reinforced " + input + " -> " + Connection);

            }

        }

        public void ReinforceOutput(ISignalAccepting output, float reward) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Output is valid", Outputs.ContainsKey(output));
            Outputs[output] += reward;

            if (Outputs[output] <= 0) {
                LockFunctions.ReleaseWritingLock(_rwLock);
                RemoveOutput(output);

                // TODO Effect


            } else {
                LockFunctions.ReleaseWritingLock(_rwLock);

                // TODO Effect
                Debug.Log("Reinforced " + Connection + " -> " + output);

            }

        }

        public override void ReinforcePath(INetworkComponent input, INetworkComponent output, object obj, float reward) {

            float inputWeight = -1;
            float outputWeight = -1;

            if (input != null) {
                ReinforceInput(input, reward);
                inputWeight = Inputs[input];
            }

            if (output != null) {
                ReinforceOutput(output, reward);
                outputWeight = Outputs[output];
            }
            
            // TODO Move this into the individual functions
            Connection.N.S.Get<EffectController>().EnqueueEffect_TS(Connection.N.S.EffectFactory.NewUpdatePathwayStrengthEffect(input, 0, inputWeight, Connection, output, 0, outputWeight));

        }

        public override float GetInputWeighting(INetworkComponent input, object obj) {
            LockFunctions.AcquireReadingLock(_rwLock);
            Assert.True("Input is valid", Inputs.ContainsKey(input));
            float r = Inputs[input];
            LockFunctions.ReleaseReadingLock(_rwLock);
            
            return r;
        }
        
    }

}