using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	internal class DiscreteIntentHandler: IntentStateDispatch
	{
		private readonly Dictionary<Color, double> _colorMap = new Dictionary<Color, double>(); 
		private readonly List<Color> _colors = new List<Color>();
		public List<Color> GetAlphaAffectedColor(IIntentStates states)
		{
			_colorMap.Clear();
			_colors.Clear();
			foreach (var intentState in states.AsList())
			{
				intentState.Dispatch(this);
			}
			foreach (var d in _colorMap)
			{
				var c = d.Key;
				_colors.Add(Color.FromArgb((byte)(d.Value * 255), c.R, c.G, c.B));
			}

			return _colors;
			
		} 

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			var value = obj.GetValue();
			HandleColor(value.Color, value.Intensity);
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			var value = obj.GetValue();
			HandleColor(value.FullColor, value.Intensity);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			var value = obj.GetValue();
			HandleColor(value.Color, value.Intensity);
		}

		private void HandleColor(Color c, double intensity)
		{
			double i;
			if (_colorMap.TryGetValue(c, out i))
			{
				if (intensity < i)
				{
					return;
				}
			}
			
			_colorMap[c] = intensity;
			
		}
	}
}
