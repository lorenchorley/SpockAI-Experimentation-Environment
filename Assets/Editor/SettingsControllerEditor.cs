using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingsController)), CanEditMultipleObjects]
public class SettingsControllerEditor : Editor {

    // Global properties
    public SerializedProperty 
        EnvironmentType,
        NetworkMode;

    // Testing properties
    public SerializedProperty
        NodeCount,
        NodesInSeries,
        NodesInParallel;

    // WNA properties
    public SerializedProperty
        SignalsPerInterval,
        SignalsPerIntervalVariation,
        IntervalLengthMilliseconds,
        NetworkExitInterfaces,
        InitialWeight,
        MinimumWeight,
        PositiveReward,
        NegativeReward,
        RewardMethod,
        InterfaceToReward,
        InterfaceHashPreference,
        SortingArraySize;

    void OnEnable() {
        // Setup the SerializedProperties
        // Global
        EnvironmentType = serializedObject.FindProperty("environmentType");
        NetworkMode = serializedObject.FindProperty("networkMode");

        // Testing
        NodeCount = serializedObject.FindProperty("NodeCount");
        NodesInSeries = serializedObject.FindProperty("NodesInSeries");
        NodesInParallel = serializedObject.FindProperty("NodesInParallel");

        // WNA
        SignalsPerInterval = serializedObject.FindProperty("SignalsPerInterval");
        SignalsPerIntervalVariation = serializedObject.FindProperty("SignalsPerIntervalVariation");
        IntervalLengthMilliseconds = serializedObject.FindProperty("IntervalLengthMilliseconds");
        NetworkExitInterfaces = serializedObject.FindProperty("NetworkExitInterfaces");

        InitialWeight = serializedObject.FindProperty("InitialWeight");
        MinimumWeight = serializedObject.FindProperty("MinimumWeight");
        PositiveReward = serializedObject.FindProperty("PositiveReward");
        NegativeReward = serializedObject.FindProperty("NegativeReward");
        RewardMethod = serializedObject.FindProperty("rewardMethod");
        InterfaceToReward = serializedObject.FindProperty("InterfaceToReward");
        InterfaceHashPreference = serializedObject.FindProperty("InterfaceHashPreference");
        SortingArraySize = serializedObject.FindProperty("ArraySize");

    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(EnvironmentType, new GUIContent());
        SettingsController.EnvironmentType envMode = (SettingsController.EnvironmentType) EnvironmentType.enumValueIndex;

        switch (envMode) {
        case SettingsController.EnvironmentType.Simple:

            WriteNetworkMode();

            break;
        case SettingsController.EnvironmentType.WeightedNetworkAdaption:

            EditorGUILayout.Separator();

            WriteNetworkMode();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Signal settings", EditorStyles.helpBox);
            EditorGUILayout.IntSlider(SignalsPerInterval, 0, 100);
            EditorGUILayout.IntSlider(SignalsPerIntervalVariation, 0, 100);
            EditorGUILayout.IntSlider(IntervalLengthMilliseconds, 1, 10000);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Interface settings", EditorStyles.helpBox);
            EditorGUILayout.IntSlider(NetworkExitInterfaces, 1, 10);
            
            EditorGUILayout.LabelField("Reward settings", EditorStyles.helpBox);
            InitialWeight.floatValue = EditorGUILayout.FloatField("Initial Weight", InitialWeight.floatValue);
            MinimumWeight.floatValue = EditorGUILayout.FloatField("Minimum Weight", MinimumWeight.floatValue);
            PositiveReward.floatValue = EditorGUILayout.FloatField("Positive Reward", PositiveReward.floatValue);
            NegativeReward.floatValue = EditorGUILayout.FloatField("Negative Reward", NegativeReward.floatValue);
            EditorGUILayout.PropertyField(RewardMethod);
            SettingsController.RewardMethod rewardMethod = (SettingsController.RewardMethod) RewardMethod.enumValueIndex;

            switch (rewardMethod) {
            case SettingsController.RewardMethod.Interface:
                EditorGUILayout.IntSlider(InterfaceToReward, 0, NetworkExitInterfaces.intValue);
                break;
            case SettingsController.RewardMethod.Hash:

                InterfaceHashPreference.arraySize = NetworkExitInterfaces.intValue;
                Show(InterfaceHashPreference, "Interface {0} hash", false);
                
                break;
            case SettingsController.RewardMethod.Sorting:

                SortingArraySize.intValue = EditorGUILayout.IntField("Sorting Array Size", SortingArraySize.intValue);

                /*A possible intellectual reasoning of this and yet another little theory could be the following.

<unfinished>
Pathways require a sort of reinforcement
Reinforcement is applied while the pathway is active to keep the pathway strong and desirable
Without reinforcement and with activity the pathway diminishes in desirability, until the pathway lacks any desirability at all. After that the existence of the pathway is unimportant since it will never be used due to it�s lack of desirability (note not undesirability)
Could formally model in neural network to test this theory
</unfinished>*/

                break;
            }

            EditorGUILayout.Separator();

            break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void WriteNetworkMode() {

        EditorGUILayout.LabelField("Network settings", EditorStyles.helpBox);
        EditorGUILayout.PropertyField(NetworkMode);
        SettingsController.NetworkMode mode = (SettingsController.NetworkMode) NetworkMode.enumValueIndex;

        switch (mode) {

        case SettingsController.NetworkMode.SingleNode:
            break;

        case SettingsController.NetworkMode.ParallelNodes:
            EditorGUILayout.IntSlider(NodeCount, 1, 100);

            break;

        case SettingsController.NetworkMode.SeriesNodes:
            EditorGUILayout.IntSlider(NodeCount, 1, 100);

            break;

        case SettingsController.NetworkMode.FeedForward:
            EditorGUILayout.IntSlider(NodesInSeries, 2, 20);
            EditorGUILayout.IntSlider(NodesInParallel, 1, 20);

            break;

        case SettingsController.NetworkMode.Custom:

            break;

        }

    }

    public static void Show(SerializedProperty list, string label, bool showListSize = true) {
        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel += 1;
        if (list.isExpanded) {
            if (showListSize) {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            }
            for (int i = 0; i < list.arraySize; i++) {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Format(label, i + 1)));
            }
        }
        EditorGUI.indentLevel -= 1;
    }

}
 