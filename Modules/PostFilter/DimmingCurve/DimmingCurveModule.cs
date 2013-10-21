<<<<<<< HEAD
﻿using System;
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
		private readonly Guid _typeId = new Guid("{2e40d6b1-43d2-4668-b63a-c600fadd7dd5}");

		public override string TypeName
		{
			get { return "Dimming Curve"; }
		}

		public override Guid TypeId
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

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (CurveEditor editor = new CurveEditor(_data.Curve)) {
				if (editor.ShowDialog() == DialogResult.OK) {
					_data.Curve = editor.Curve;
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
			float newIntensity = _curve.GetValue(lightingValue.Intensity * 100.0f) / 100.0f;
			_intentValue = new StaticIntentState<LightingValue>(obj, new LightingValue(lightingValue.hsv.H, lightingValue.hsv.S, newIntensity));
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue rgbValue = obj.GetValue();
			HSV hsv = HSV.FromRGB(rgbValue.Color);
			float newIntensity = _curve.GetValue(rgbValue.Intensity * 100.0f) / 100.0f;
			hsv.V = newIntensity;
			_intentValue = new StaticIntentState<RGBValue>(obj, new RGBValue(hsv.ToRGB().ToArgb()));
		}
	}


	internal class DimmingCurveOutput : IDataFlowOutput<IntentsDataFlowData>
	{
		private readonly Curve _curve;
		private readonly DimmingCurveFilter _filter;

		public DimmingCurveOutput(Curve curve)
		{
			_curve = curve;
			_filter = new DimmingCurveFilter(_curve);
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
			get { return "Dimming Curve Output"; }
		}
	}
=======
﻿using System;
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
		private readonly Guid _typeId = new Guid("{2e40d6b1-43d2-4668-b63a-c600fadd7dd5}");

		public override string TypeName
		{
			get { return "Dimming Curve"; }
		}

		public override Guid TypeId
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

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (CurveEditor editor = new CurveEditor(_data.Curve)) {
				if (editor.ShowDialog() == DialogResult.OK) {
					_data.Curve = editor.Curve;
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
			float newIntensity = _curve.GetValue(lightingValue.Intensity * 100.0f) / 100.0f;
			_intentValue = new StaticIntentState<LightingValue>(obj, new LightingValue(lightingValue.hsv.H, lightingValue.hsv.S, newIntensity));
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue rgbValue = obj.GetValue();
			HSV hsv = HSV.FromRGB(rgbValue.Color);
			float newIntensity = _curve.GetValue(rgbValue.Intensity * 100.0f) / 100.0f;
			hsv.V = newIntensity;
			_intentValue = new StaticIntentState<RGBValue>(obj, new RGBValue(hsv.ToRGB().ToArgb()));
		}
	}


	internal class DimmingCurveOutput : IDataFlowOutput<IntentsDataFlowData>
	{
		private readonly Curve _curve;
		private readonly DimmingCurveFilter _filter;

		public DimmingCurveOutput(Curve curve)
		{
			_curve = curve;
			_filter = new DimmingCurveFilter(_curve);
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
			get { return "Dimming Curve Output"; }
		}
	}
>>>>>>> parent of 42f78e6... Git insists these need committing even tho nothing has changed
}