using System;
using Vixen.Module;
using Vixen.Module.Preview;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	public class PreviewFactory : IOutputDeviceFactory {
		public IOutputDevice CreateDevice(Guid typeId, string name) {
			Guid instanceId = Guid.NewGuid();
			return CreateDevice(instanceId, typeId, instanceId, name);
		}

		public IOutputDevice CreateDevice(Guid id, Guid moduleId, Guid moduleInstanceId, string name) {
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer<IPreviewModuleInstance> outputModuleConsumer = new OutputModuleConsumer<IPreviewModuleInstance>(moduleId, moduleInstanceId, dataRetriever);
			IHardware executionControl = new BasicOutputModuleExecutionControl(outputModuleConsumer.Module);
			return new OutputPreview(id, name, executionControl, outputModuleConsumer);
		}
	}
}
