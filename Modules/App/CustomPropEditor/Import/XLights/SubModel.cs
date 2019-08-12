using System.Collections.Generic;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class SubModel
	{
		public SubModel()
		{
			FaceInfo = new FaceInfo(FaceComponent.None);
		}
		public string Name { get; set; }

		public string Layout { get; set; }

		public SubModelType Type { get; set; }

		public List<Range> Ranges { get; set; }

		public FaceInfo FaceInfo { get; set; }
	}

	public class Range
	{
		public int Start { get; set; }

		public int End { get; set; }
	}

	public enum SubModelType
	{
		Ranges
	}
}
