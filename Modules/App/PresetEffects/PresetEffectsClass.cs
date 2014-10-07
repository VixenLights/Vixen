using System;

namespace VixenModules.App.PresetEffects
{
	public class PresetEffect
	{
		public string Name { get; set; }

		public string EffectTypeName { get; set; }

		public Guid EffectTypeGuid { get; set; }

		public object[] ParameterValues { get; set; }
	}
}
