using System;
using System.Collections;

namespace Spock {

    public abstract class SignalContent : SignalComponent {

        public abstract void RequestCopyOfData(SignalContent SC);
        public abstract void SetContent(object obj);
        public abstract object GetContent();
        public abstract int GetHashCodeOfContent();

    }

}