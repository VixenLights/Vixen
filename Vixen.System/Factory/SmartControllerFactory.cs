using System;
using Vixen.Module;
using Vixen.Module.SmartController;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	public class SmartControllerFactory : IOutputDeviceFactory {
		public IOutputDevice CreateDevice(Guid moduleId, string name) {
			return CreateDevice(Guid.NewGuid(), moduleId, name);
		}

		public IOutputDevice CreateDevice(Guid id, Guid moduleId, string name) {
			IHasOutputs<IntentOutput> outputs = new OutputCollection<IntentOutput>();
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer<ISmartControllerModuleInstance> outputModuleConsumer = new OutputModuleConsumer<ISmartControllerModuleInstance>(moduleId, dataRetriever);
			IOutputMediator<IntentOutput> outputMediator = new OutputMediator<IntentOutput>(outputs, outputModuleConsumer.Module);
			IHardware executionControl = new BasicOutputModuleExecutionControl(outputModuleConsumer.Module);
			return new SmartOutputController(id, name, outputMediator, executionControl, outputModuleConsumer);
		}
	}
}
