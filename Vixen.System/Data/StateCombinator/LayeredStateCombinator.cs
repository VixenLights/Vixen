using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using Vixen.Extensions;
using Vixen.Intent;
using Vixen.Sys;

namespace Vixen.Data.StateCombinator
{
	public class LayeredStateCombinator:StateCombinator<LayeredStateCombinator>
	{
		private Color _combinedMixingColor = Color.Empty;
		private Color _tempMixingColor = Color.Empty;
		private Dictionary<int, DiscreteValue> _combinedDiscreteColors = new Dictionary<int, DiscreteValue>(4);
		private readonly Dictionary<int, DiscreteValue> _tempDiscreteColors = new Dictionary<int, DiscreteValue>(4);
		private readonly StaticIntentState<RGBValue> _mixedIntentState = new StaticIntentState<RGBValue>(new RGBValue(Color.Black));
		
		public override List<IIntentState> Combine(List<IIntentState> states)
		{
			StateCombinatorValue.Clear();
			if (states == null || states.Count <= 0) return StateCombinatorValue;
			if (states.Count == 1)
			{
				StateCombinatorValue.Add(states[0]);
				return StateCombinatorValue;
			}

			_tempMixingColor = Color.Empty;
			_combinedMixingColor = Color.Empty;
			_combinedDiscreteColors.Clear();
			_tempDiscreteColors.Clear();

			var orderedStates = states.OrderByDescending(x => x.Layer).GroupBy(x => x.Layer);
			
			foreach (var intentStateGroups in orderedStates)
			{
				foreach (IIntentState intentState in intentStateGroups)
				{
					intentState.Dispatch(this);
				}
				if (_tempMixingColor != Color.Empty)
				{
					_combinedMixingColor = _combinedMixingColor == Color.Empty ? _tempMixingColor : MixLayerColors(_combinedMixingColor, _tempMixingColor);
				}
				if (_tempDiscreteColors.Count > 0)
				{
					MixDiscreteLayerColors(ref _combinedDiscreteColors, _tempDiscreteColors);
				}
				_tempMixingColor = Color.Empty;
				_tempDiscreteColors.Clear();
			}
			
			if (_combinedMixingColor != Color.Empty)
			{
				_mixedIntentState.SetValue(new RGBValue(_combinedMixingColor));
				StateCombinatorValue.Add(_mixedIntentState);
			}

			if (_combinedDiscreteColors.Count > 0)
			{
				foreach (var combinedDiscreteColor in _combinedDiscreteColors)
				{
					StateCombinatorValue.Add(new StaticIntentState<DiscreteValue>(combinedDiscreteColor.Value));
				}
			}

			return StateCombinatorValue;
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			_tempMixingColor = _tempMixingColor == Color.Empty ? obj.GetValue().FullColor : _tempMixingColor.Mix(obj.GetValue().FullColor);
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			_tempMixingColor = _tempMixingColor == Color.Empty ? obj.GetValue().FullColor : _tempMixingColor.Mix(obj.GetValue().FullColor);
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			var discreteValue = obj.GetValue();
			var argbColor = discreteValue.FullColor.ToArgb();
			DiscreteValue value;

			if (_tempDiscreteColors.TryGetValue(argbColor, out value))
			{
				if (value.Intensity < discreteValue.Intensity)
				{
					_tempDiscreteColors[argbColor] = discreteValue;
				}
			}
			else
			{
				_tempDiscreteColors[argbColor] = discreteValue;
			}	
		}

		public override void Handle(IIntentState<CommandValue> obj)
		{
			//You may pass untouched for now
			StateCombinatorValue.Add(obj);
		}

		public override void Handle(IIntentState<PositionValue> obj)
		{
			//You may pass untouched for now
			StateCombinatorValue.Add(obj);
		}

		private static Color MixLayerColors(Color highLayer, Color lowLayer)
		{
			var hsv = HSV.FromRGB(lowLayer);
			hsv.V = hsv.V*(1 - HSV.VFromRgb(highLayer));
			return highLayer.Mix(hsv.ToRGB());
		}

		private static void MixDiscreteLayerColors(ref Dictionary<int, DiscreteValue> highLayer, Dictionary<int, DiscreteValue> lowLayer)
		{
			//We are going to look at the lower layer and modify any values in the higher layer if the proportioned value is 
			//higher than our existing colors intensity
			foreach (var color in lowLayer)
			{
				DiscreteValue highDiscreteValue;
				if (highLayer.TryGetValue(color.Key, out highDiscreteValue))
				{
					double intensity = color.Value.Intensity*(1 - highDiscreteValue.Intensity);
					highDiscreteValue.Intensity = Math.Max(intensity, highDiscreteValue.Intensity);
				}
				else
				{
					highLayer[color.Key] = color.Value;
				}
			}
		}
		
	}
}
