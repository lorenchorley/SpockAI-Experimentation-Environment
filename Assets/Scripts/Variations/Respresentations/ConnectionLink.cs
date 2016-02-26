using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spock {

    public class ConnectionLink : MonoBehaviour {

        public float[] strength;

        public static ConnectionLink GetConnectionLink(GameObject obj) {
            ConnectionLink link = obj.GetComponent<ConnectionLink>();
            if (link == null) {
                link = obj.AddComponent<ConnectionLink>();
                link.strength = new float[WNAController.I.HashBins];
                for (int i = 0; i < WNAController.I.HashBins; i++)
                    link.strength[i] = 1;
            }
            return link;
        }

    }

}
