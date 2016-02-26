using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Spock {

    public abstract class SignalComponent : IMonoPhaseInitialisation {

        protected readonly ReaderWriterLock _rwLock;

        public abstract void Init();
        protected abstract SignalComponent NewComponentInstance();
        protected abstract void DuplicateComponentProperties(SignalComponent OriginalComponent);

        public Signal Signal { get; private set; }

        public SignalComponent() { 
            _rwLock = new ReaderWriterLock();
        }

        public S Duplicate<S>(Signal newSignal) where S : SignalComponent {

            // Ask the original component (this instance) to create a new instance of itself
            SignalComponent newComponent = (S) NewComponentInstance();
            Assert.True("newComponent is S", newComponent is S);

            // Set the reference to the new Signal into the new component instance
            newComponent.SetProperties(newSignal);

            LockFunctions.AcquireWritingLock(_rwLock);

            // Initialise the new instance
            newComponent.Init();

            // Ask the new component instance to copy any necessary values out of the original component instance (this)
            newComponent.DuplicateComponentProperties(this);

            LockFunctions.ReleaseWritingLock(_rwLock);

            return newComponent as S;
        }

        public void SetProperties(Signal Signal) {
            LockFunctions.AcquireWritingLock(_rwLock);
            this.Signal = Signal;
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

    }

}
