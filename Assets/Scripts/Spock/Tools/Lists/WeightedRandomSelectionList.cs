using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spock {

	public class WeightedRandomSelectionList<T> : Dictionary<T, float> {

        private float totalWeight = 0;

        public new void Add(T key, float value) {
            Assert.True("Weight is greater than zero", value > 0);
            totalWeight += value;
            base.Add(key, value);
        }

        public new void Remove(T key) {
            totalWeight -= base[key];
            base.Remove(key);
        }

        public new float this[T key] {
            get {
                return base[key];
            }
            set {
                Assert.True("Weight is greater than zero", value > 0);
                totalWeight += value - base[key];
                base[key] = value;
            }
        }

        public T WeightedRandomValue() {

            if (Count == 0)
                return default(T);

            float selectedValue = (float) new System.Random().NextDouble() * totalWeight;
            float runningTotal = 0;

            Assert.WithinBounds("The selected value is valid", 0, selectedValue, totalWeight);

            foreach (T item in Keys) {
                runningTotal += this[item];

                if (totalWeight <= runningTotal)
                    return item;

            }

            throw new Exception("No selection made");
        }

    }

}