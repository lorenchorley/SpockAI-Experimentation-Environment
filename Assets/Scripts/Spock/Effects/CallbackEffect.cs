using System;
using UnityEngine;

namespace Spock {

	public class CallbackEffect : IVisualEffect {

		private Action callback;

		public CallbackEffect(Action callback) {
			this.callback = callback;
		}

		public void Apply() {
			callback.Invoke();
		}

	}

}
