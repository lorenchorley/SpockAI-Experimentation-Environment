using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Spock {

    public class WeightedSP : StorageProcess {

        private readonly float INITIAL_WEIGHT = 1f;

        public WeightedRandomSelectionList<ISignalSending> Inputs;
        public WeightedRandomSelectionList<ISignalAccepting> Outputs;
        public LinkedList<Signal> Signals;
        
        public override void Init() {
            Inputs = new WeightedRandomSelectionList<ISignalSending>();
            Outputs = new WeightedRandomSelectionList<ISignalAccepting>();
            Signals = new LinkedList<Signal>();
        }

        protected override NodeComponent NewComponentInstance() {
            return new WeightedSP();
        }

        protected override void DuplicateComponentProperties(NodeComponent OriginalComponent) {
            WeightedSP original = (WeightedSP) OriginalComponent;

            LockFunctions.AcquireWritingLock(_rwLock);

            original.RequestCopyOfData(Inputs, Outputs, Signals);

            LockFunctions.ReleaseWritingLock(_rwLock);

            LockFunctions.AcquireReadingLock(_rwLock);

            // Update the outputs in the input connections of the new node
            foreach (ISignalSending obj in Inputs.Keys)
                obj.AddOutputs(Node);

            // Update the inputs in the output connections of the new node
            foreach (ISignalAccepting obj in Outputs.Keys)
                obj.AddInputs(Node);

            LockFunctions.ReleaseReadingLock(_rwLock);

        }

        public void RequestCopyOfData(Dictionary<ISignalSending, float> inputsCopy, Dictionary<ISignalAccepting, float> outputsCopy, LinkedList<Signal> signalsCopy) {

            LockFunctions.AcquireReadingLock(_rwLock);

            // Copy all data into the new instance
            LinkedListNode<Signal> node = Signals.First;
            while (node != null) {
                signalsCopy.AddLast(node.Value);
                node = node.Next;
            }

            // Copy all connections from the original
            foreach (ISignalSending input in Inputs.Keys)
                inputsCopy.Add(input, Inputs[input]);
            foreach (ISignalAccepting output in Outputs.Keys)
                outputsCopy.Add(output, Outputs[output]);

            LockFunctions.ReleaseReadingLock(_rwLock);

        }

        public override void StoreSignal(Signal signal) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Signals.AddLast(signal);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override Signal ViewNextSignal() {
            LockFunctions.AcquireReadingLock(_rwLock);
            Signal s = Signals.First.Value;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return s;
        }

        public override Signal PopNextSignal() {
            LockFunctions.AcquireWritingLock(_rwLock);
            Signal s = null;
            if (Signals.First != null) {
                s = Signals.First.Value;
                Signals.RemoveFirst();
            }
            LockFunctions.ReleaseWritingLock(_rwLock);

            return s;
        }

        public override void PushNextSignal(Signal signal) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Signals.AddFirst(signal);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override bool HasSignals() {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Signals.Count > 0;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return b;
        }

        public override void AddInput(ISignalSending obj) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Inputs.Add(obj, INITIAL_WEIGHT);
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void AddOutput(ISignalAccepting obj) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Outputs.Add(obj, INITIAL_WEIGHT);
            LockFunctions.ReleaseWritingLock(_rwLock);

            Node.Get<TargetSelection>().NewOutputRegistered(obj);
        }

        public override bool HasInput(ISignalSending obj) {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Inputs.ContainsKey(obj);
            LockFunctions.ReleaseReadingLock(_rwLock);

            return b;
        }

        public override bool HasOutput(ISignalAccepting obj) {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Outputs.ContainsKey(obj);
            LockFunctions.ReleaseReadingLock(_rwLock);

            return b;
        }

        public override void RemoveInput(ISignalSending input) {
            LockFunctions.AcquireReadingLock(_rwLock);
            Assert.True("Removing valid input", Inputs.ContainsKey(input));
            Inputs.Remove(input);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }

        public override void RemoveOutput(ISignalAccepting output) {
            LockFunctions.AcquireReadingLock(_rwLock);
            Assert.True("Removing valid output", Outputs.ContainsKey(output));
            Outputs.Remove(output);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }

        public override void EraseAllInputs() { // TODO proxy these in StorageProcess so that the effects arent being generated by the implementation
            LockFunctions.AcquireWritingLock(_rwLock);

            foreach (Connection connection in Inputs.Keys) {
                Node.N.S.Get<EffectController>().EnqueueEffect_TS(Node.N.S.EffectFactory.NewRemoveConnectionOutputEffect(connection, Node));
                if (connection.LS.HasOutput(Node))
                    connection.LS.RemoveOutput(Node);
            }
            Inputs.Clear();

            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void EraseAllOutputs() {
            LockFunctions.AcquireWritingLock(_rwLock);

            foreach (Connection connection in Outputs.Keys) {
                Node.N.S.Get<EffectController>().EnqueueEffect_TS(Node.N.S.EffectFactory.NewRemoveConnectionInputEffect(connection, Node));
                if (connection.LS.HasInput(Node))
                    connection.LS.RemoveInput(Node);
            }
            Outputs.Clear();

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

        public override bool HasInputs() {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Inputs.Count > 0;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return b;
        }

        public override bool HasOutputs() {
            LockFunctions.AcquireReadingLock(_rwLock);
            bool b = Outputs.Count > 0;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return b;
        }

        public override int InputCount() {
            LockFunctions.AcquireReadingLock(_rwLock);
            int i = Inputs.Count;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return i;
        }

        public override int OutputCount() {
            LockFunctions.AcquireReadingLock(_rwLock);
            int i = Outputs.Count;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return i;
        }

        public override int SignalCount() {
            LockFunctions.AcquireReadingLock(_rwLock);
            int i = Signals.Count;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return i;
        }

        public override ISignalSending GetRandomInput() {
            LockFunctions.AcquireReadingLock(_rwLock);
            ISignalSending s = Inputs.WeightedRandomValue();
            LockFunctions.ReleaseReadingLock(_rwLock);

            return s;
        }

        public override ISignalAccepting GetRandomOutput() {
            LockFunctions.AcquireReadingLock(_rwLock);
            ISignalAccepting s = Outputs.WeightedRandomValue();
            LockFunctions.ReleaseReadingLock(_rwLock);

            return s;
        }

        public void ReinforceInput(ISignalSending input, float reward) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Input is valid", Inputs.ContainsKey(input));
            Inputs[input] += reward;

            if (Inputs[input] <= 0) {
                LockFunctions.ReleaseWritingLock(_rwLock);
                RemoveInput(input);
            } else {
                //TotalInputWeight += reward;
                LockFunctions.ReleaseWritingLock(_rwLock);
            }

        }

        public void ReinforceOutput(ISignalAccepting output, float reward) {
            LockFunctions.AcquireWritingLock(_rwLock);
            Assert.True("Output is valid", Outputs.ContainsKey(output));
            Outputs[output] += reward;

            if (Outputs[output] <= 0) {
                LockFunctions.ReleaseWritingLock(_rwLock);
                RemoveOutput(output);
            } else {
                //TotalOutputWeight += reward;
                LockFunctions.ReleaseWritingLock(_rwLock);
            }

        }

        public override float GetInputWeighting(INetworkComponent input, object obj) {
            throw new NotImplementedException();
        }

        public override float GetOutputWeighting(INetworkComponent output, object obj) {
            throw new NotImplementedException();
        }

    }

}