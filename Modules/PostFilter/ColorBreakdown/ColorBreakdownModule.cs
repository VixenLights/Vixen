using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Commands;
using Vixen.Data.Evaluator;
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
			get { return DataFlowType.SingleCommand; }
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
		private string _name;

		public ColorBreakdownItem()
		{
			Color = Color.White;
			Name = "Unnamed";
		}

		[DataMember]
		public Color Color { get; set; }

		[DataMember]
		public string Name
		{
			get => _name;
			set => _name = string.Intern(value);
		}
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

	/// <summary>
	/// This filter gets the intensity percent for a given state in simple RGB mixing
	/// </summary>
	internal class ColorBreakdownMixingFilter : IntentStateDispatch, IBreakdownFilter
	{
		private double _intensityValue;
		
		public ColorBreakdownMixingFilter(ColorBreakdownItem breakdownItem)
		{
			if (breakdownItem.Color.Equals(Color.Red))
			{
				_getMaxProportionFunc = color => color.R / 255f;
			}
			else if(breakdownItem.Color.Equals(Color.Lime))
			{
				_getMaxProportionFunc = color => color.G / 255f;
			}
			else
			{
				_getMaxProportionFunc = color => color.B / 255f;
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

		private readonly Func<Color, float> _getMaxProportionFunc;

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue value = obj.GetValue();
			_intensityValue = _getMaxProportionFunc(value.FullColor);
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			_intensityValue = lightingValue.Intensity * _getMaxProportionFunc(lightingValue.Color);
		}

	}




	internal class ColorBreakdownOutput : IDataFlowOutput<CommandDataFlowData>
	{
		private readonly IBreakdownFilter _filter;
		private readonly ColorBreakdownItem _breakdownItem;

		public ColorBreakdownOutput(ColorBreakdownItem breakdownItem, bool mixColors)
		{
			Data = new CommandDataFlowData(CommandLookup8BitEvaluator.CommandLookup[0]); ;
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
					intensity = Math.Max(i, intensity);
				}
			}

			Data.Value = CommandLookup8BitEvaluator.CommandLookup[(byte)(intensity * Byte.MaxValue)];
		}

		IDataFlowData IDataFlowOutput.Data => Data;

		public string Name
		{
			get { return _breakdownItem.Name; }
		}

		/// <inheritdoc />
		public CommandDataFlowData Data { get; }
	}
}