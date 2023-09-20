using System.Drawing;
using VixenModules.App.CustomPropEditor.Import.XLights.Ranges;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Faces
{
	internal class FaceItem
	{
		public FaceComponent FaceComponent { get; set; }

		public string Name { get; set; }

		public Color Color { get; set; } = Color.White;

		public RangeGroup RangeGroup { get; set; }
	}
}
