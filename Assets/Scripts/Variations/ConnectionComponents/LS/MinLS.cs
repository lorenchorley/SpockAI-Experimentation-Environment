using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Spock {

    public class MinLS : LinkStorage {

        private readonly List<ISignalSending> Inputs;
        private readonly List<ISignalAccepting> Outputs;

        public MinLS() {
            Inputs = new List<ISignalSending>();
            Outputs = new List<ISignalAccepting>();
        }

        public override void Init() {

        }

        public override void AddInput(ISignalSending input) {
            LockFunctions.AcquireWritingLock(_rwLock);
            
            if (!Inputs.Contains(input))
                Inputs.Add(input);

            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void AddOutput(ISignalAccepting output) {
            LockFunctions.AcquireWritingLock(_rwLock);

            if (!Outputs.Contains(output))
                Outputs.Add(output);
                
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
            foreach (ISignalSending s in Inputs)
                callback.Invoke(s);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }

        public override void ForEachOutput(Action<ISignalAccepting> callback) {
            LockFunctions.AcquireReadingLock(_rwLock);
            foreach (ISignalAccepting s in Outputs)
                callback.Invoke(s);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }

        public void ForEachOutputLastSeparately(Action<ISignalAccepting> callback, Action<ISignalAccepting> lastCallback) {
            // TODO deal with no outputs

            for (int i = 1; i < Outputs.Count; i++) {
                callback.Invoke(Outputs[i]);
            }
            
            if (Outputs.Count > 0)
                lastCallback.Invoke(Outputs[0]);

        }

        public override void ReinforcePath(INetworkComponent input, INetworkComponent output, object obj, float reward) {
        }

        public override float GetInputWeighting(INetworkComponent input, object obj) {
            throw new NotImplementedException();
        }

        public override ISignalSending SelectWeightedRandomInput(object obj) {
            throw new NotImplementedException();
        }

        public override ISignalAccepting SelectWeightedRandomOutput(object obj) {
            throw new NotImplementedException();
        }
    }

}