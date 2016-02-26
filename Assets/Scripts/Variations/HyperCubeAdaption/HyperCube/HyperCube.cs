using System.Threading;
using UnityEngine;

namespace Spock {

    public abstract class HyperCube<S, I> where S : HyperCubeSlot where I : HyperCubeIndex {

        protected readonly ReaderWriterLock l = new ReaderWriterLock();

        public abstract S GetSlot(I index);
        public abstract bool IsValidIndex(I index);

        public virtual void Walk(HyperCubeWalker<S, I> walker, I entryIndex) {
            I currentIndex = entryIndex;
            S currentSlot = GetSlot(currentIndex);
            
            while (KeepLoopingCondition(walker, currentIndex, currentSlot)) { 
                currentIndex = walker.Step(this, currentIndex, currentSlot);
                currentSlot = GetSlot(currentIndex);
            }

            walker.LastProfile = CreateProfile(walker, currentIndex, currentSlot);

        }

        public virtual bool KeepLoopingCondition(HyperCubeWalker<S, I> walker, I currentIndex, S currentSlot) {
            return !currentSlot.IsExitSlot(); // not repeating any index (TODO), and currentIndex is not an exit slot
        }

        public virtual HyperCubeWalkProfile<S, I> CreateProfile(HyperCubeWalker<S, I> walker, I currentIndex, S currentSlot) {
            HyperCubeWalkProfile<S, I> walkProfile = new HyperCubeWalkProfile<S, I>();

            if (currentSlot.IsExitSlot())
                walkProfile.RegisterLastWalkExitedAt(currentIndex, currentSlot);
            else {
                walkProfile.RegisterLastWalkEndedInLoop(null); // TODO
            }

            return walkProfile;
        }

    }

}