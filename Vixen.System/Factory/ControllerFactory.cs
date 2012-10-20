using System;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	public class ControllerFactory {
		public IOutputDevice CreateDevice(Guid moduleId, string name) {
			return CreateDevice(Guid.NewGuid(), moduleId, Guid.NewGuid(), name);
		}

		public IOutputDevice CreateDevice(Guid deviceId, Guid moduleId, Guid moduleInstanceId, string name) {
			IHasOutputs<CommandOutput> outputs = new OutputCollection<CommandOutput>();
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer<IControllerModuleInstance> outputModuleConsumer = new OutputModuleConsumer<IControllerModuleInstance>(moduleId, moduleInstanceId, dataRetriever);
			IOutputMediator<CommandOutput> outputMediator = new OutputMediator<CommandOutput>(outputs, outputModuleConsumer.Module);
			IHardware executionControl = new BasicOutputModuleExecutionControl(outputModuleConsumer.Module);
			return new OutputController(deviceId, name, outputMediator, executionControl, outputModuleConsumer);
		}
	}
}
