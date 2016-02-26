using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spock {

	public class RandomSelectionList<C> : List<C> {

        public C SelectRandom() {
            return this[new System.Random().Next(Count - 1)];
        }

        public C SelectAndRemoveRandom() {
            int index = new System.Random().Next(Count - 1);
            C value = this[index];
            RemoveAt(index);
            return value;
        }

    }

}