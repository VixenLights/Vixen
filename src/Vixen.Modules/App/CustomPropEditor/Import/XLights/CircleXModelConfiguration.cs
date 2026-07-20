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

		public double ScaleX { get; init; } = 1.0;

		public double ScaleY { get; init; } = 1.0;

		public int PixelCount { get; init; }

		public int NumStrings { get; init; }

		public int NodesPerString { get; init; }

		public List<CircleXModelRing> Rings { get; init; } = [];
	}
}
