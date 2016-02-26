
namespace Spock {

	public interface IController : IDualPhaseInitialisation {

        void SetSpockInstance(SpockInstance S);
        void SetEnabled(bool enabled);

	}

}
