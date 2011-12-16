using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Transform;
using Vixen.Commands;

namespace Vixen.Module.Output {
    public interface IOutput {
        // As defined by the owning controller.
        int OutputCount { get; set; }
        // One command for each output as defined by the owning controller.
        void UpdateState(Command[] outputStates);
		IEnumerable<ITransformModuleInstance> BaseTransforms { get; set; }
		void AddTransform(int outputIndex, ITransformModuleInstance transformModule);
		void RemoveTransform(int outputIndex, Guid transformTypeId, Guid transformInstanceId);
		void SetTransforms(int outputIndex, IEnumerable<ITransformModuleInstance> transforms);
		IEnumerable<ITransformModuleInstance> GetTransforms(int outputIndex);
		IModuleDataSet ModuleDataSet { get; set; }
		int UpdateInterval { get; }
    	int ChainIndex { get; set; }
    }
}
