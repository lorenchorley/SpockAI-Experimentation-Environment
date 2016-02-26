using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Spock {

    public class SpockController : MonoBehaviour {

        public static SpockController I;

        public SpockInstance S;
        
        public Dictionary<Type, IController> Controllers;

        public SpockController() {
            SpockController.I = this;
        }

        void Awake() {

            // Gather all controllers together into a dictionary for easy access
            Controllers = new Dictionary<Type, IController>();
            foreach (IController controller in GetComponents<IController>()) {
                Controllers.Add(controller.GetType(), controller);
            }

            // Initialise the controllers
            MultiphaseInitialiser.Initialise(delegate (IController controller) {
                controller.SetEnabled(false);
                controller.SetSpockInstance(S);
            }, Controllers.Values);
            
            // Then start the initialiser
            UseInitialiser(SettingsController.NewInitialiser(Controllers));
            
            // Enable the controllers
            foreach (IController controller in Controllers.Values) {
                controller.SetEnabled(true);
            }

        }

        public void UseInitialiser(SpockInitialiser initialiser) {
            initialiser.Preinit();
            initialiser.Init();
            S = initialiser.GetInstance();
        }
        
    }

}