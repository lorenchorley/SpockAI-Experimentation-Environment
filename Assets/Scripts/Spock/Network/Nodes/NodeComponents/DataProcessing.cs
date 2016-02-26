using UnityEngine;
using System.Collections;

namespace Spock {

    public abstract class DataProcessing : NodeComponent {

        public abstract Signal ProcessData(Signal signal);

    }

}