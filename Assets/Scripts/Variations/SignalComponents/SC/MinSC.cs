using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public class MinSC : SignalContent {

        private object obj;

        public override void Init() {
            
        }

        protected override SignalComponent NewComponentInstance() {
            return new MinSC();
        }

        protected override void DuplicateComponentProperties(SignalComponent OriginalComponent) {
            ((SignalContent) OriginalComponent).RequestCopyOfData(this);
        }

        public override void RequestCopyOfData(SignalContent SC) {
            LockFunctions.AcquireReadingLock(_rwLock);
            SC.SetContent(obj);
            LockFunctions.ReleaseReadingLock(_rwLock);
        }

        public override object GetContent() {
            LockFunctions.AcquireReadingLock(_rwLock);
            object o = obj;
            LockFunctions.ReleaseReadingLock(_rwLock);

            return o;
        }

        public override void SetContent(object obj) {
            LockFunctions.AcquireWritingLock(_rwLock);
            this.obj = obj;
            LockFunctions.ReleaseWritingLock(_rwLock);
        }

        public override int GetHashCodeOfContent() {
            return obj.GetHashCode();
        }
    }

}