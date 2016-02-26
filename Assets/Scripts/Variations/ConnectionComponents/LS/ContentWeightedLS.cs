using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Spock {

    public class ContentWeightedLS : LinkStorage {

        private readonly HashsetWeightedRandomSelectionList<ISignalSending> Inputs;
        private readonly HashsetWeightedRandomSelectionList<ISignalAccepting> Outputs;

        public ContentWeightedLS() {
            Inputs = new HashsetWeightedRandomSelectionList<ISignalSending>(WNAController.I.HashBins, SettingsController.I.InitialWeight, SettingsController.I.MinimumWeight, WNAController.I.GetHashOf);
            Outputs = new HashsetWeightedRandomSelectionList<ISignalAccepting>(WNAController.I.HashBins, SettingsController.I.InitialWeight, SettingsController.I.MinimumWeight, WNAController.I.GetHashOf);
        }

        public override void Init() {
        }

        public override void AddInput(ISignalSending input) {
            LockFunctions.AcquireWritingLock(_rwLock);

            if (!Inputs.Contains(input)) {
                Inputs.Add(input);
            }

            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void AddOutput(ISignalAccepting output) {
            LockFunctions.AcquireWritingLock(_rwLock);

            if (!Outputs.Contains(output)) {
                Outputs.Add(output);
            }
                
            LockFunctions.ReleaseWritingLock(_rwLock);
        }
        
        public override bool HasInput(ISignalSending input) {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Inputs.Contains(input);
            LockFunctions.ReleaseReadingLock(_rwLock);
            
            return b;
        }

        public override bool HasOutput(ISignalAccepting output) {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Outputs.Contains(output);
            LockFunctions.ReleaseReadingLock(_rwLock);

            return b;
        }

        public override void RemoveInput(ISignalSending input) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Removing valid input", Inputs.Contains(input));
            Inputs.Remove(input);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void RemoveOutput(ISignalAccepting output) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Removing valid output", Outputs.Contains(output));
            Outputs.Remove(output);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void ForEachInput(Action<ISignalSending> callback) {
            LockFunctions.AcquireReadingLock(_rwLock);
            foreach (ISignalSending s in Inputs.Items)
                callback.Invoke(s);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }

        public override void ForEachOutput(Action<ISignalAccepting> callback) {
            LockFunctions.AcquireReadingLock(_rwLock);
            foreach (ISignalAccepting s in Outputs.Items)
                callback.Invoke(s);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }
        
        public override ISignalSending SelectWeightedRandomInput(object obj) {
            ISignalSending s = null;

            LockFunctions.AcquireReadingLock(_rwLock);

            s = Inputs.WeightedRandomValue(obj);

            LockFunctions.ReleaseReadingLock(_rwLock);

            Assert.NotNull("SelectWeightedRandomInputByObject found input", s);

            return s;
        }

        public override ISignalAccepting SelectWeightedRandomOutput(object obj) {
            ISignalAccepting s = null;

            LockFunctions.AcquireReadingLock(_rwLock);

            s = Outputs.WeightedRandomValue(obj);

            LockFunctions.ReleaseReadingLock(_rwLock);

            Assert.NotNull("SelectWeightedRandomOutputByObject found input", s);

            return s;
        }

        public void ReinforceInput(ISignalSending input, object obj, float reward) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Input is valid", Inputs.Contains(input));
            Inputs.AddWeight(input, obj, reward);

            if (Inputs.GetWeight(input, obj) <= 0) {
                LockFunctions.ReleaseWritingLock(_rwLock);
                RemoveInput(input);

                // TODO Effect

            } else {
                LockFunctions.ReleaseWritingLock(_rwLock);

                // TODO Effect
                //Debug.Log("Reinforced " + input + " -> " + Connection);

            }

        }

        public void ReinforceOutput(ISignalAccepting output, object obj, float reward) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Output is valid", Outputs.Contains(output));
            Outputs.AddWeight(output, obj, reward);

            if (Outputs.GetWeight(output, obj) <= 0) {
                LockFunctions.ReleaseWritingLock(_rwLock);
                RemoveOutput(output);

                // TODO Effect


            } else {
                LockFunctions.ReleaseWritingLock(_rwLock);

                // TODO Effect
                //Debug.Log("Reinforced " + Connection + " -> " + output);

            }

        }

        public override void ReinforcePath(INetworkComponent input, INetworkComponent output, object obj, float reward) {

            float inputWeight = -1;
            float outputWeight = -1;

            int hash = WNAController.I.GetHashOf(obj);

            if (input != null) {
                ReinforceInput(input, obj, reward);
                inputWeight = GetInputWeighting(input, hash);
            }

            if (output != null) {
                ReinforceOutput(output, obj, reward);
                outputWeight = GetOutputWeighting(output, hash);
            }

            // TODO Move this into the individual functions
            Connection.N.S.Get<EffectController>().EnqueueEffect_TS(Connection.N.S.EffectFactory.NewUpdatePathwayStrengthEffect(input, hash, inputWeight, Connection, output, hash, outputWeight));

        }

        public override float GetInputWeighting(INetworkComponent input, object obj) {
            LockFunctions.AcquireReadingLock(_rwLock);
            Assert.True("Input is valid", Inputs.Contains(input));
            float r = Inputs.GetWeight(input, obj);
            LockFunctions.ReleaseReadingLock(_rwLock);
            
            return r;
        }

        public float GetOutputWeighting(INetworkComponent output, object obj) {
            LockFunctions.AcquireReadingLock(_rwLock);
            Assert.True("Output is valid", Outputs.Contains(output));
            float r = Outputs.GetWeight(output, obj);
            LockFunctions.ReleaseReadingLock(_rwLock);

            return r;
        }
        
    }

}