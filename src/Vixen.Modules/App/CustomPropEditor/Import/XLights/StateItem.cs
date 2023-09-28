﻿using System.Drawing;
using VixenModules.App.CustomPropEditor.Import.XLights.Ranges;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
    public class StateItem
	{
		public StateItem(int index, string name)
		{
			Name = name;
			Index = index;
		}

		public int Index { get; set; }

		public string Name { get; }

		public Color Color { get; set; } = Color.White;

		public RangeGroup RangeGroup { get; set; }
	}
}
