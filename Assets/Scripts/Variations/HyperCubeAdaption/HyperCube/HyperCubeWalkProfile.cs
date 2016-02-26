
namespace Spock {

    public class HyperCubeWalkProfile<S, I> where S : HyperCubeSlot where I : HyperCubeIndex {

        private bool lastWalkExited;
        private I lastWalksExitIndex;
        private S lastWalksExitSlot;
        private I[] lastWalksLoop;
        
        public void RegisterLastWalkExitedAt(I index, S slot) {
            lastWalkExited = true;
            lastWalksExitIndex = index;
            lastWalksExitSlot = slot;
        }

        public void RegisterLastWalkEndedInLoop(I[] loop) {
            lastWalkExited = false;
            lastWalksLoop = loop;
        }

        public bool GetLastWalkExited() {
            return lastWalkExited;
        }

        public I GetLastWalksExitIndex() {
            return lastWalksExitIndex;
        }

        public S GetLastWalksExitSlot() {
            return lastWalksExitSlot;
        }

        public I[] GetLastWalksLoop() {
            return lastWalksLoop;
        }

    }

}