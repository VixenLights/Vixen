using System.Drawing;
using VixenModules.App.CustomPropEditor.Import.XLights.Ranges;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class SubModel
	{
		public SubModel(string name)
		{
			Name = name;
		}
		public string Name { get; }

		public Color Color { get; set; }

		public string Layout { get; set; }

		public ModelType Type { get; set; }

		public List<RangeGroup> Ranges { get; set; }

	}
}
