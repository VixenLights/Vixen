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



	class ColorBreakdownFilter : IntentStateDispatch
	{
		private IIntentState _intentValue;
		private readonly ColorBreakdownItem _breakdownItem;
		private readonly bool _mixColors;

		public ColorBreakdownFilter(ColorBreakdownItem breakdownItem, bool mixColors)
		{
			_breakdownItem = breakdownItem;
			_mixColors = mixColors;
		}

		public IIntentState Filter(IIntentState intentValue)
		{
			intentValue.Dispatch(this);
			return _intentValue;
		}

		private float _getMaxProportion(Color inputColor)
		{
			float result = 1.0f;

			if (_breakdownItem.Color.R > 0)
				result = Math.Min(result, (float)inputColor.R / _breakdownItem.Color.R);

			if (_breakdownItem.Color.G > 0)
				result = Math.Min(result, (float)inputColor.G / _breakdownItem.Color.G);

			if (_breakdownItem.Color.B > 0)
				result = Math.Min(result, (float)inputColor.B / _breakdownItem.Color.B);

			return result;
		}

		public override void Handle(IIntentState<ColorValue> obj)
		{
			ColorValue colorValue = obj.GetValue();
			if (_mixColors) {
				float maxProportion = _getMaxProportion(colorValue.Color);
				Color finalColor = Color.FromArgb((int)(_breakdownItem.Color.R * maxProportion), (int)(_breakdownItem.Color.G * maxProportion), (int)(_breakdownItem.Color.B * maxProportion));
				_intentValue = new StaticIntentState<ColorValue>(obj, new ColorValue(finalColor));
			} else {
				if (colorValue.Color.ToArgb() == _breakdownItem.Color.ToArgb()) {
					_intentValue = new StaticIntentState<ColorValue>(obj, colorValue);
				} else {
					// TODO: return 'null', or some osrt of empty intent state here instead. (null isn't handled well, and we don't have an 'empty' state class.)
					_intentValue = new StaticIntentState<ColorValue>(obj, new ColorValue(Color.Black));
				}
			}
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			if (_mixColors) {
				_intentValue = new StaticIntentState<LightingValue>(obj, new LightingValue(_breakdownItem.Color, lightingValue.Intensity * _getMaxProportion(lightingValue.Color)));
			} else {
				if (lightingValue.Color.ToArgb() == _breakdownItem.Color.ToArgb()) {
					_intentValue = new StaticIntentState<LightingValue>(obj, lightingValue);
				} else {
					// TODO: return 'null', or some osrt of empty intent state here instead. (null isn't handled well, and we don't have an 'empty' state class.)
					_intentValue = new StaticIntentState<LightingValue>(obj, new LightingValue(_breakdownItem.Color, 0));
				}
			}
		}
	}


	class ColorBreakdownOutput : IDataFlowOutput<IntentsDataFlowData>
	{
		private readonly ColorBreakdownFilter _filter;
		private readonly ColorBreakdownItem _breakdownItem;
		private readonly bool _mixColors;

		public ColorBreakdownOutput(ColorBreakdownItem breakdownItem, bool mixColors)
		{
			_filter = new ColorBreakdownFilter(breakdownItem, mixColors);
			_breakdownItem = breakdownItem;
			_mixColors = mixColors;
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
