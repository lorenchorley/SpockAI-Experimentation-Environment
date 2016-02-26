using UnityEngine;
using System.Collections;
using Spock;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class WNAController : MonoBehaviour, IController {

    public static WNAController I;
    
    public SpockInstance S;
    
    public Button StartSendingButton;
    public int HashBins;

    public void SetSpockInstance(SpockInstance S) {
        this.S = S;
    }

    public void SetEnabled(bool enabled) {
        this.enabled = enabled;
    }

    public void Preinit() {
        WNAController.I = this;
    }

    public void Init() {

    }

    public int GetHashOf(System.Object item) {
        Assert.NotNull("item", item);
        int hashcode = item.GetHashCode();
        if (hashcode >= 0)
            return item.GetHashCode() % HashBins;
        else
            return -item.GetHashCode() % HashBins;
    }

}
