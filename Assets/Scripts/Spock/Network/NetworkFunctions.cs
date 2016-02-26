using UnityEngine;
using System.Collections.Generic;
using System;

namespace Spock {

    public class NetworkFunctions {

        public class SingleNodeNetworkProfile {
            public Connection InConnection, OutConnection;
            public Node Node;
        }

        public class ParallelDuplicationProfile {
            public Node OriginalNode, NewNode;
        }

        public class SeriesDuplicationProfile {
            public Connection InConnection, MiddleConnection, OutConnection;
            public Node OriginalFirstNode, NewSecondNode;
        }

        public static SingleNodeNetworkProfile SetupSingleNodeNetwork(Network N, NetworkEntryCI NetworkCIin, NetworkExitCI NetworkCIout) {
            SingleNodeNetworkProfile profile = new SingleNodeNetworkProfile();

            // Create the in and out bound connections
            profile.InConnection = N.NewConnection();
            profile.OutConnection = N.NewConnection();

            // Setup the only node
            profile.Node = N.NewNode();

            // Connect the node in the middle of the two connections to create a complete path
            profile.Node.AddInputs(profile.InConnection);
            profile.Node.AddOutputs(profile.OutConnection);

            NetworkCIin.SetNetworkConnection(profile.InConnection);

            // Register the network outgoing interface with the out connection
            profile.OutConnection.AddOutputs(NetworkCIout);

            return profile;
        }

        public static void DuplicateNodeInParallel(Node node, Action completionCallback = null) {
            ParallelDuplicationProfile profile = new ParallelDuplicationProfile();

            profile.OriginalNode = node;
            profile.NewNode = new Node(node, completionCallback);

        }

        public static ParallelDuplicationProfile DuplicateNodeInParallelAndReturnProfile(Node node, Action<ParallelDuplicationProfile> completionCallback) {
            ParallelDuplicationProfile profile = new ParallelDuplicationProfile();

            profile.OriginalNode = node;
            profile.NewNode = new Node(node, delegate {
                
                if (completionCallback != null)
                    completionCallback.Invoke(profile);

            });
            
            return profile;
        }

        public static void DuplicateNodeInParallel(Node node, int times, Action completionCallback = null) {
            int count = times;

            Action duplicate = null;
            duplicate = delegate {
                if (count-- == 1) {
                    if (completionCallback != null)
                        completionCallback.Invoke();
                    return;
                }
                NetworkFunctions.DuplicateNodeInParallel(node, duplicate);
            };

            duplicate();
        }

        public static void DuplicateNodeInSeries(Node node, Action completionCallback = null) {
            SeriesDuplicationProfile profile = new SeriesDuplicationProfile();

            profile.OriginalFirstNode = node;
            profile.MiddleConnection = node.N.NewConnection();
            profile.NewSecondNode = new Node(node, delegate {

                // Remove the outputs of the original node
                profile.OriginalFirstNode.SP.EraseAllOutputs();

                // Remove the inputs of the new node
                profile.NewSecondNode.SP.EraseAllInputs();

                // Add the middle connection inbetween the two nodes
                profile.MiddleConnection.AddInputs(profile.OriginalFirstNode);
                profile.MiddleConnection.AddOutputs(profile.NewSecondNode);
                
                if (completionCallback != null)
                    completionCallback.Invoke();

            });

        }

        public static void DuplicateNodeInSeriesAndReturnProfile(Node node, Action<SeriesDuplicationProfile> completionCallback) {
            SeriesDuplicationProfile profile = new SeriesDuplicationProfile();

            profile.OriginalFirstNode = node;
            profile.MiddleConnection = node.N.NewConnection();
            profile.NewSecondNode = new Node(node, delegate {

                // Remove the outputs of the original node
                profile.OriginalFirstNode.SP.EraseAllOutputs();

                // Remove the inputs of the new node
                profile.NewSecondNode.SP.EraseAllInputs();

                // Add the middle connection inbetween the two nodes
                profile.MiddleConnection.AddInputs(profile.OriginalFirstNode);
                profile.MiddleConnection.AddOutputs(profile.NewSecondNode);

                if (completionCallback != null)
                    completionCallback.Invoke(profile);

            });

        }

        public static void DuplicateNodeInSeries(Node node, int times, Action completionCallback = null) {
            int count = times;

            Action duplicate = null;
            duplicate = delegate {
                if (count-- == 1) {
                    if (completionCallback != null)
                        completionCallback.Invoke();
                    return;
                }
                NetworkFunctions.DuplicateNodeInSeries(node, duplicate);
            };

            duplicate();
        }

        // TODO decide whether this should stay as is or be converted into a network component style module
        public static void RewardNetwork(Network N, float level, int type) {
            DateTime now = DateTime.Now;
            foreach (Node node in N.Nodes.Values) {
                node.Get<MetricManagement>().SetReinforcement(level, type, now);
            }
        }

        public static void DuplicateNodeInFeedForwardNetwork(Node startingNode, int seriesCount, int parallelCount, Action completionCallback = null) {
            
            Action<Node> duplicateInSeries = null;
            Action<Node, Node> duplicateInParallel = null;
            Action<Node> simpleDuplicateInParallel = null;

            duplicateInSeries = delegate (Node node) {
                parallelCount = SettingsController.I.NodesInParallel;
                NetworkFunctions.DuplicateNodeInSeriesAndReturnProfile(node, (NetworkFunctions.SeriesDuplicationProfile profile) => duplicateInParallel(profile.OriginalFirstNode, profile.NewSecondNode));
            };

            duplicateInParallel = delegate (Node first, Node second) {
                if (parallelCount == 1) {

                    if (seriesCount-- == 2) {
                        // Duplicate node in the final layer
                        parallelCount = SettingsController.I.NodesInParallel;
                        simpleDuplicateInParallel(second);
                        return;
                    }

                    // Start next layer
                    duplicateInSeries(second);

                } else {
                    // Continue duplication of nodes in current layer
                    parallelCount--;
                    NetworkFunctions.DuplicateNodeInParallel(first, () => duplicateInParallel(first, second));
                }
            };

            simpleDuplicateInParallel = delegate (Node node) {
                if (parallelCount == 1) {
                    if (completionCallback != null)
                        completionCallback.Invoke();
                } else {
                    parallelCount--;
                    NetworkFunctions.DuplicateNodeInParallel(node, () => simpleDuplicateInParallel(node));
                }
            };

            duplicateInSeries(startingNode);
        }

    }

}