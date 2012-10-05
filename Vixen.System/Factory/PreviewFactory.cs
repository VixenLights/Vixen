using System;
using Vixen.Module;
using Vixen.Module.Preview;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	public class PreviewFactory : IOutputDeviceFactory {
		public IOutputDevice CreateDevice(Guid moduleId, string name) {
			return CreateDevice(Guid.NewGuid(), moduleId, Guid.NewGuid(), name);
		}

		public IOutputDevice CreateDevice(Guid deviceId, Guid moduleId, Guid moduleInstanceId, string name) {
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer<IPreviewModuleInstance> outputModuleConsumer = new OutputModuleConsumer<IPreviewModuleInstance>(moduleId, moduleInstanceId, dataRetriever);
			IHardware executionControl = new BasicOutputModuleExecutionControl(outputModuleConsumer.Module);
			return new OutputPreview(deviceId, name, executionControl, outputModuleConsumer);
		}
	}
}
