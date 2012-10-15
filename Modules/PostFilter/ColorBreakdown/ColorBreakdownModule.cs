using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
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
		private readonly Guid _typeId = new Guid("{ab38a16f-0de1-4f6e-a8c0-ae5b20d347e0}");

		public override string TypeName
		{
			get { return "Color Breakdown"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(ColorBreakdownModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(ColorBreakdownData); }
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
			get { return DataFlowType.MultipleIntents; }
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
				_data = (ColorBreakdownData)value;
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
			_outputs = _data.BreakdownItems.Select(x => new ColorBreakdownOutput(x)).ToArray();
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
			return newInstance;
		}

		[DataMember]
		public List<ColorBreakdownItem> BreakdownItems { get; set; }
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



	class ColorBreakdownFilter : IntentStateDispatch
	{
		private IIntentState _intentValue;
		private readonly ColorBreakdownItem _breakdownItem;

		public ColorBreakdownFilter(ColorBreakdownItem breakdownItem)
		{
			_breakdownItem = breakdownItem;
		}

		public IIntentState Filter(IIntentState intentValue)
		{
			intentValue.Dispatch(this);
			return _intentValue;
		}

		public override void Handle(IIntentState<ColorValue> obj)
		{
			ColorValue colorValue = obj.GetValue();

			float maxProportion = 1.0f;

			if (_breakdownItem.Color.R > 0) {
				maxProportion = Math.Min(maxProportion, (float)colorValue.Color.R / _breakdownItem.Color.R);
			}

			if (_breakdownItem.Color.G > 0) {
				maxProportion = Math.Min(maxProportion, (float)colorValue.Color.G / _breakdownItem.Color.G);
			}

			if (_breakdownItem.Color.B > 0) {
				maxProportion = Math.Min(maxProportion, (float)colorValue.Color.B / _breakdownItem.Color.B);
			}

			Color finalColor = Color.FromArgb((int)(_breakdownItem.Color.R * maxProportion), (int)(_breakdownItem.Color.G * maxProportion), (int)(_breakdownItem.Color.B * maxProportion));
			_intentValue = new StaticIntentState<ColorValue>(obj, new ColorValue(finalColor));
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();

			float maxProportion = 1.0f;
			float intensity;

			// in a probably-misguided-attempt at shortcutting the most commonly used use-cases,
			// we're going to special-case the cases of R/G/B only (eg. #FF0000, #00FF00, and #0000FF).
			// this can potentially avoid some (maybe?) costly floating-point computation later on.
			// (look at me, I sound like I'm coding for a 386 without a floating point co-pro....)
			// if it's ever a case of more than 1 value, or the value isn't exactly a 100% filter,
			// we'll have to do some scaling, so will need to check proportions.
			bool doRed = _breakdownItem.Color.R > 0;
			bool doGreen = _breakdownItem.Color.G > 0;
			bool doBlue = _breakdownItem.Color.B > 0;

			if (doRed && !doGreen && !doBlue && _breakdownItem.Color.R == byte.MaxValue) {
				intensity = (float)lightingValue.Color.R / _breakdownItem.Color.R;
			} else if (!doRed && doGreen && !doBlue && _breakdownItem.Color.G == byte.MaxValue) {
				intensity = (float)lightingValue.Color.G / _breakdownItem.Color.G;
			} else if (!doRed && !doGreen && doBlue && _breakdownItem.Color.B == byte.MaxValue) {
				intensity = (float)lightingValue.Color.B / _breakdownItem.Color.B;
			} else {
				if (doRed)
					maxProportion = Math.Min(maxProportion, (float) lightingValue.Color.R/_breakdownItem.Color.R);
				if (doGreen)
					maxProportion = Math.Min(maxProportion, (float) lightingValue.Color.G/_breakdownItem.Color.G);
				if (doBlue)
					maxProportion = Math.Min(maxProportion, (float) lightingValue.Color.B/_breakdownItem.Color.B);

				intensity = lightingValue.Intensity * maxProportion;
			}

			LightingValue result = new LightingValue(_breakdownItem.Color, intensity);

			_intentValue = new StaticIntentState<LightingValue>(obj, result);
		}
	}


	class ColorBreakdownOutput : IDataFlowOutput<IntentsDataFlowData>
	{
		private readonly ColorBreakdownFilter _filter;
		private readonly ColorBreakdownItem _breakdownItem;

		public ColorBreakdownOutput(ColorBreakdownItem breakdownItem)
		{
			_filter = new ColorBreakdownFilter(breakdownItem);
			_breakdownItem = breakdownItem;
		}

		public void ProcessInputData(IntentsDataFlowData data)
		{
			Data = new IntentsDataFlowData(data.Value.Select(_filter.Filter));
		}

		public IntentsDataFlowData Data { get; private set; }

		IDataFlowData IDataFlowOutput.Data
		{
			get { return Data; }
		}

		public string Name
		{
			get { return _breakdownItem.Name; }
		}
	}
}
