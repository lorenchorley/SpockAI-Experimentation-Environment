using UnityEngine;
using System.Collections;
using Spock;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SettingsController : MonoBehaviour, IController {

    public enum EnvironmentType {
        Simple,
        WeightedNetworkAdaption 
    }

    public enum NetworkMode {
        SingleNode,
        ParallelNodes,
        SeriesNodes,
        FeedForward,
        Custom
    }

    public enum RewardMethod {
        Interface,
        Hash,
        Sorting
    }
    
    public static SettingsController I;
    
    public SpockInstance S;

    private Dictionary<NetworkMode, SpockInitialiser> initialisers;

    public NetworkMode networkMode;
    public EnvironmentType environmentType;
    
    // Testing
    // =======

    // ParallelNodes and SeriesNodes
    public int NodeCount = 2;

    // FeedForward
    public int NodesInSeries = 2;
    public int NodesInParallel = 2;

    // Weighted Network Adaption
    // =========================

    // Signal settings
    public int SignalsPerInterval;
    public int SignalsPerIntervalVariation;
    public int IntervalLengthMilliseconds;

    // Interface settings
    public int NetworkExitInterfaces;

    // Reward settings
    public float InitialWeight;
    public float MinimumWeight;
    public float PositiveReward;
    public float NegativeReward;
    public RewardMethod rewardMethod;

    // By interface
    public int InterfaceToReward;

    // By hash
    public int[] InterfaceHashPreference;

    // By sorting
    public int ArraySize;

    public void SetSpockInstance(SpockInstance S) {
        this.S = S;
    }

    public void SetEnabled(bool enabled) {
        this.enabled = enabled;
    }

    public void Preinit() {
        SettingsController.I = this;
    }

    public void Init() {

    }

    public static SpockInitialiser NewInitialiser(Dictionary<Type, IController> Controllers) {
        switch (I.environmentType) {
        case EnvironmentType.Simple:
            return new MinInitialiser(Controllers);
        case EnvironmentType.WeightedNetworkAdaption:
            return new WNAInitialiser(Controllers);
        default:
            throw new NotImplementedException();
        }
    }

    public static Spock.Environment NewEnvironment(SpockInstance S) {
        switch (I.environmentType) {
        case EnvironmentType.Simple:
            return new MinEnvironment(S);
        case EnvironmentType.WeightedNetworkAdaption:
            return new WNAEnvironment(S);
        default:
            throw new NotImplementedException();
        }
    }
    

}
