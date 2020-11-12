using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
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
		private DimmingCurveOutput[] _output;

		#region Overrides of OutputFilterModuleInstanceBase

		/// <inheritdoc />
		public override void Handle(CommandDataFlowData obj)
		{
			foreach (var dimmingCurveOutput in _output)
			{
				dimmingCurveOutput.ProcessInputData(obj);
			}
		}

		#endregion

		public override void Handle(IntentsDataFlowData obj)
		{
			foreach (var dimmingCurveOutput in _output)
			{
				dimmingCurveOutput.ProcessInputData(obj);
			}
		}

		public override void Handle(IntentDataFlowData obj)
		{
			foreach (var dimmingCurveOutput in _output)
			{
				dimmingCurveOutput.ProcessInputData(obj);
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
			get { return _output; }
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (DimmingCurveData)value;
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
			using (CurveEditor editor = new CurveEditor(_data.Curve))
			{
				if (editor.ShowDialog() == DialogResult.OK)
				{
					DimmingCurve = editor.Curve;
					_CreateOutputs();
					return true;
				}
			}
			return false;
		}

		private void _CreateOutputs()
		{
			_output = new[] { new DimmingCurveOutput(_data.Curve) };
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
		private readonly StaticIntentState<LightingValue> _lvState = new StaticIntentState<LightingValue>(new LightingValue());
		private readonly StaticIntentState<RGBValue> _rgbState = new StaticIntentState<RGBValue>(new RGBValue());
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

		public ICommand Filter(ICommand command)
		{
			if (command is _8BitCommand cmd)
			{
				double newIntensity = (_curve.GetValue(cmd.CommandValue / 255d) / 100d) * Byte.MaxValue;
				return CommandLookup8BitEvaluator.CommandLookup[(byte) newIntensity];
			}

			return command;
		}

		

		public override void Handle(IIntentState<LightingValue> obj)
		{
			LightingValue lightingValue = obj.GetValue();
			if (lightingValue.Intensity > 0)
			{
				double newIntensity = _curve.GetValue(lightingValue.Intensity * 100.0) / 100.0;
				_lvState.SetValue(new LightingValue(lightingValue, newIntensity));
				_intentValue = _lvState;
			}
			else
			{
				_intentValue = null;
			}
			
		}

		public override void Handle(IIntentState<RGBValue> obj)
		{
			RGBValue rgbValue = obj.GetValue();
			var i = rgbValue.Intensity;
			if (i > 0)
			{
				double newIntensity = _curve.GetValue(i * 100.0) / 100.0;
				HSV hsv = HSV.FromRGB(rgbValue.R, rgbValue.G, rgbValue.B);
				hsv.V = newIntensity;
				_rgbState.SetValue(new RGBValue(hsv.ToRGB()));
				_intentValue = _rgbState;
			}
			else
			{
				_intentValue = null;
			}
		}

		public override void Handle(IIntentState<DiscreteValue> obj)
		{
			DiscreteValue discreteValue = obj.GetValue();
			if (discreteValue.Intensity > 0)
			{
				double newIntensity = _curve.GetValue(discreteValue.Intensity * 100.0) / 100.0;
				if (obj is StaticIntentState<DiscreteValue> state)
				{
					discreteValue.Intensity = newIntensity;
					state.SetValue(discreteValue);
					_intentValue = state;
				}
				else
				{
					_intentValue = new StaticIntentState<DiscreteValue>(new DiscreteValue(discreteValue.Color, newIntensity));
				}
			}
			else
			{
				_intentValue = null;
			}
		}

		public override void Handle(IIntentState<IntensityValue> obj)
		{
			IntensityValue intensityValue = obj.GetValue();
			double newIntensity = _curve.GetValue(intensityValue.Intensity * 100.0) / 100.0;
			if (obj is StaticIntentState<IntensityValue> state)
			{
				intensityValue.Intensity = newIntensity;
				state.SetValue(intensityValue);
				_intentValue = state;
			}
			else
			{
				_intentValue = new StaticIntentState<IntensityValue>(new IntensityValue(newIntensity));
			}
		}
	}


	internal class DimmingCurveOutput : IDataFlowOutput<IntentsDataFlowData>, IDataFlowOutput<CommandDataFlowData>
	{
		private readonly DimmingCurveFilter _filter;
		private readonly List<IIntentState> _states = new List<IIntentState>();
		private readonly CommandDataFlowData _commandState;
		private IntentsDataFlowData _intentData;
		private CommandDataFlowData _data;

		public DimmingCurveOutput(Curve curve)
		{
			_intentData = new IntentsDataFlowData(Enumerable.Empty<IIntentState>().ToList());
			_commandState = new CommandDataFlowData(CommandLookup8BitEvaluator.CommandLookup[0]);
			_filter = new DimmingCurveFilter(curve);
		}

		public void ProcessInputData(IntentsDataFlowData data)
		{
			//Very important!!! 
			//using foreach here instead of linq to reduce iterator allocations
			if (data.Value?.Count > 0)
			{
				//We can use a reuable list here because we are going to be first in the controlled update
				//cycle of the filter chain.
				_states.Clear();
				foreach (var intentState in data.Value)
				{
					var state = _filter.Filter(intentState);
					if (state != null)
					{
						_states.Add(state);
					}
				}

				_intentData.Value = _states;
			}
			else
			{
				_intentData.Value = null;
			}

			InternalData = _intentData;
		}

		public void ProcessInputData(CommandDataFlowData data)
		{
			if (data.Value != null)
			{
				var command = _filter.Filter(data.Value);
				_commandState.Value = command;
			}
			else
			{
				_commandState.Value = CommandLookup8BitEvaluator.CommandLookup[0];
			}

			InternalData = _commandState;
		}

		public void ProcessInputData(IntentDataFlowData data)
		{
			//In this case, we might have a controller consuming the output and that is not
			//predicatble so we can't write to a list that might be accessed at
			//the same time for read. So we are stuck creating a new one. Fortunatly this 
			//Should be a little used use case where the dimming curve is last.
			if (data.Value != null)
			{
				var state = _filter.Filter(data.Value);
				var states = new List<IIntentState>(1);
				if (state != null)
				{
					states.Add(state);
				}
				_intentData.Value = states;
			}
			else
			{
				_intentData.Value = null;
			}
			
		}

		public IDataFlowData InternalData { get; private set; }
		
		public string Name
		{
			get { return "Dimming Curve Output"; }
		}

		/// <inheritdoc />
		IDataFlowData IDataFlowOutput.Data => InternalData;

		/// <inheritdoc />
		public IntentsDataFlowData Data => _intentData;

		/// <inheritdoc />
		CommandDataFlowData IDataFlowOutput<CommandDataFlowData>.Data => _commandState;

	
	}
}