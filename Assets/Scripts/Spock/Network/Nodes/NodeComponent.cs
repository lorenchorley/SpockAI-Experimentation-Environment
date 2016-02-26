using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Spock {

    public abstract class NodeComponent : IMonoPhaseInitialisation {

        protected readonly ReaderWriterLock _rwLock;

        public abstract void Init();
        protected abstract NodeComponent NewComponentInstance();
        protected abstract void DuplicateComponentProperties(NodeComponent OriginalComponent);

        public Node Node { get; private set; }

        public NodeComponent() { 
            _rwLock = new ReaderWriterLock();
        }

        public NodeComponent Duplicate(Type type, Node newNode) {

            // Ask the original component (this instance) to create a new instance of itself
            NodeComponent newComponent = NewComponentInstance();
            Assert.True("newComponent is correct type", newComponent.GetType().IsSubclassOf(type));

            // Set the reference to the new node into the new component instance and initialise it
            newComponent.SetProperties(newNode);

            // Initialise the new component
            newComponent.Init();

            LockFunctions.AcquireReadingLock(_rwLock);

            // Ask the new component instance to copy any necessary values out of the original component instance (this)
            newComponent.DuplicateComponentProperties(this);

            LockFunctions.ReleaseReadingLock(_rwLock);

            return newComponent;
        }

        public N Duplicate<N>(Node newNode) where N : NodeComponent {

            // Ask the original component (this instance) to create a new instance of itself
            NodeComponent newComponent = (N) NewComponentInstance();
            Assert.True("newComponent is N", newComponent is N);

            // Set the reference to the new node into the new component instance and initialise it
            newComponent.SetProperties(newNode);

            LockFunctions.AcquireWritingLock(_rwLock);

            // Initialise the new component
            newComponent.Init();

            // Ask the new component instance to copy any necessary values out of the original component instance (this)
            newComponent.DuplicateComponentProperties(this);

            LockFunctions.ReleaseWritingLock(_rwLock);

            return newComponent as N;
        }

        public void SetProperties(Node Node) {
            LockFunctions.AcquireWritingLock(_rwLock);
            this.Node = Node;
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

    }

}
