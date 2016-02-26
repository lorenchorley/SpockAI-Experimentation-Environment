using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Spock {
    
    public abstract class ExitComponentInterface : ComponentInterface {

        protected EntryComponentInterface eci;
        
        public ExitComponentInterface(SpockInstance S) : base(S) { }

        public void ConnectToEntryComponentInterface(EntryComponentInterface eci) {
            this.eci = eci;
        }

    }

}