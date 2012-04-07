using Vixen.Commands;

namespace Vixen.Sys.Output {
	public interface IOutputModule {
		//void UpdateState(ICommand[] outputStates);
		int UpdateInterval { get; }
		IDataPolicy DataPolicy { get; }
	}
}
