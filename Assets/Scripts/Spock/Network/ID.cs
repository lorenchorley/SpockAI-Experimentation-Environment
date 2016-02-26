using UnityEngine;
using System.Collections;

namespace Spock {

    public class ID<C> where C : INetworkComponent {

        public readonly C component;
        public readonly long id;

        public ID(long id, C component) {
            this.id = id;
            this.component = component;
        }

        public override bool Equals(object obj) {
            if (obj.GetType() != typeof(ID<C>))
                return false;

            return ((ID<C>) obj).id == id;
        }

        public override int GetHashCode() {
            return (int) id;
        }

        public override string ToString() {
            return id.ToString();
        }

    }

}