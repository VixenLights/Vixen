using System.Collections.Generic;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	public class StateInfo
	{
		public StateInfo(string name)
		{
			StateItems = new List<StateItem>();
			ModelType = ModelType.NodeRanges;
			Name = name;
		}

		public string Name { get; }

		public bool CustomColors { get; set; }

		public ModelType ModelType { get; set; }

		public List<StateItem> StateItems { get; set; }
	}
}
