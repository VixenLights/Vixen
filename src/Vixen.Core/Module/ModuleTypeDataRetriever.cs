namespace Vixen.Module
{
	internal class ModuleTypeDataRetriever : IModuleDataRetriever
	{
		private IModuleDataSet _moduleDataSet;

		public ModuleTypeDataRetriever(IModuleDataSet moduleDataSet)
		{
			_moduleDataSet = moduleDataSet;
		}

		public void AssignModuleData(IModuleInstance module)
		{
			_moduleDataSet.AssignModuleTypeData(module);
		}
	}
}