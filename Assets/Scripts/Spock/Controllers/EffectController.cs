#define USE_EFFECTS

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spock {

	public class EffectController : MonoBehaviour, IController {

		public static EffectController I;

#if USE_EFFECTS

        // Lock
        private static readonly object _queueLock = new object();

        private IVisualEffect effect;
		private Queue<IVisualEffect> localEffectsCopy;
        
        public EffectController() {
			localEffectsCopy = new Queue<IVisualEffect>();
		}
        
#endif

        public void SetSpockInstance(SpockInstance S) { }

        public void SetEnabled(bool enable) {
			this.enabled = enable;
		}

		public void Preinit() {

			// Setup static instance reference
			EffectController.I = this;

		}
		
		public void Init() {

		}

#if USE_EFFECTS

        void Update () {

			// Check the effect queue for new effects to do
			lock (_queueLock) {
				while (EffectQueue.Count > 0)
					localEffectsCopy.Enqueue(EffectQueue.Dequeue());
			}
			
			// Move the application of the effects outside of the lock to not hog the lock and cause potential problems
			// Do this via an extra queue to preserve order and avoid repeated re-entry to the lock
			while (localEffectsCopy.Count > 0) {
                SelectiveDebug.LogEffect(localEffectsCopy.Peek().GetType().Name);
                localEffectsCopy.Dequeue().Apply();
			}

		}
		
		// Effect management
		
		// Queues and counters for use in thread safe regions of code
		private Queue<IVisualEffect> EffectQueue = new Queue<IVisualEffect>();

#endif

        public void EnqueueEffect_TS(IVisualEffect newEffect) {

#if USE_EFFECTS

            lock (_queueLock) {
				EffectQueue.Enqueue(newEffect);
            }
#endif

        }

    }

}
