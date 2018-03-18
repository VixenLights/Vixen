using System;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Module;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.LumaKey
{
	public class LumaKeyModule : LayerMixingFilterModuleInstanceBase
	{
		private LumaKeyData _data;

		public override DiscreteValue CombineDiscreteIntensity(DiscreteValue highLayerValue, DiscreteValue lowLayerValue)
		{
			if (lowLayerValue.Intensity >= _data.LowerLimit && lowLayerValue.Intensity <= _data.UpperLimit)
			{
				return highLayerValue;
			}
			return lowLayerValue;
		}

		public override Color CombineFullColor(Color highLayerColor, Color lowLayerColor)
		{
			var lowLayerV = Math.Round(HSV.VFromRgb(lowLayerColor), 2);
			return (lowLayerV >= _data.LowerLimit && lowLayerV <= _data.UpperLimit) ? highLayerColor : lowLayerColor;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = (LumaKeyData)value; }
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (var setup = new LumaKeySetup(_data.LowerLimit, _data.UpperLimit))
			{
				if (setup.ShowDialog() != DialogResult.OK) return false;
				_data.LowerLimit = setup.LowerLimit;
				_data.UpperLimit = setup.UpperLimit;
				return true;
			}
		}
	}
}
