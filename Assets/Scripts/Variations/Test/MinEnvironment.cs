using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Spock {

    public class MinEnvironment : Environment {

        public MinEnvironment(SpockInstance S) : base(S) {}

        public override void AcceptSignal(Signal signal, int channel) {
            Debug.LogWarning("MinEnvironment received: " + signal + " on channel " + channel);
        }

        public override void Start() {
            MinController.I.SendSignalButton.onClick.AddListener(SendSingleSignal);
            MinController.I.SendBurstButton.onClick.AddListener(SendBurstSignal);
        }

        public void SendSingleSignal() {
            CIs[0].AcceptSignal(new Signal(S, null, S.SignalComponentTemplatesByID["MinTemplate"]), null);
        }

        public void SendBurstSignal() {
            int count;
            int variation;
            if (int.TryParse(MinController.I.BurstCount.text, out count) && count >= 0) {

                if (int.TryParse(MinController.I.ContentVariation.text, out variation) && variation >= 0) {

                    for (int i = 0; i < count; i++) {
                        int content = Random.Range(0, variation) + 1;
                        Signal signal = S.NetworksByID["N"].NewSignal();
                        signal.SC.SetContent(content);
                        CIs[0].AcceptSignal(signal, null);
                    }

                } else {
                    MinController.I.ContentVariation.text = "";
                }
                
            } else {
                MinController.I.BurstCount.text = "";
            }
        }

    }

}