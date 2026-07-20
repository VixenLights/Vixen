namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal sealed class CircleXModelConfiguration
	{
		public string Name { get; init; }

		public List<int> LayerSizes { get; init; } = [];

		public bool InsideOut { get; init; }

		public char StartSide { get; init; }

		public char Direction { get; init; }

		public int CenterPercent { get; init; }

		public int PixelSize { get; init; }

		public int PixelCount { get; init; }

		public int NumStrings { get; init; }

		public int NodesPerString { get; init; }
	}
}
