using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class NewEnvironmentEffect : INewEnvironmentEffect {

        private Environment environment;

        public NewEnvironmentEffect(Environment environment) {
            this.environment = environment;
        }

        public override void Apply() {
            RepresentationFunctions.CreateRepresentation(environment);
        }

    }

}
