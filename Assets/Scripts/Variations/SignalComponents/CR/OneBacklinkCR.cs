using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class OneBacklinkCR : ComponentRegistration {

        public ISpockComponent currentNode;
        public Connection intermediateConnection; 
        public ISpockComponent previousNode;

        public override void Init() {

        }

        protected override SignalComponent NewComponentInstance() {
            return new OneBacklinkCR();
        }

        protected override void DuplicateComponentProperties(SignalComponent OriginalComponent) {
            OneBacklinkCR original = OriginalComponent as OneBacklinkCR;
            currentNode = original.currentNode;
            intermediateConnection = original.intermediateConnection;
            previousNode = original.previousNode;
        }

        public override void VisitNode(Node node) {
            LockFunctions.AcquireWritingLock(_rwLock);
            previousNode = currentNode;
            currentNode = node;
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void VisitComponentInterface(ComponentInterface ci) {
            if (ci is EntryComponentInterface)
                Signal.N = ci.N;

            LockFunctions.AcquireWritingLock(_rwLock);
            previousNode = currentNode;
            currentNode = ci;
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void VisitConnection(Connection connection) {
            LockFunctions.AcquireWritingLock(_rwLock);
            intermediateConnection = connection;
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public ISpockComponent GetPreviousNode() {
            LockFunctions.AcquireReadingLock(_rwLock);
            ISpockComponent c = previousNode;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return c;
        }

        public Connection GetPreviousConnection() {
            LockFunctions.AcquireReadingLock(_rwLock);
            Connection c = intermediateConnection;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return c;
        }

        public override void RewardForSignalPath(float reward) {
        }

    }

}