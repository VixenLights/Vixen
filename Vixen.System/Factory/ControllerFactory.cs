using System;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	public class ControllerFactory : IOutputDeviceFactory {
		public IOutputDevice CreateDevice(Guid moduleId, string name) {
			return CreateDevice(Guid.NewGuid(), moduleId, name);
		}

		public IOutputDevice CreateDevice(Guid id, Guid moduleId, string name) {
			IHasOutputs<CommandOutput> outputs = new OutputCollection<CommandOutput>();
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer<IControllerModuleInstance> outputModuleConsumer = new OutputModuleConsumer<IControllerModuleInstance>(moduleId, dataRetriever);
			IOutputMediator<CommandOutput> outputMediator = new OutputMediator<CommandOutput>(outputs, outputModuleConsumer.Module);
			IHardware executionControl = new BasicOutputModuleExecutionControl(outputModuleConsumer.Module);
			return new OutputController(id, name, outputMediator, executionControl, outputModuleConsumer);
		}
	}
}
