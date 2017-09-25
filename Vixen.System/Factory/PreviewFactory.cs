using System;
using Vixen.Module;
using Vixen.Module.Preview;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory
{
	public class PreviewFactory
	{
		public IOutputDevice CreateDevice(Guid typeId, string name)
		{
			Guid instanceId = Guid.NewGuid();
			return CreateDevice(typeId, instanceId, name);
		}

		public IOutputDevice CreateDevice(Guid moduleId, Guid moduleInstanceId, string name)
		{
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer<IPreviewModuleInstance> outputModuleConsumer =
				new OutputModuleConsumer<IPreviewModuleInstance>(moduleId, moduleInstanceId, dataRetriever);
			outputModuleConsumer.Module.Name = name;
			IHardware executionControl = new BasicOutputModuleExecutionControl(outputModuleConsumer.Module);
			// Yes, we are intentionally using the module instance id as the device id.
			// Previews are not referenced by id at runtime in the way that controllers are referenced for linking.
			// We foresee no conflicts in doing this.
			return new OutputPreview(moduleInstanceId, name, executionControl, outputModuleConsumer);
		}
	}
}