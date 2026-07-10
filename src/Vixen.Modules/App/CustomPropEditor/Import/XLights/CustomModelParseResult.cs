namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal sealed class CustomModelParseResult
	{
		internal CustomModelParseResult(
			CustomModelSource source,
			Dictionary<int, ModelNode> modelNodes,
			FormatException compressedException,
			FormatException customModelException)
		{
			Source = source;
			ModelNodes = modelNodes;
			CompressedException = compressedException;
			CustomModelException = customModelException;
		}

		internal CustomModelSource Source { get; }

		internal Dictionary<int, ModelNode> ModelNodes { get; }

		internal FormatException CompressedException { get; }

		internal FormatException CustomModelException { get; }

		internal bool Success => Source != CustomModelSource.None;
	}
}
