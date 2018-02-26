using System.Collections.Generic;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class SubModel
	{
		public string Name { get; set; }

		public string Layout { get; set; }

		public SubModelType Type { get; set; }

		public List<Range> Ranges { get; set; }
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
