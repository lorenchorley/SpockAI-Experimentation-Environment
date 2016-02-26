using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spock {

    public class HashsetWeightedRandomSelectionList<T> {

        private int numberOfBins;
        private float initialValue;
        private float minimumValue;
        private float[] totalWeights;
        private Func<Object, int> GetHashOf;

        private Dictionary<T, float[]> dict;

        public HashsetWeightedRandomSelectionList(int numberOfBins, float initialValue, float minimumValue, Func<Object, int> GetHashOf) {
            this.numberOfBins = numberOfBins;
            this.initialValue = initialValue;
            this.minimumValue = minimumValue;
            this.GetHashOf = GetHashOf;

            dict = new Dictionary<T, float[]>();

            // Set initial weight totals
            totalWeights = new float[numberOfBins];
            for (int i = 0; i < numberOfBins; i++)
                totalWeights[i] = 0;

        }

        public void Add(T item) {

            // Get the hash for this key object
            //int keyHash = GetHashOf(item);

            // Create a new list of weights and populate it appropriately
            float[] newWeightList = new float[numberOfBins];
            for (int i = 0; i < numberOfBins; i++) {
                //if (i == keyHash) {
                //    newWeightList[keyHash] = value;

                //    // Update the total weight entry for this hash
                //    totalWeights[keyHash] += value;

                //} else {

                    newWeightList[i] = initialValue;

                    // Update the total weight entry for this hash
                    totalWeights[i] += initialValue;

                //}
            }

            // Add the key and the new list to the dictionary
            dict.Add(item, newWeightList);

        }

        public void Remove(T item) {

            // Remove the weights of this key from the total weights
            float[] weights = dict[item];
            for (int i = 0; i < numberOfBins; i++)
                totalWeights[i] -= weights[i];

            // Remove the key and its weights array
            dict.Remove(item);

        }

        public bool Contains(T item) {
            return dict.ContainsKey(item);
        }

        public IEnumerable<T> Items {
            get {
                return dict.Keys;
            }
        }

        public float GetWeight(T item, object obj) {
            return dict[item][GetHashOf(obj)];
        }

        public float GetWeight(T item, int hash) {
            return dict[item][hash];
        }

        public void AddWeight(T item, object obj, float value) {
            AddWeight(item, GetHashOf(obj), value);
        }

        public void AddWeight(T item, int hash, float value) {
            //Assert.True("Weight is greater than zero", value > 0);

            float weight = dict[item][hash];
            float change = value;

            if (weight + change < minimumValue) {

                // Update the total weight with the difference of the old weight and the new weight
                totalWeights[hash] += minimumValue - weight;
                Assert.True("totalWeights[hash] > 0", totalWeights[hash] > 0);

                // Update the weight
                dict[item][hash] = minimumValue;

            } else {

                // Update the total weight with the difference of the old weight and the new weight
                totalWeights[hash] += value;

                // Update the weight
                dict[item][hash] += change;

            }

        }

        public void SetWeight(T item, object obj, float value) {
            SetWeight(item, GetHashOf(obj), value);
        }

        public void SetWeight(T item, int hash, float value) {
            Assert.True("Weight is greater than or equal to the minimum value", value >= minimumValue);

            // Update the total weight with the difference of the old weight and the new weight
            float[] weights = dict[item];
            totalWeights[hash] += value - weights[hash];

            // Update the weight
            weights[hash] = value;

        }

        public T WeightedRandomValue(object obj) {
            return WeightedRandomValue(GetHashOf(obj));
        }

        public T WeightedRandomValue(int hash) {

            if (dict.Count == 0) {
                return default(T);
            }

            float totalWeight = totalWeights[hash];
            float selectedValue = (float) new System.Random().NextDouble() * totalWeight;
            float runningTotal = 0;

            Assert.WithinBounds("The selected value is valid", 0, selectedValue, totalWeight);

            foreach (T item in dict.Keys) {
                runningTotal += dict[item][hash];

                if (selectedValue <= runningTotal) {
                    return item;
                }

            }

            throw new Exception("No selection made");
        }
        
    }

    public class HashsetWeightedRandomSelectionListTest : UUnitTestCase {
        
        int HashBins = 3;
        float initialValue = 1;
        float minimumValue = 0;
        HashsetWeightedRandomSelectionList<string> list;

        public HashsetWeightedRandomSelectionListTest() {
            list = new HashsetWeightedRandomSelectionList<string>(HashBins, initialValue, minimumValue, item => {
                UUnitAssert.NotNull(item);
                int hashcode = item.GetHashCode();
                if (hashcode >= 0)
                    return item.GetHashCode() % HashBins;
                else
                    return -item.GetHashCode() % HashBins;
            });

            list.Add("First");
            list.Add("Second");
            list.Add("Third");

        }

        [UUnitTest]
        public void AddRetrieveAndRemoveTest() {
            
            list.AddWeight("First", 0, 2);
            list.AddWeight("Second", 0, -1);
            list.SetWeight("Third", 0, 0);

            UUnitAssert.True(list.GetWeight("First", 0) == 3);
            UUnitAssert.True(list.GetWeight("Second", 0) == 0);
            UUnitAssert.True(list.GetWeight("Third", 0) == 0);

        }

        [UUnitTest]
        public void ReturnRandomTest() {

            list.SetWeight("First", 1, 0);
            list.SetWeight("Second", 1, 0);
            list.SetWeight("Third", 1, 3);

            list.SetWeight("First", 2, 0);
            list.SetWeight("Second", 2, 4);
            list.SetWeight("Third", 2, 0);

            UUnitAssert.True(list.WeightedRandomValue(1) == "Third");
            UUnitAssert.True(list.WeightedRandomValue(2) == "Second");

        }

    }

}