using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public abstract class LinkStorage : ConnectionComponent {

        public abstract void AddInput(ISignalSending input);
        public abstract void AddOutput(ISignalAccepting output);
        public abstract bool HasInput(ISignalSending input);
        public abstract bool HasOutput(ISignalAccepting output);
        public abstract void RemoveInput(ISignalSending input);
        public abstract void RemoveOutput(ISignalAccepting output);
        public abstract void ForEachInput(Action<ISignalSending> callback);
        public abstract void ForEachOutput(Action<ISignalAccepting> callback);
        public abstract void ReinforcePath(INetworkComponent input, INetworkComponent output, object obj, float reward);
        public abstract float GetInputWeighting(INetworkComponent input, object obj);
        public abstract ISignalSending SelectWeightedRandomInput(object obj);
        public abstract ISignalAccepting SelectWeightedRandomOutput(object obj);

    }

    }