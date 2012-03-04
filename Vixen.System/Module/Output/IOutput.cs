using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.Output {
    public interface IOutput {
        // As defined by the owning controller.
        int OutputCount { get; set; }
        // One command for each output as defined by the owning controller.
        void UpdateState(ICommand[] outputStates);
		//IEnumerable<ITransformModuleInstance> BaseTransforms { get; set; }
		//void AddTransform(int outputIndex, ITransformModuleInstance transformModule);
		//void RemoveTransform(int outputIndex, Guid transformTypeId, Guid transformInstanceId);
		//void SetTransforms(int outputIndex, IEnumerable<ITransformModuleInstance> transforms);
		//IEnumerable<ITransformModuleInstance> GetTransforms(int outputIndex);
		ModuleLocalDataSet ModuleDataSet { get; set; }
		int UpdateInterval { get; }
    	int ChainIndex { get; set; }
    	IDataPolicy DataPolicy { get; }
    }
}
