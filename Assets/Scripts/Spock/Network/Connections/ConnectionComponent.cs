using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Spock {

    public abstract class ConnectionComponent : IMonoPhaseInitialisation {

        protected readonly ReaderWriterLock _rwLock;

        public abstract void Init();

        public Connection Connection { get; private set; }

        public ConnectionComponent() { 
            _rwLock = new ReaderWriterLock();
        }

        public void SetProperties(Connection Connection) {
            LockFunctions.AcquireWritingLock(_rwLock);
            this.Connection = Connection;
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

    }

}
