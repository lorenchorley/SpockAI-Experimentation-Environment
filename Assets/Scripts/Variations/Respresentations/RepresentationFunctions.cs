using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class RepresentationFunctions {

        public static float distance = 0f;
        public static float linearDrag = 10f;
        public static float angularDrag = 1f;

        public static void PlaceBetween(Transform obj, IEnumerable<ISpockComponent> objs) {
            obj.position = GetPositionBetween(objs);
        }

        public static void PlaceBetween(Transform obj, params Vector3[] points) {
            obj.position = GetPositionBetween(points);
        }

        public static Vector3 GetPositionBetween(IEnumerable<ISpockComponent> objs) {

            // Place the connection center at the average of all node positions
            Vector3 average = Vector3.zero;
            int length = 0;
            foreach (ISpockComponent component in objs) {
                average += component.GetRepresentation().GetGameObject().transform.position;
                length++;
            }

            return average * (1f / length);
        }

        public static Vector3 GetPositionBetween(params Vector3[] points) {

            // Place the connection center at the average of all node positions
            Vector3 average = Vector3.zero;
            int length = 0;
            foreach (Vector3 point in points) {
                average += point;
                length++;
            }

            return average * (1f / length);
        }
        
        public static void AddSpringConnection(GameObject Start, GameObject End) {

            ChargedParticle cpStart = Start.GetComponent<ChargedParticle>();
            ChargedParticle cpEnd = End.GetComponent<ChargedParticle>();

            if (cpStart == null)
                cpStart = Start.AddComponent<ChargedParticle>();

            if (cpEnd == null)
                cpEnd = End.AddComponent<ChargedParticle>();

            Rigidbody cpStartRB = cpStart.GetComponent<Rigidbody>();
            Rigidbody cpEndRB = cpEnd.GetComponent<Rigidbody>();

            cpStartRB.drag = linearDrag;
            cpEndRB.drag = linearDrag;

            cpStartRB.angularDrag = angularDrag;
            cpEndRB.angularDrag = angularDrag;

            cpStart.LinkChargedParticle(cpEnd);
            
        }
        
        public static void RemoveSpringConnection(GameObject Start, GameObject End) {
            ChargedParticle cpStart = Start.GetComponent<ChargedParticle>();
            ChargedParticle cpEnd = End.GetComponent<ChargedParticle>();
            cpStart.UnlinkChargedParticle(cpEnd);
        }

        public static bool HasSpringConnection(GameObject Start, GameObject End) {
            ChargedParticle cpStart = Start.GetComponent<ChargedParticle>();

            if (cpStart == null)
                return false;

            ChargedParticle cpEnd = End.GetComponent<ChargedParticle>();

            if (cpEnd == null)
                return false;

            return cpStart.LinkedParticles.Contains(cpEnd);
        }

        public static C CreateRepresentation<C>(C obj) where C : ISpockComponent {
            GameObject newObject;

            if (obj is Node) {

                Node node = obj as Node;
                newObject = GameObject.Instantiate<GameObject>(ObjectController.I.NodeTemplate);
                node.SetRepresentation(newObject.AddComponent<NodeRepresentation>());
                node.GetRepresentation<NodeRepresentation>().Node = node;
                newObject.name = obj.GetType().Name + " " + node.ID.id;
                newObject.transform.SetParent(ObjectController.I.networks[node.N].transform);

            } else if (obj is Connection) {

                Connection connection = obj as Connection;
                newObject = new GameObject(obj.GetType().Name + " " + connection.ID.id);
                connection.SetRepresentation(newObject.AddComponent<ConnectionRepresentation>());
                connection.GetRepresentation<ConnectionRepresentation>().Connection = connection;
                Assert.True("Network exists", ObjectController.I.networks.ContainsKey(connection.N));
                newObject.transform.SetParent(ObjectController.I.networks[connection.N].transform);

            } else if (obj is Signal) {

                Signal signal = obj as Signal;
                newObject = GameObject.Instantiate<GameObject>(ObjectController.I.SignalTemplate);
                signal.SetRepresentation(newObject.AddComponent<SignalRepresentation>());
                signal.GetRepresentation<SignalRepresentation>().Signal = signal;
                newObject.transform.SetParent(ObjectController.I.SignalContainer.transform);
                newObject.name = obj.GetType().Name + " " + signal.GetID();

            } else if (obj is Spock.Network) {

                Spock.Network network = obj as Spock.Network;
                newObject = GameObject.Instantiate<GameObject>(ObjectController.I.NetworkTemplate);
                network.SetRepresentation(newObject.AddComponent<NetworkRepresentation>());
                network.GetRepresentation<NetworkRepresentation>().Network = network;
                ObjectController.I.networks.Add(network, network.GetRepresentation<NetworkRepresentation>());
                newObject.name = "Network: " + network.name;

            } else if (obj is Spock.Environment) {

                Spock.Environment environment = obj as Spock.Environment;
                newObject = GameObject.Instantiate<GameObject>(ObjectController.I.EnvironmentTemplate);
                environment.SetRepresentation(newObject.AddComponent<EnvironmentRepresentation>());
                environment.GetRepresentation<EnvironmentRepresentation>().Environment = environment;
                newObject.transform.SetParent(ObjectController.I.EnvironmentContainer.transform);
                newObject.name = "Environment: " + obj.GetType().Name;

            } else if (obj is ComponentInterface) {

                ComponentInterface ini = obj as ComponentInterface;
                newObject = GameObject.Instantiate<GameObject>(ObjectController.I.INITemplate);
                ini.SetRepresentation(newObject.AddComponent<ComponentInterfaceRepresentation>());
                ini.GetRepresentation<ComponentInterfaceRepresentation>().CI = ini;
                newObject.name = "Interface: " + obj.GetType().Name;

            }

            return obj;
        }

    }

}
