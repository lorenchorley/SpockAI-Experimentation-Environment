using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewComponentInterfaceInputEffect : INewComponentInterfaceInputEffect {

        private ComponentInterface ci;
        private ISignalSending obj;

        public NewComponentInterfaceInputEffect(ComponentInterface ci, ISignalSending obj) {
            this.ci = ci;
            this.obj = obj;
        }

        public override void Apply() {
            if (obj is Connection) {
                ci.GetRepresentation<ComponentInterfaceRepresentation>().AddConnection(obj as Connection, false);
            }
        }

        }

}
