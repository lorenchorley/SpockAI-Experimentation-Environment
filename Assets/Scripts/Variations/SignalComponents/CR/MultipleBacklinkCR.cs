using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Spock {

    public class MultipleBacklinkCR : ComponentRegistration {

        private int _maxLinks = 4;
        public int maxLinks {
            get {
                LockFunctions.AcquireReadingLock(_rwLock);
                int n = _maxLinks;
                LockFunctions.ReleaseReadingLock(_rwLock);
                return n;
            }
            set {
                LockFunctions.AcquireWritingLock(_rwLock);
                _maxLinks = value;
                LockFunctions.ReleaseWritingLock(_rwLock);
            }
        }
        private List<INetworkComponent> backlinkSequence;

        public override void Init() {
            backlinkSequence = new List<INetworkComponent>();
        }

        protected override SignalComponent NewComponentInstance() {
            return new MultipleBacklinkCR();
        }

        protected override void DuplicateComponentProperties(SignalComponent OriginalComponent) {
            MultipleBacklinkCR original = OriginalComponent as MultipleBacklinkCR;
            LockFunctions.AcquireWritingLock(_rwLock);
            
            original.CopyData(this);
            Assert.True("component sequence not too long", backlinkSequence.Count <= maxLinks);

            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        protected void CopyData(MultipleBacklinkCR CR) {
            LockFunctions.AcquireReadingLock(_rwLock);

            // Copy the backlink from this component into duplicate
            for (int i = 0; i < backlinkSequence.Count; i++)
                CR.backlinkSequence.Insert(i, backlinkSequence[i]);

            // Copy the max backlinks from this component to the duplicate
            CR.maxLinks = _maxLinks;

            LockFunctions.ReleaseReadingLock(_rwLock);
        }

        public override void VisitNode(Node node) {
            LockFunctions.AcquireWritingLock(_rwLock);
            backlinkSequence.Insert(0, node);
            TrimEndOfList();
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void VisitComponentInterface(ComponentInterface ci) {
            if (ci is EnvironmentEntryCI) {
                Signal.N = null;
            } else if (ci is NetworkEntryCI) {
                Signal.N = ci.N;
                Signal.N.RegisterSignal(Signal);

                // Don't want to keep the backlinks for any previous network (Keep it when entering environments though)
                backlinkSequence.Clear();

            } else if (ci is ExitComponentInterface && Signal.N != null) {
                Signal.N.DeregisterSignal(Signal);
                Signal.N = null;
            }

            LockFunctions.AcquireWritingLock(_rwLock);
            backlinkSequence.Insert(0, ci);
            TrimEndOfList();
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override void VisitConnection(Connection connection) {
            LockFunctions.AcquireWritingLock(_rwLock);
            backlinkSequence.Insert(0, connection);
            TrimEndOfList();
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public ISpockComponent GetPreviousNode(int previousCount = 1) {
            Assert.True("previousCount is valid", previousCount >= 1 && previousCount <= maxLinks / 2); //TODO fix division

            ISpockComponent c = null;

            LockFunctions.AcquireReadingLock(_rwLock);

            if (backlinkSequence.Count > 0) {
                int index = 2 * (previousCount - 1);
                if (backlinkSequence[0] is Connection && backlinkSequence.Count >= index + 1) {
                    c = backlinkSequence[index + 1];
                    Assert.True("component is a node or component interface", c is Node || c is ComponentInterface);
                } else if (backlinkSequence[0] is Node && backlinkSequence.Count >= index) {
                    c = backlinkSequence[index];
                    Assert.True("component is a node or component interface", c is Node || c is ComponentInterface);
                }
            }

            LockFunctions.ReleaseReadingLock(_rwLock);

            return c;
        }

        public Connection GetPreviousConnection(int previousCount = 1) {
            Assert.True("previousCount is valid", previousCount >= 1 && previousCount <= maxLinks / 2);

            ISpockComponent c = null;

            LockFunctions.AcquireReadingLock(_rwLock);

            if (backlinkSequence.Count > 0) {
                int index = 2 * (previousCount - 1);
                if (backlinkSequence[0] is Connection && backlinkSequence.Count >= index ) {
                    c = backlinkSequence[index];
                    Assert.True("component is a node or component interface", c is Node || c is ComponentInterface);
                } else if (backlinkSequence[0] is Node && backlinkSequence.Count >= index + 1) {
                    c = backlinkSequence[index + 1];
                    Assert.True("component is a node or component interface", c is Node || c is ComponentInterface);
                }
            }

            LockFunctions.ReleaseReadingLock(_rwLock);

            return c as Connection;
        }

        private void TrimEndOfList() {
            if (backlinkSequence.Count > maxLinks) {
                backlinkSequence.RemoveAt(maxLinks);
                Assert.True("The number of component in the sequence is now equal to the maximum", backlinkSequence.Count == maxLinks);
            }
        }
        
        public override void RewardForSignalPath(float reward) {
            foreach (INetworkComponent c in backlinkSequence) {
                // For each connection at i, reinforce the path i-1, i, i+1
                if (c is Connection) {
                    int index = backlinkSequence.IndexOf(c);

                    INetworkComponent next = null;
                    INetworkComponent previous = null;

                    if (index != 0)
                        next = backlinkSequence[index - 1];

                    if (index != backlinkSequence.Count - 1)
                        previous = backlinkSequence[index + 1];

                    (c as Connection).LS.ReinforcePath(previous, next, Signal.SC.GetContent(), reward);

                }
            }
        }

    }

}