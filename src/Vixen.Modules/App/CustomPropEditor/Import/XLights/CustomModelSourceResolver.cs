namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal static class CustomModelSourceResolver
	{
		internal static CustomModelParseResult Resolve(
			string compressedModelDefinition,
			string modelDefinition,
			int scale)
		{
			FormatException compressedException = null;
			FormatException customModelException = null;

			if (!string.IsNullOrWhiteSpace(compressedModelDefinition))
			{
				try
				{
					return new CustomModelParseResult(
						CustomModelSource.CustomModelCompressed,
						CustomModelParser.ParseCustomModelCompressed(compressedModelDefinition, scale),
						null,
						null);
				}
				catch (FormatException ex)
				{
					compressedException = ex;
				}
			}

			if (!string.IsNullOrWhiteSpace(modelDefinition))
			{
				try
				{
					return new CustomModelParseResult(
						CustomModelSource.CustomModel,
						CustomModelParser.ParseCustomModel(modelDefinition, scale),
						compressedException,
						null);
				}
				catch (FormatException ex)
				{
					customModelException = ex;
				}
			}

			return new CustomModelParseResult(
				CustomModelSource.None,
				[],
				compressedException,
				customModelException);
		}
	}
}
