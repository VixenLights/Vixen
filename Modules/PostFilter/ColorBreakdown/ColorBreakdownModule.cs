using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	public class ColorBreakdownDescriptor : OutputFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{ab38a16f-0de1-4f6e-a8c0-ae5b20d347e0}");

		public override string TypeName
		{
			get { return "Color Breakdown"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public static Guid ModuleId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ColorBreakdownModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (ColorBreakdownData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "An output filter that breaks down color intents into discrete color components."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}




	public class ColorBreakdownModule : OutputFilterModuleInstanceBase
	{
		private ColorBreakdownData _data;
		private ColorBreakdownOutput[] _outputs;

		public override void Handle(IntentsDataFlowData obj)
		{
			foreach (ColorBreakdownOutput output in Outputs) {
				output.ProcessInputData(obj);
			}
		}

		public override DataFlowType InputDataType
		{
			get { return DataFlowType.MultipleIntents; }
		}

		public override DataFlowType OutputDataType
		{
			get { return DataFlowType.SingleIntent; }
		}

		public override IDataFlowOutput[] Outputs
		{
			get { return _outputs; }
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (ColorBreakdownData) value;
				_CreateOutputs();
			}
		}


		public List<ColorBreakdownItem> BreakdownItems
		{
			get { return _data.BreakdownItems; }
			set
			{
				_data.BreakdownItems = value;
				_CreateOutputs();
			}
		}

		public bool MixColors
		{
			get { return _data.MixColors; }
			set
			{
				_data.MixColors = value;
				_CreateOutputs();
			}
		}


		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (ColorBreakdownSetup setup = new ColorBreakdownSetup(_data)) {
				if (setup.ShowDialog() == DialogResult.OK) {
					_data.BreakdownItems = setup.BreakdownItems;
					_CreateOutputs();
					return true;
				}
			}
			return false;
		}

		private void _CreateOutputs()
		{
			_outputs = _data.BreakdownItems.Select(x => new ColorBreakdownOutput(x, _data.MixColors)).ToArray();
		}
	}




	public class ColorBreakdownData : ModuleDataModelBase
	{
		public ColorBreakdownData()
		{
			BreakdownItems = new List<ColorBreakdownItem>();
		}

		public override IModuleDataModel Clone()
		{
			ColorBreakdownData newInstance = new ColorBreakdownData();
			newInstance.BreakdownItems = new List<ColorBreakdownItem>(BreakdownItems);
			newInstance.MixColors = MixColors;
			return newInstance;
		}

		[DataMember]
		public List<ColorBreakdownItem> BreakdownItems { get; set; }

		[DataMember]
		public bool MixColors { get; set; }
	}




	[DataContract]
	public class ColorBreakdownItem
	{
		public ColorBreakdownItem()
		{
			Color = Color.White;
			Name = "Unnamed";
		}

		[DataMember]
		public Color Color { get; set; }

		[DataMember]
		public string Name { get; set; }
	}

	internal interface IBreakdownFilter
	{
		double GetIntensityForState(IIntentState intentValue);
	}

	/// <summary>
	/// This filter gets the intensity percent for a given state for non mixing colors
	/// </summary>
	internal class ColorBreakdownFilter : IntentStateDispatch, IBreakdownFilter
	{
		private double _intensityValue;
		private readonly ColorBreakdownItem _breakdownItem;
		private const double Tolerance = .0001; //For how close the Hue and Saturation should match for Discrete.
		
		public ColorBreakdownFilter(ColorBreakdownItem breakdownItem)
		{
			_breakdownItem = breakdownItem;
		}

		/// <summary>
		/// Process the intent and return a value that represents the percent of intensity for the state
		/// </summary>
		/// <param name="intentValue"></param>
		/// <returns></returns>
		public double GetIntensityForState(IIntentState intentValue)
		{
			intentValue.Dispatch(this);
			return _intensityValue;
		}

		private float _getMaxProportion(Color inputColor)
		{
			float result = 1.0f;

			if (_breakdownItem.Color.R > 0)
				result = Math.Min(result, (float) inputColor.R/_breakdownItem.Color.R);

			if (_breakdownItem.Color.G > 0)
				result = Math.Min(result, (float) inputColor.G/_breakdownItem.Color.G);

			if (_breakdownItem.Color.B > 0)
				result = Math.Min(result, (float) inputColor.B/_breakdownItem.Color.B);

			return result;
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue value = obj.GetValue();
			
			// if we're not mixing colors, we need to compare the input color against the filter color -- but only the
			// hue and saturation components; ignore the intensity.
			if (Math.Abs(value.FullColor.R - _breakdownItem.Color.R) < Tolerance &&
			        Math.Abs(value.FullColor.G - _breakdownItem.Color.G) < Tolerance &&
			        Math.Abs(value.FullColor.B - _breakdownItem.Color.B) < Tolerance)
			{
				var i = HSV.VFromRgb(value.FullColor);
				//the value types are structs, so modify our copy and then set it back
				_intensityValue = i;
			} else {
				_intensityValue = 0;
			}
			
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			
			// if we're not mixing colors, we need to compare the input color against the filter color -- but only the
			// hue and saturation components; ignore the intensity.
			if (Math.Abs(lightingValue.Color.R - _breakdownItem.Color.R) < Tolerance &&
				Math.Abs(lightingValue.Color.G - _breakdownItem.Color.G) < Tolerance &&
				Math.Abs(lightingValue.Color.B - _breakdownItem.Color.B) < Tolerance) {
				_intensityValue = lightingValue.Intensity;
			}
			else {
				_intensityValue = 0;
			}
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			DiscreteValue discreteValue = obj.GetValue();

			// if we're not mixing colors, we need to compare the input color against the filter color -- but only the
			// hue and saturation components; ignore the intensity.
			if (discreteValue.Color.ToArgb() == _breakdownItem.Color.ToArgb())
			{
				_intensityValue = discreteValue.Intensity;
			}
			else
			{
				_intensityValue = 0;
			}
		}
	}

	internal enum MixingChannel
	{
		Red,
		Green,
		Blue
	}

	/// <summary>
	/// This filter gets the intensity percent for a given state in simple RGB mixing
	/// </summary>
	internal class ColorBreakdownMixingFilter : IntentStateDispatch, IBreakdownFilter
	{
		private double _intensityValue;
		private readonly MixingChannel _mixingChannel;
		
		public ColorBreakdownMixingFilter(ColorBreakdownItem breakdownItem)
		{
			if (breakdownItem.Color.Equals(Color.Red))
			{
				_mixingChannel = MixingChannel.Red;
			}
			else if(breakdownItem.Color.Equals(Color.Lime))
			{
				_mixingChannel = MixingChannel.Green;
			}
			else
			{
				_mixingChannel = MixingChannel.Blue;
			}
		}

		/// <summary>
		/// Process the intent and return a value that represents the percent of intensity for the state
		/// </summary>
		/// <param name="intentValue"></param>
		/// <returns></returns>
		public double GetIntensityForState(IIntentState intentValue)
		{
			intentValue.Dispatch(this);
			return _intensityValue;
		}

		private float _getMaxProportion(Color inputColor)
		{
			switch (_mixingChannel)
			{
				case MixingChannel.Red:
					return (float)inputColor.R / 255;
				case MixingChannel.Green:
					return (float)inputColor.G / 255;
				case MixingChannel.Blue:
					return (float)inputColor.B / 255;
				default:
					return 0;
					
			}
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue value = obj.GetValue();
			_intensityValue = _getMaxProportion(value.FullColor);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			_intensityValue = lightingValue.Intensity * _getMaxProportion(lightingValue.Color);
		}

	}




	internal class ColorBreakdownOutput : IDataFlowOutput<IntentDataFlowData>
	{
		private readonly IBreakdownFilter _filter;
		private readonly ColorBreakdownItem _breakdownItem;
		private readonly StaticIntentState<IntensityValue> _state = new StaticIntentState<IntensityValue>(new IntensityValue());
		private readonly IntentDataFlowData _data;

		public ColorBreakdownOutput(ColorBreakdownItem breakdownItem, bool mixColors)
		{
			_data = new IntentDataFlowData(_state);
			if (mixColors)
			{
				_filter = new ColorBreakdownMixingFilter(breakdownItem);
			}
			else
			{
				_filter = new ColorBreakdownFilter(breakdownItem);
			}
			
			_breakdownItem = breakdownItem;
		}

		public void ProcessInputData(IntentsDataFlowData data)
		{
			//Because we are combining at the layer above us, we should really only have one
			//intent that matches this outputs color setting. 
			//Everything else will have a zero intensity and should be thrown away when it does not match our outputs color.
			double intensity = 0;
			if (data.Value?.Count > 0)
			{
				foreach (var intentState in data.Value)
				{
					var i = _filter.GetIntensityForState(intentState);
					if (i > 0)
					{
						intensity = i;
					}
				}
			}

			if (intensity > 0)
			{
				//Get a ref to the state value which is a struct and update it with the new intensity
				ref IntensityValue intensityValue = ref _state.GetValueRef();
				intensityValue.Intensity = intensity;
				_data.Value = _state;
			}
			else
			{
				_data.Value = null;
			}
			
		}

		public IntentDataFlowData Data => _data;

		IDataFlowData IDataFlowOutput.Data => _data;

		public string Name
		{
			get { return _breakdownItem.Name; }
		}
	}
}