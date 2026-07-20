namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal static class CustomXModelSourceResolver
	{
		internal static CustomXModelParseResult Resolve(
			string compressedModelDefinition,
			string modelDefinition,
			int scale)
		{
			return Resolve(compressedModelDefinition, modelDefinition, new XModelCoordinateScale(scale, scale));
		}

		internal static CustomXModelParseResult Resolve(
			string compressedModelDefinition,
			string modelDefinition,
			XModelCoordinateScale scale)
		{
			FormatException compressedException = null;
			FormatException customModelException = null;

			if (!string.IsNullOrWhiteSpace(compressedModelDefinition))
			{
				try
				{
					return new CustomXModelParseResult(
						CustomXModelSource.CustomModelCompressed,
						CustomXModelNodeParser.ParseCustomModelCompressed(compressedModelDefinition, scale),
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
					return new CustomXModelParseResult(
						CustomXModelSource.CustomModel,
						CustomXModelNodeParser.ParseCustomModel(modelDefinition, scale),
						compressedException,
						null);
				}
				catch (FormatException ex)
				{
					customModelException = ex;
				}
			}

			return new CustomXModelParseResult(
				CustomXModelSource.None,
				[],
				compressedException,
				customModelException);
		}
	}
}
