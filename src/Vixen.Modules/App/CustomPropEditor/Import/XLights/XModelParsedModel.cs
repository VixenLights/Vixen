namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal sealed class XModelParsedModel
	{
		public XModelParsedModel(CustomModel customModel)
		{
			CustomModel = customModel;
		}

		public CustomModel CustomModel { get; }

		public List<XModelGeneratedGroup> GeneratedGroups { get; } = [];
	}
}
