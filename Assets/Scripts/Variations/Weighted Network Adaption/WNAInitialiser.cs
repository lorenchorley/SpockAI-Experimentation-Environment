using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class WNAInitialiser : SpockInitialiser {

        private Environment E;
        private Network N;

        private NodeComponentTemplate nodeTemplate;
        private ConnectionComponentTemplate connectionTemplate;
        private SignalComponentTemplate signalTemplate;

        private EnvironmentExitCI Eout;
        private NetworkEntryCI Nin;

        private List<NetworkExitCI> Nouts;
        private List<EnvironmentEntryCI> Eins;

        private NetworkFunctions.SingleNodeNetworkProfile SingleNodeNetworkProfile;

        public WNAInitialiser(Dictionary<Type, IController> Controllers) : base (Controllers) { }

        protected override void Start() {

        }

        protected override IEffectFactory SetupEffectFactory() {
            return new EffectFactory();
        }
        
        protected override void SetupEnvironments() {
            E = S.RegisterEnvironment("WNAInitialiser", SettingsController.NewEnvironment(S));
        }

        protected override void SetupComponentTemplates() {

            nodeTemplate = S.RegisterNodeComponentTemplate("WNATemplate", new NodeComponentTemplate().SetComponents(
                typeof(AlwaysActivatedAC),
                typeof(WeightedTS),
                typeof(PassThroughDP),
                typeof(NextSignalTC),
                typeof(MinLC),
                typeof(MinEM),
                typeof(MinMM),
                typeof(MinPM),
                typeof(MinIP),
                typeof(MinOP),
                typeof(MinSP)
                ).Finalise());

            connectionTemplate = S.RegisterConnectionComponentTemplate("WNATemplate", new ConnectionComponentTemplate().SetComponents(
                typeof(ContentWeightedLS),
                typeof(WeightedSR),
                typeof(WeightedCS)
                ).Finalise());

            signalTemplate = S.RegisterSignalComponentTemplate("WNATemplate", new SignalComponentTemplate().SetComponents(
                typeof(MinSC),
                typeof(MultipleBacklinkCR)
                ).Finalise());

        }

        protected override void SetupNetworks() {

            // Create the network
            N = S.NewNetwork("N");

            // Set the component templates
            N.RegisterNodeTemplate(nodeTemplate);
            N.RegisterConnectionTemplate(connectionTemplate);
            N.RegisterSignalTemplate(signalTemplate);

        }

        protected override void SetupComponentInterfaces() {

            // Environment to network interfaces
            Eout = (EnvironmentExitCI) S.RegisterComponentInterface("Eout", new EnvironmentExitCI(S, E));
            Nin = (NetworkEntryCI) S.RegisterComponentInterface("Nin", new NetworkEntryCI(S, N));

            // Network to environment interfaces
            Nouts = new List<NetworkExitCI>();
            Eins = new List<EnvironmentEntryCI>();
            for (int i = 0; i < SettingsController.I.NetworkExitInterfaces; i++) {
                NetworkExitCI Nout = (NetworkExitCI) S.RegisterComponentInterface("Nout " + i, new NetworkExitCI(S, N));
                EnvironmentEntryCI Ein = (EnvironmentEntryCI) S.RegisterComponentInterface("Ein " + i, new EnvironmentEntryCI(S, E));
                
                // Link interfaces
                Nout.ConnectToEntryComponentInterface(Ein);

                // Register the environment with the ingoing interface
                Ein.SetEnvironmentChannel(E, i);

                Nouts.Add(Nout);
                Eins.Add(Ein);

            }

            // Link interfaces
            Eout.ConnectToEntryComponentInterface(Nin);

            // Register the environment outgoing interface with the environment
            E.SetOutgoingInterface(Eout, 0);

        }

        protected override void Finish() {

            SingleNodeNetworkProfile = NetworkFunctions.SetupSingleNodeNetwork(N, Nin, Nouts[0]);
            for (int i = 1; i < SettingsController.I.NetworkExitInterfaces; i++) {
                Connection newConnection = new Connection(N, connectionTemplate);
                newConnection.AddInputs(SingleNodeNetworkProfile.Node);
                newConnection.AddOutputs(Nouts[i]);
            }


            if (SettingsController.I.networkMode == SettingsController.NetworkMode.ParallelNodes)
                NetworkFunctions.DuplicateNodeInParallel(SingleNodeNetworkProfile.Node, SettingsController.I.NodeCount, () => ReallyFinished());

            else if (SettingsController.I.networkMode == SettingsController.NetworkMode.SeriesNodes)
                NetworkFunctions.DuplicateNodeInSeries(SingleNodeNetworkProfile.Node, SettingsController.I.NodeCount, () => ReallyFinished());

            else if (SettingsController.I.networkMode == SettingsController.NetworkMode.FeedForward)
                NetworkFunctions.DuplicateNodeInFeedForwardNetwork(SingleNodeNetworkProfile.Node,
                                                                   SettingsController.I.NodesInSeries,
                                                                   SettingsController.I.NodesInParallel,
                                                                   () => ReallyFinished()
                                                                   );

            else if (SettingsController.I.networkMode == SettingsController.NetworkMode.Custom)
                NetworkFunctions.DuplicateNodeInParallel(SingleNodeNetworkProfile.Node, delegate {
                    NetworkFunctions.DuplicateNodeInSeries(SingleNodeNetworkProfile.Node);
                });

            else
                ReallyFinished();

            // Set the component interface colours so as to differentiate them visually
            for (int i = 0; i < SettingsController.I.NetworkExitInterfaces; i++) {
                Nouts[i].GetRepresentation<ComponentInterfaceRepresentation>().SetColour(Color.blue);
                Eins[i].GetRepresentation<ComponentInterfaceRepresentation>().SetColour(Color.blue);
            }
            Eout.GetRepresentation<ComponentInterfaceRepresentation>().SetColour(Color.green);
            Nin.GetRepresentation<ComponentInterfaceRepresentation>().SetColour(Color.green);

        }

        public void ReallyFinished() {
            S.Get<EffectController>().EnqueueEffect_TS(new CallbackEffect(() => ObjectController.SelectUI()));
        }

    }

}
