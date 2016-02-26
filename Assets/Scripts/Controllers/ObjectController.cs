using UnityEngine;
using System.Collections;
using Spock;
using System;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour, IController {

    public static ObjectController I;

    [NonSerialized]
    public SpockController Spock;

    public SpockInstance S;

    [NonSerialized]
    public GameObject SignalContainer;
    [NonSerialized]
    public GameObject EnvironmentContainer;

    public List<GameObject> EnvironmentUIs;

    public Vector3 NetworkOrigin;
    public Vector3 NetworkCIOffsets;
    public Vector3 EnvironmentOrigin;
    public Vector3 EnvironmentCIOffsets;

    public GameObject NodeTemplate;
    public LineRenderer ConnectionTemplate;
    public Material ConnectionLineMaterialBase;
    public GameObject SignalTemplate;
    public GameObject NetworkTemplate;
    public GameObject EnvironmentTemplate;
    public GameObject INITemplate;

    public Dictionary<Spock.Network, NetworkRepresentation> networks;

    public ObjectController() {
        networks = new Dictionary<Spock.Network, NetworkRepresentation>();
    }

    public void SetSpockInstance(SpockInstance S) {
        this.S = S;
    }

    public void SetEnabled(bool enabled) {
        this.enabled = enabled;
    }

    public void Preinit() {
        ObjectController.I = this;
        Spock = GetComponent<SpockController>();
        SignalContainer = new GameObject("Signals");
        EnvironmentContainer = new GameObject("Environments");

        foreach (GameObject obj in I.EnvironmentUIs) {
            obj.SetActive(false);
        }

    }

    public void Init() {

    }

    public static void SelectUI() {
        foreach (GameObject obj in I.EnvironmentUIs) {
            obj.SetActive(obj.name == SettingsController.I.environmentType.ToString() + "UI");
        }
    }

}
