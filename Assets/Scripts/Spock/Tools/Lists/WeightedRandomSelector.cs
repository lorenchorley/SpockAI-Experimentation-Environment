using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spock {

	public class WeightedRandomSelector<K> {

        private float totalWeight; // this stores sum of weights of all elements before current
        private K selected; // currently selected element
        private Random rnd;

        public WeightedRandomSelector() {
            rnd = new Random();
            Reset();
        }

        public void Reset() {
            totalWeight = 0;
            selected = default(K);
        }

        public void Add(K obj, float weight) {
            float r = (float) rnd.NextDouble() * (totalWeight + weight);
            if (r >= totalWeight) // probability of this is weight/(totalWeight+weight)
                selected = obj; // it is the probability of discarding last selected element and selecting current one instead
            totalWeight += weight; // increase weight sum
        }

        public K GetRandomSelection() {
            return selected;
        }
        
    }

}