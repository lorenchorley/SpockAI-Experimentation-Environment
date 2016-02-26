using UnityEngine;
using System.Collections.Generic;
using System;

namespace Spock {

	public class EffectFactory : IEffectFactory {

        public INewConnectionEffect NewConnectionEffect(Connection connection) {
            return new NewConnectionEffect(connection);
        }

        public INewEnvironmentEffect NewEnvironmentEffect(Environment environment) {
            return new NewEnvironmentEffect(environment);
        }

        public INewNetworkEffect NewNetworkEffect(Network network) {
            return new NewNetworkEffect(network);
        }

        public INewNodeEffect NewNodeEffect(Node node) {
            return new NewNodeEffect(node);
        }

        public INewSignalEffect NewSignalEffect(Signal signal) {
            return new NewSignalEffect(signal);
        }

        public INewComponentInterfaceEffect NewComponentInterfaceEffect(ComponentInterface ci) {
            return new NewComponentInterfaceEffect(ci);
        }

        public IDeleteSignalEffect NewDeleteSignalEffect(Signal signal) {
            return new DeleteSignalEffect(signal);
        }

        public INewComponentInterfaceInputEffect NewComponentInterfaceInputEffect(ComponentInterface ci, ISignalSending obj) {
            return new NewComponentInterfaceInputEffect(ci, obj);
        }

        public INewComponentInterfaceOutputEffect NewComponentInterfaceOutputEffect(ComponentInterface ci, ISignalAccepting obj) {
            return new NewComponentInterfaceOutputEffect(ci, obj);
        }

        public INewConnectionInputEffect NewConnectionInputEffect(Connection connection, ISignalSending obj) {
            return new NewConnectionInputEffect(connection, obj);
        }

        public INewConnectionOutputEffect NewConnectionOutputEffect(Connection connection, ISignalAccepting obj) {
            return new NewConnectionOutputEffect(connection, obj);
        }

        public INewNodeInputEffect NewNodeInputEffect(Node node, ISignalSending obj) {
            return new NewNodeInputEffect(node, obj);
        }

        public INewNodeOutputEffect NewNodeOutputEffect(Node node, ISignalAccepting obj) {
            return new NewNodeOutputEffect(node, obj);
        }

        public IReceivedSignalEffect NewReceivedSignalEffect(INetworkComponent fromComponent, INetworkComponent toComponent) {
            return new ReceivedSignalEffect(fromComponent, toComponent);
        }

        public ISentSignalEffect NewSentSignalEffect(INetworkComponent fromComponent, INetworkComponent toComponent) {
            return new SentSignalEffect(fromComponent, toComponent);
        }

        public IUpdatePathwayStrengthEffect NewUpdatePathwayStrengthEffect(INetworkComponent fromComponent, int fromHash, float fromStrength, Connection connection, INetworkComponent toComponent, int toHash, float toStrength) {
            return new UpdatePathwayStrengthEffect(fromComponent, fromHash, fromStrength, connection, toComponent, toHash, toStrength);
        }


        public IRemoveConnectionInputEffect NewRemoveConnectionInputEffect(Connection connection, Node node) {
            return new RemoveConnectionInputEffect(connection, node);
        }

        public IRemoveConnectionOutputEffect NewRemoveConnectionOutputEffect(Connection connection, Node node) {
            return new RemoveConnectionOutputEffect(connection, node);
        }

    }

}
