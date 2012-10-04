using System;
using Vixen.Module;
using Vixen.Module.Preview;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	public class PreviewFactory : IOutputDeviceFactory {
		public IOutputDevice CreateDevice(Guid moduleId, string name) {
			return CreateDevice(Guid.NewGuid(), moduleId, name);
		}

		public IOutputDevice CreateDevice(Guid id, Guid moduleId, string name) {
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer<IPreviewModuleInstance> outputModuleConsumer = new OutputModuleConsumer<IPreviewModuleInstance>(moduleId, id, dataRetriever);
			IHardware executionControl = new BasicOutputModuleExecutionControl(outputModuleConsumer.Module);
			return new OutputPreview(id, name, executionControl, outputModuleConsumer);
		}
	}
}
