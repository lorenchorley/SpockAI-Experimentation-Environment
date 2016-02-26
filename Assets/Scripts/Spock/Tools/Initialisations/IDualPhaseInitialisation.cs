using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public interface IDualPhaseInitialisation : IMonoPhaseInitialisation {

        void Preinit();
        
    }

}
