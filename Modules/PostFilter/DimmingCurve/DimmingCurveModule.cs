using System;
using System.Collections.Generic;
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
using VixenModules.App.Curves;

namespace VixenModules.OutputFilter.DimmingCurve
{
	public class DimmingCurveDescriptor : OutputFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{2e40d6b1-43d2-4668-b63a-c600fadd7dd5}");

		public override string TypeName
		{
			get { return "Dimming Curve"; }
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
			get { return typeof (DimmingCurveModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (DimmingCurveData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get
			{
				return
					"An output filter that translates lighting intensity values according to a curve (to compensate for non-linear lighting response).";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}


	public class DimmingCurveModule : OutputFilterModuleInstanceBase
	{
		private DimmingCurveData _data;
		private DimmingCurveOutput _output;

		public override void Handle(IntentsDataFlowData obj)
		{
			_output.ProcessInputData(obj);
		}

		public override void Handle(IntentDataFlowData obj)
		{
			_output.ProcessInputData(obj);
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
			get { return new IDataFlowOutput[] {_output}; }
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (DimmingCurveData) value;
				_CreateOutputs();
			}
		}

		public Curve DimmingCurve
		{
			get { return _data.Curve; }
			set { _data.Curve = value; }
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (CurveEditor editor = new CurveEditor(_data.Curve)) {
				if (editor.ShowDialog() == DialogResult.OK) {
					DimmingCurve = editor.Curve;
					_CreateOutputs();
					return true;
				}
			}
			return false;
		}

		private void _CreateOutputs()
		{
			_output = new DimmingCurveOutput(_data.Curve);
		}
	}


	public class DimmingCurveData : ModuleDataModelBase
	{
		public DimmingCurveData()
		{
			Curve = new Curve();
		}

		public override IModuleDataModel Clone()
		{
			DimmingCurveData newInstance = new DimmingCurveData();
			newInstance.Curve = new Curve(Curve);
			return newInstance;
		}

		[DataMember]
		public Curve Curve { get; set; }
	}


	internal class DimmingCurveFilter : IntentStateDispatch
	{
		private IIntentState _intentValue;
		private readonly Curve _curve;

		public DimmingCurveFilter(Curve curve)
		{
			_curve = curve;
		}

		public IIntentState Filter(IIntentState intentValue)
		{
			intentValue.Dispatch(this);
			return _intentValue;
		}

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			double newIntensity = _curve.GetValue(lightingValue.Intensity * 100.0) / 100.0;
			_intentValue = new StaticIntentState<LightingValue>(new LightingValue(lightingValue, newIntensity));
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue rgbValue = obj.GetValue();
			HSV hsv = HSV.FromRGB(rgbValue.FullColor);
			double newIntensity = _curve.GetValue(rgbValue.Intensity * 100.0) / 100.0;
			hsv.V = newIntensity;
			_intentValue = new StaticIntentState<RGBValue>(new RGBValue(hsv.ToRGB()));
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			DiscreteValue discreteValue = obj.GetValue();
			double newIntensity = _curve.GetValue(discreteValue.Intensity * 100.0) / 100.0;
			_intentValue = new StaticIntentState<DiscreteValue>(new DiscreteValue(discreteValue.Color, newIntensity));
		}

		public override void Handle(IIntentState<IntensityValue> obj)
		{
			IntensityValue intensityValue = obj.GetValue();
			double newIntensity = _curve.GetValue(intensityValue.Intensity * 100.0) / 100.0;
			_intentValue = new StaticIntentState<IntensityValue>(new IntensityValue(newIntensity));
		}
	}


	internal class DimmingCurveOutput : IDataFlowOutput<IntentsDataFlowData>
	{
		private readonly Curve _curve;
		private readonly DimmingCurveFilter _filter;
		private static readonly List<IIntentState> EmptyState = Enumerable.Empty<IIntentState>().ToList(); 

		public DimmingCurveOutput(Curve curve)
		{
			Data = new IntentsDataFlowData(Enumerable.Empty<IIntentState>().ToList());
			_curve = curve;
			_filter = new DimmingCurveFilter(_curve);
		}

		public void ProcessInputData(IntentsDataFlowData data)
		{
			//Very important!!! 
			//using foreach here instead of linq to reduce iterator allocations
			//If we had better control over our update cycle, we could possibly eliminate the new list.
			if (data.Value.Count > 0)
			{
				var states = new List<IIntentState>(data.Value.Count);
				foreach (var intentState in data.Value)
				{
					states.Add(_filter.Filter(intentState));
				}

				Data.Value = states;
			}
			else
			{
				Data.Value = EmptyState;
			}
		}

		public void ProcessInputData(IntentDataFlowData data)
		{
			var states = new List<IIntentState>(1);
			states.Add(_filter.Filter(data.Value));
			Data.Value = states;
		}

		public IntentsDataFlowData Data { get; private set; }

	
		IDataFlowData IDataFlowOutput.Data
		{
			get { return Data; }
		}

		public string Name
		{
			get { return "Dimming Curve Output"; }
		}

	}
}