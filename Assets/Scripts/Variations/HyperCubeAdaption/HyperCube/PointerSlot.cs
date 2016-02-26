
using System;

namespace Spock {

    public class PointerSlot : HyperCubeSlot {

        private HyperCubeIndex target;
        private bool isExit;
        private Action callbackOnExit;

        public PointerSlot(HyperCubeIndex internalTarget) {
            this.target = internalTarget;
            this.isExit = false;
        }

        public PointerSlot(Action callbackOnExit) {
            this.callbackOnExit = callbackOnExit;
            this.isExit = true;
        }

        public override bool IsExitSlot() {
            return isExit;
        }

        public HyperCubeIndex GetInternalTarget() {
            return target;
        }

        public void CallExitFunction() {
            callbackOnExit.Invoke();
        }

    }

}