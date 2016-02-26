using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public abstract class SpockInitialiser : IDualPhaseInitialisation {

        protected readonly SpockInstance S;
        
        protected abstract void Start();
        protected abstract void Finish(); // The only place in the initialiser it is safe to work with representations
        protected abstract IEffectFactory SetupEffectFactory();
        //protected abstract void SetupEventActionHandler();
        protected abstract void SetupEnvironments();
        protected abstract void SetupComponentTemplates();
        protected abstract void SetupNetworks();
        protected abstract void SetupComponentInterfaces();

        private bool preinit = false;
        private bool init = false;

        public SpockInitialiser(Dictionary<Type, IController> Controllers) {
            S = new SpockInstance(Controllers, SetupEffectFactory());
        }

        public void Preinit() {
            Assert.True("Preinit assert", !preinit && !init);
            
            preinit = true;
        }

        public void Init() {
            Assert.True("Init assert", preinit && !init);

            Start();
            
            SetupEnvironments();
            SetupComponentTemplates();
            SetupNetworks();
            SetupComponentInterfaces();

            S.Get<EffectController>().EnqueueEffect_TS(new CallbackEffect(Finish));

            init = true;
        }

        public SpockInstance GetInstance() {
            Assert.True("GetInstance assert", preinit && init);
            return S;
        }

        public void EventActionFallback(int code, params object[] parameters) { }

    }

}
