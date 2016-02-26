using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

namespace Spock {

    public class WNAEnvironment : Environment {

        public Network N;

        private System.Random rnd;
        private int[] sortedArray;
        private Queue<int> numbersToSort;
        private bool StopSending = false;

        public WNAEnvironment(SpockInstance S) : base(S) {
            rnd = new System.Random();
            sortedArray = new int[SettingsController.I.ArraySize];
            numbersToSort = new Queue<int>();
            for (int i = 0; i < SettingsController.I.ArraySize; i++) {
                sortedArray[i] = -1;
                numbersToSort.Enqueue(i);
            }
        }

        public override void AcceptSignal(Signal signal, int channel) {
            Assert.True("Channel is valid", channel >= 0 && channel < SettingsController.I.NetworkExitInterfaces);
            //Debug.LogWarning("TextEnvironment received: " + signal + " on channel " + channel);

            // Reward the network if it route the signal to the right interface
            switch (SettingsController.I.rewardMethod) {
            case SettingsController.RewardMethod.Interface:

                if (channel == SettingsController.I.InterfaceToReward - 1) {
                    signal.CR.RewardForSignalPath(SettingsController.I.PositiveReward);
                } else {
                    signal.CR.RewardForSignalPath(SettingsController.I.NegativeReward);
                }

                break;
            case SettingsController.RewardMethod.Hash:

                if (WNAController.I.GetHashOf(signal.SC.GetContent()) == SettingsController.I.InterfaceHashPreference[channel]) {
                    Debug.Log("Interface " + (channel + 1) + " +vely rewarded signal with hash " + WNAController.I.GetHashOf(signal.SC.GetContent()));
                    signal.CR.RewardForSignalPath(SettingsController.I.PositiveReward);
                } else {
                    Debug.Log("Interface " + (channel + 1) + " -vely rewarded signal with hash " + WNAController.I.GetHashOf(signal.SC.GetContent()));
                    signal.CR.RewardForSignalPath(SettingsController.I.NegativeReward);
                }

                break;
            case SettingsController.RewardMethod.Sorting:

                float reward;

                if (sortedArray[channel] != -1) {
                    Debug.Log("Array index already used");
                    numbersToSort.Enqueue((int) signal.SC.GetContent());

                    // Negatively reward for a missed assignments
                    reward = SettingsController.I.NegativeReward;

                } else {
                    sortedArray[channel] = (int) signal.SC.GetContent();

                    // Reward depending on how close the signal was to the correct position
                    // The aim is to get the number x into slot x in the array, so this number will give us the closeness
                    float distance = Mathf.Abs(sortedArray[channel] - channel);

                    Debug.Log("distance: " + distance);

                    if (distance == 0) { // Full reward
                        reward = SettingsController.I.PositiveReward;
                    } else if (distance == 1) {
                        reward = SettingsController.I.PositiveReward / 2;
                    } else if (distance == 2) {
                        reward = SettingsController.I.PositiveReward / 4;
                    } else if (distance == 3) { // Negative reward, too far
                        reward = SettingsController.I.NegativeReward / 4;
                    } else if (distance == 4) {
                        reward = SettingsController.I.NegativeReward / 2;
                    } else {
                        reward = SettingsController.I.NegativeReward;
                    }
                    
                }

                signal.CR.RewardForSignalPath(reward);

                break;
            }

            // Remove the signal
            signal.DeleteSignal();

        }

        public override void Start() {
            N = S.NetworksByID["N"];

            WNAController.I.StartSendingButton.onClick.AddListener(() =>
                ThreadController.AddJobToQueue(JobFunctions.ProcessWakeAndInvoke, new object[] { (Action) SendSingleSignal, (Func<int>) CalculateTimeToNextSignalSending })
            );

            // Reward method specific UI
            switch (SettingsController.I.rewardMethod) {
            case SettingsController.RewardMethod.Interface:
                break;
            case SettingsController.RewardMethod.Hash:
                break;
            case SettingsController.RewardMethod.Sorting:



                break;
            }
        }

        public void SendSingleSignal() {

            bool stillGoing = false;
            for (int i = 0; i < sortedArray.Length; i++)
                if (sortedArray[i] == -1) {
                    stillGoing = true;
                    break;
                }

            if (!stillGoing && numbersToSort.Count == 0) {
                Debug.Log("Stopped");
                StopSending = true;
                FinishedSorting();
                return;
            }

            if (numbersToSort.Count > 0) {
                Signal newSignal = new Signal(S, null, S.SignalComponentTemplatesByID["WNATemplate"]);

                newSignal.SC.SetContent(numbersToSort.Dequeue());

                // Use the exit interface to send a new signal into the network. The interface will automatically start a new thread
                CIs[0].AcceptSignal(newSignal, null);
            }

        }
        
        private void FinishedSorting() {
            for (int i = 0; i < sortedArray.Length; i++) {

            }
        }

        public int CalculateTimeToNextSignalSending() {

            if (StopSending)
                return -1;

            int variation = rnd.Next(2 * SettingsController.I.SignalsPerIntervalVariation) - SettingsController.I.SignalsPerIntervalVariation;
            int time = SettingsController.I.IntervalLengthMilliseconds / SettingsController.I.SignalsPerInterval + variation;
            return time;
        }
        
    }

}