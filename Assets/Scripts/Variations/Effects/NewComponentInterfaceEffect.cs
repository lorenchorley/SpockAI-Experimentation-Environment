using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewComponentInterfaceEffect : INewComponentInterfaceEffect {

        private ComponentInterface ci;

        public NewComponentInterfaceEffect(ComponentInterface ci) {
            this.ci = ci;
        }

        public override void Apply() {
            RepresentationFunctions.CreateRepresentation(ci);
        }

    }

}
