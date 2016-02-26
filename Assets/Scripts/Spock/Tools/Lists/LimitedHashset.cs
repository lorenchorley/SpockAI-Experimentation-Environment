using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spock {

    // TODO remove if not needed
	public class LimitedHashset<T> {

        public int numberOfBins { get; private set; }
        private List<T>[] bins;

        public LimitedHashset(int numberOfBins) {
            Assert.True("numberOfBins is valid", numberOfBins > 0);
            this.numberOfBins = numberOfBins;
            bins = new List<T>[numberOfBins];
            for (int i = 0; i < numberOfBins; i++) {
                bins[i] = new List<T>();
            }
        }

        public void Add(T item) {
            bins[GetHashOf(item)].Add(item);
        }

        public void Remove(T item) {
            bins[GetHashOf(item)].Remove(item);
        }

        public bool Contains(T item) {
            return bins[GetHashOf(item)].Contains(item);
        }

        public IEnumerable<T> this[int hash] {
            get {
                return bins[hash];
            }
        }

        public IEnumerable<T> this[T item] {
            get {
                return bins[GetHashOf(item)];
            }
        }

        public int GetHashOf(Object item) {
            int hashcode = item.GetHashCode();
            if (hashcode >= 0)
                return item.GetHashCode() % numberOfBins;
            else
                return -item.GetHashCode() % numberOfBins;
        }

    }

}