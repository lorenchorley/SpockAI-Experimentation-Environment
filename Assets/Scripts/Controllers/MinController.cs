using UnityEngine;
using System.Collections;
using Spock;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class MinController : MonoBehaviour, IController {

    public static MinController I;
    
    public SpockInstance S;
    
    public Button SendSignalButton;

    public Button SendBurstButton;
    public InputField BurstCount;
    public InputField ContentVariation;

    public void SetSpockInstance(SpockInstance S) {
        this.S = S;
    }

    public void SetEnabled(bool enabled) {
        this.enabled = enabled;
    }

    public void Preinit() {
        MinController.I = this;
    }

    public void Init() {

    }
    
}
