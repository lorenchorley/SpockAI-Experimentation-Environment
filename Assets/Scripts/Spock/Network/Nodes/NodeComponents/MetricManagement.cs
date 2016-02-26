using UnityEngine;
using System.Collections;
using System;

namespace Spock {

    public abstract class MetricManagement : NodeComponent {

        public abstract void SetReinforcement(float level, int type, DateTime time);

    }

}