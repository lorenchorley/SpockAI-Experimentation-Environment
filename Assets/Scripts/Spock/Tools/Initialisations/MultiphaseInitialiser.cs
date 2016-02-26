using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spock {

    public class MultiphaseInitialiser {

        public static void Initialise<I>(Action<I> callback, IEnumerable<I> objs) where I : IMonoPhaseInitialisation {
            Initialise(callback, objs.ToArray<I>());
        }

        public static void Initialise<I>(Action<I> callback, params I[] objs) where I : IMonoPhaseInitialisation {
            if (objs.Length == 0)
                return;

            foreach (I obj in objs) {
                callback.Invoke(obj);
            }

            Initialise(objs);

        }

        private static void Initialise<I>(IEnumerable<I> objs) where I : IMonoPhaseInitialisation {

            if (objs.GetEnumerator().Current is IDualPhaseInitialisation) {
                foreach (IDualPhaseInitialisation obj in objs) {
                    obj.Preinit();
                }
            }

            foreach (I obj in objs) {
                obj.Init();
            }

        }

        private static void Initialise<I>(params I[] objs) where I : IMonoPhaseInitialisation {

            if (objs[0] is IDualPhaseInitialisation) {
                foreach (IDualPhaseInitialisation obj in objs) {
                    obj.Preinit();
                }
            }

            foreach (I obj in objs) {
                obj.Init();
            }

        }

        // TODO
        public static void DoForEach(Action<NodeComponent> callback, params NodeComponent[] components) {
            foreach (NodeComponent component in components)
                callback.Invoke(component);
        }

    }

}
