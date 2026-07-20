namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal sealed class XModelParsedModel
	{
		public XModelParsedModel(XModelImportModel importModel)
		{
			ImportModel = importModel;
		}

		public XModelImportModel ImportModel { get; }

		public List<XModelGeneratedGroup> GeneratedGroups { get; } = [];
	}
}
