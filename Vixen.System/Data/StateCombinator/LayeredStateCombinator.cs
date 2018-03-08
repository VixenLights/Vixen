using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module.MixingFilter;
using Vixen.Sys;

namespace Vixen.Data.StateCombinator
{
	public class LayeredStateCombinator:StateCombinator<LayeredStateCombinator>
	{
		private Color? _combinedMixingColor;
		private Color? _tempMixingColor;
		private Dictionary<int, DiscreteValue> _combinedDiscreteColors = new Dictionary<int, DiscreteValue>(4);
		private readonly Dictionary<int, DiscreteValue> _tempDiscreteColors = new Dictionary<int, DiscreteValue>(4);
		private readonly StaticIntentState<RGBValue> _mixedIntentState = new StaticIntentState<RGBValue>(new RGBValue(Color.Black));
		private static readonly IntentStateLayerComparer LayerComparer = new IntentStateLayerComparer();
		private static readonly List<IIntentState>  EmptyState = Enumerable.Empty<IIntentState>().ToList();

		public override List<IIntentState> Combine(List<IIntentState> states)
		{
			//Reset our return type and check to see if we really have anything to combine. 
			//If we have one or none we can skip all the complex stuff			
			if (states.Count <= 1)
			{
				if (states.Count == 1 && states[0].Layer.RequiresMixingPartner)
				{
					return EmptyState;
				}
				return states;
			}

			//Reset all our temp variables
			StateCombinatorValue.Clear();
			_tempMixingColor = null;
			_combinedMixingColor = null;
			_combinedDiscreteColors.Clear();
			_tempDiscreteColors.Clear();

			//Order all states in decending order by layer
			//We are going to do this without Linq because it is way more memory efficient
			states.Sort(LayerComparer);

			//Establish the top level layer
			var currentLayer = states[0].Layer;
			ILayerMixingFilter filter= null;
			
			//Walk through the groups of layers and process them
			foreach (var state in states)
			{
				//Iterate the states in the layer
				if (state.Layer.LayerLevel == currentLayer.LayerLevel)
				{
					//Dispatch each state to the Handle method for its type to combine down to one state
					//per layer in a mixing fashion
					state.Dispatch(this);
					continue;
				}

				//We have a new layer so we need to wrap up the previous one
				MixLayers(filter);
				filter = currentLayer.LayerMixingFilter;
				state.Dispatch(this);
				currentLayer = state.Layer;
			}

			//Mix the final layer
			
			MixLayers(filter);

			//Now we should be down to one mixing type and we can put that in our return obejct as a RGBValue.
			//This will convert all mxing types to the simpler more efficient RGBValue 
			if (_combinedMixingColor != null)
			{
				_mixedIntentState.SetValue(new RGBValue(_combinedMixingColor.Value));
				StateCombinatorValue.Add(_mixedIntentState);
			}

			//Do the same for the discrete types. Here we should be down to one per color and we return these as Discrete values
			//so we can maintatin the source color and an intensity
			if (_combinedDiscreteColors.Count > 0)
			{
				foreach (var combinedDiscreteColor in _combinedDiscreteColors)
				{
					StateCombinatorValue.Add(new StaticIntentState<DiscreteValue>(combinedDiscreteColor.Value));
				}
			}

			return StateCombinatorValue;
		}

		private void MixLayers(ILayerMixingFilter filter)
		{
			//Check to see if the layer had any mixing type colors that were combined down to one to process
			if (_tempMixingColor != null)
			{
				//If there are then mix that with the previous layer with the layer mixing logic
				//If we have not seen a previous layer, just assign this color as our color
				//Otherwise pass this color and the previous layer color into the mixing logic
				_combinedMixingColor = _combinedMixingColor == null
					? _tempMixingColor
					: MixLayerColors(_combinedMixingColor.Value, _tempMixingColor.Value, filter);
			}
			//Check to see if there were any discrete types. These are combined down to one per color
			if (_tempDiscreteColors.Count > 0)
			{
				if (_combinedDiscreteColors.Count == 0)
				{
					foreach (var tempDiscreteColor in _tempDiscreteColors)
					{
						_combinedDiscreteColors[tempDiscreteColor.Key] = tempDiscreteColor.Value;
					}
				}
				else
				{
					//If there are then mix that with the previous layer with the layer mixing logic
					//Discrete are special, so we have to handle them a litle different. We need to maintain each indivdual color and manipulate the intensity
					MixDiscreteLayerColors(ref _combinedDiscreteColors, _tempDiscreteColors, filter);
				}
				
			}

			//reset our temp variables and go back for more layers
			_tempMixingColor = Color.Empty;
			_tempDiscreteColors.Clear();
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			//Handles mixing the LightingValue type intents within the same layer all togther in a simple mixing form of 
			//highest value R, G, B
			_tempMixingColor = _tempMixingColor == null ? obj.GetValue().FullColor : _tempMixingColor.Value.Combine(obj.GetValue().FullColor);
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			//Handles mixing the RGBValue type intents within the same layer all togther in a simple mixing form of 
			//highest value R, G, B
			_tempMixingColor = _tempMixingColor == null ? obj.GetValue().FullColor : _tempMixingColor.Value.Combine(obj.GetValue().FullColor);
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			//Handles mixing the DiscreteValue type intents within the same layer all togther in a highest intensity
			//per color fashion
			var discreteValue = obj.GetValue();
			var argbColor = discreteValue.Color.ToArgb();
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

		//For Command and Position values we can defer to the base implementation in StateCombinator
		//so we don't need handle methods for those
		

		private static Color MixLayerColors(Color highLayer, Color lowLayer, ILayerMixingFilter filter)
		{
			//Mixing logic for mixing colors between layers
			 return filter.CombineFullColor(highLayer, lowLayer);
			
			//var hsv = HSV.FromRGB(lowLayer);
			//hsv.V = hsv.V*(1 - HSV.VFromRgb(highLayer));
			//return highLayer.Combine(hsv.ToRGB());
		}

		private static void MixDiscreteLayerColors(ref Dictionary<int, DiscreteValue> highLayer, Dictionary<int, DiscreteValue> lowLayer, ILayerMixingFilter filter)
		{
			//Mixing logic for Discrete colors between layers
			//We are going to look at the lower layer and modify any values in the higher layer if the proportioned value is 
			//higher than our existing colors intensity
			foreach (var color in lowLayer)
			{
				DiscreteValue highDiscreteValue;
				if (highLayer.TryGetValue(color.Key, out highDiscreteValue))
				{
					var value = filter.CombineDiscreteIntensity(highDiscreteValue, color.Value);
					if (value.Intensity > 0)
					{
						highLayer[color.Key] = value;
					}
					else
					{
						highLayer.Remove(color.Key);
					}
					//double intensity = color.Value.Intensity*(1 - highDiscreteValue.Intensity);
					//highDiscreteValue.Intensity = Math.Max(intensity, highDiscreteValue.Intensity);
					//highLayer[color.Key] = highDiscreteValue; //this is a struct, so put our modified copy back in the collection.
				}
				else
				{
					highLayer[color.Key] = color.Value;
				}
			}
		}
		
	}

	/// <summary>
	/// Class to order states in decending order by layer
	/// </summary>
	internal class IntentStateLayerComparer : IComparer<IIntentState>
	{
		public int Compare(IIntentState x, IIntentState y)
		{
			return -1 * x.Layer.LayerLevel.CompareTo(y.Layer.LayerLevel);
		}
	}
}
