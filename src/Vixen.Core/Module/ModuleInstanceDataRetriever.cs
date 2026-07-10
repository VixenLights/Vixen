namespace Vixen.Module
{
	internal class ModuleInstanceDataRetriever : IModuleDataRetriever
	{
		private IModuleDataSet _moduleDataSet;

		public ModuleInstanceDataRetriever(IModuleDataSet moduleDataSet)
		{
			_moduleDataSet = moduleDataSet;
		}

		public void AssignModuleData(IModuleInstance module)
		{
			_moduleDataSet.AssignModuleInstanceData(module);
		}
	}
}