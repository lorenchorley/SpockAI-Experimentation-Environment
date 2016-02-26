using UnityEngine;
using System.Collections.Generic;
using System;

namespace Spock {

	public interface IEffectFactory {

        INewNetworkEffect NewNetworkEffect(Network network);
        INewEnvironmentEffect NewEnvironmentEffect(Environment environment);
        INewNodeEffect NewNodeEffect(Node node);
        INewConnectionEffect NewConnectionEffect(Connection connection);
        INewComponentInterfaceEffect NewComponentInterfaceEffect(ComponentInterface ci);
        INewSignalEffect NewSignalEffect(Signal signal);

        IDeleteSignalEffect NewDeleteSignalEffect(Signal signal);

        INewNodeInputEffect NewNodeInputEffect(Node node, ISignalSending obj);
        INewNodeOutputEffect NewNodeOutputEffect(Node node, ISignalAccepting obj);
        INewConnectionInputEffect NewConnectionInputEffect(Connection connection, ISignalSending obj);
        INewConnectionOutputEffect NewConnectionOutputEffect(Connection connection, ISignalAccepting obj);
        INewComponentInterfaceInputEffect NewComponentInterfaceInputEffect(ComponentInterface ci, ISignalSending obj);
        INewComponentInterfaceOutputEffect NewComponentInterfaceOutputEffect(ComponentInterface ci, ISignalAccepting obj);

        IReceivedSignalEffect NewReceivedSignalEffect(INetworkComponent fromComponent, INetworkComponent toComponent);
        ISentSignalEffect NewSentSignalEffect(INetworkComponent fromComponent, INetworkComponent toComponent);
        IUpdatePathwayStrengthEffect NewUpdatePathwayStrengthEffect(INetworkComponent fromComponent, int fromHash, float fromStrength, Connection connection, INetworkComponent toComponent, int toHash, float toStrength);

        IRemoveConnectionInputEffect NewRemoveConnectionInputEffect(Connection connection, Node node);
        IRemoveConnectionOutputEffect NewRemoveConnectionOutputEffect(Connection connection, Node node);

    }

}
