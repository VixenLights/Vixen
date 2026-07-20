using VixenModules.App.CustomPropEditor.Import.XLights;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Custom
{
	internal sealed class CustomXModelParseResult
	{
		internal CustomXModelParseResult(
			CustomXModelSource source,
			Dictionary<int, ModelNode> modelNodes,
			FormatException compressedException,
			FormatException customModelException)
		{
			Source = source;
			ModelNodes = modelNodes;
			CompressedException = compressedException;
			CustomModelException = customModelException;
		}

		internal CustomXModelSource Source { get; }

		internal Dictionary<int, ModelNode> ModelNodes { get; }

		internal FormatException CompressedException { get; }

		internal FormatException CustomModelException { get; }

		internal bool Success => Source != CustomXModelSource.None;
	}
}
