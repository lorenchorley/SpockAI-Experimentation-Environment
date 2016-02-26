using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewComponentInterfaceOutputEffect : INewComponentInterfaceOutputEffect {

        private ComponentInterface ci;
        private ISignalAccepting obj;

        public NewComponentInterfaceOutputEffect(ComponentInterface ci, ISignalAccepting obj) {
            this.ci = ci;
            this.obj = obj;
        }

        public override void Apply() {
            if (obj is Connection) {
                ci.GetRepresentation<ComponentInterfaceRepresentation>().AddConnection(obj as Connection, true);
            }
        }

    }

}
