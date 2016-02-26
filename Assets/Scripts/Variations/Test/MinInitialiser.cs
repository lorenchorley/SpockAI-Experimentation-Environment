using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class MinInitialiser : SpockInitialiser {

        private Environment E;
        private Network N;

        private NodeComponentTemplate nodeTemplate;
        private ConnectionComponentTemplate connectionTemplate;
        private SignalComponentTemplate signalTemplate;

        private NetworkEntryCI Nin;
        private NetworkExitCI Nout;
        private EnvironmentEntryCI Ein;
        private EnvironmentExitCI Eout;

        private NetworkFunctions.SingleNodeNetworkProfile SingleNodeNetworkProfile;

        public MinInitialiser(Dictionary<Type, IController> Controllers) : base (Controllers) { }

        protected override void Start() {
            
        }

        protected override IEffectFactory SetupEffectFactory() {
            return new EffectFactory();
        }
        
        protected override void SetupEnvironments() {
            E = S.RegisterEnvironment("MinEnvironment", SettingsController.NewEnvironment(S));
        }

        protected override void SetupComponentTemplates() {

            nodeTemplate = S.RegisterNodeComponentTemplate("MinTemplate", new NodeComponentTemplate().SetComponents(
                typeof(AlwaysActivatedAC),
                typeof(RandomTS),
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

            connectionTemplate = S.RegisterConnectionComponentTemplate("MinTemplate", new ConnectionComponentTemplate().SetComponents(
                typeof(MinLS),
                typeof(AllToAllSR),
                typeof(MinCS)
                ).Finalise());

            signalTemplate = S.RegisterSignalComponentTemplate("MinTemplate", new SignalComponentTemplate().SetComponents(
                typeof(MinSC),
                typeof(OneBacklinkCR)
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
            
            // Network interfaces
            Nin = (NetworkEntryCI) S.RegisterComponentInterface("Nin", new NetworkEntryCI(S, N));

            Nout = (NetworkExitCI) S.RegisterComponentInterface("Nout", new NetworkExitCI(S, N));

            // Environment interfaces
            Ein = (EnvironmentEntryCI) S.RegisterComponentInterface("Ein", new EnvironmentEntryCI(S, E));

            Eout = (EnvironmentExitCI) S.RegisterComponentInterface("Eout", new EnvironmentExitCI(S, E));

            // Link interfaces
            Nout.ConnectToEntryComponentInterface(Ein);
            Eout.ConnectToEntryComponentInterface(Nin);

            // Register the environment with the ingoing interface
            Ein.SetEnvironmentChannel(E, 0);

            // Register the environment outgoing interface with the environment
            E.SetOutgoingInterface(Eout, 0);

        }

        protected override void Finish() {

            SingleNodeNetworkProfile = NetworkFunctions.SetupSingleNodeNetwork(N, Nin, Nout);

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
            Nout.GetRepresentation<ComponentInterfaceRepresentation>().SetColour(Color.blue);
            Ein.GetRepresentation<ComponentInterfaceRepresentation>().SetColour(Color.blue);
            Eout.GetRepresentation<ComponentInterfaceRepresentation>().SetColour(Color.green);
            Nin.GetRepresentation<ComponentInterfaceRepresentation>().SetColour(Color.green);

        }

        public void ReallyFinished() {
            S.Get<EffectController>().EnqueueEffect_TS(new CallbackEffect(() => ObjectController.SelectUI()));
        }

    }

}
