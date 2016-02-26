using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Spock {

    public abstract class StorageProcess : NodeComponent {

        public abstract void StoreSignal(Signal signal);
        public abstract bool HasSignals();
        public abstract Signal ViewNextSignal();
        public abstract Signal PopNextSignal();
        public abstract void PushNextSignal(Signal signal);
        public abstract void AddInput(ISignalSending obj);
        public abstract void AddOutput(ISignalAccepting obj);
        public abstract bool HasInput(ISignalSending obj);
        public abstract bool HasOutput(ISignalAccepting obj);
        public abstract bool HasInputs();
        public abstract bool HasOutputs();
        public abstract void RemoveInput(ISignalSending input);
        public abstract void RemoveOutput(ISignalAccepting output);
        public abstract int InputCount();
        public abstract int OutputCount();
        public abstract int SignalCount();
        public abstract ISignalSending GetRandomInput();
        public abstract ISignalAccepting GetRandomOutput();
        public abstract void EraseAllInputs();
        public abstract void EraseAllOutputs();
        public abstract void ForEachInput(Action<ISignalSending> callback);
        public abstract void ForEachOutput(Action<ISignalAccepting> callback);
        public abstract float GetInputWeighting(INetworkComponent input, object obj);
        public abstract float GetOutputWeighting(INetworkComponent output, object obj);


    }

}