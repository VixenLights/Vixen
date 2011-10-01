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
		void AddTransform(int outputIndex, Guid transformTypeId, Guid transformInstanceId = default(Guid));
		void RemoveTransform(int outputIndex, Guid transformTypeId, Guid transformInstanceId);
		IEnumerable<ITransformModuleInstance> GetOutputTransforms(int outputIndex);
		IModuleDataSet TransformModuleData { get; set; }
    }
}
