using UnityEngine;
using System.Collections.Generic;
using Spock;

namespace Spock {

    public interface ISpockComponent {

        void SetRepresentation(IRepresentation representation);
        IRepresentation GetRepresentation();
        R GetRepresentation<R>() where R : IRepresentation;

    }

}