using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Commands;
using Vixen.Data.Evaluator;
using Vixen.Data.Policy;
using Vixen.Data.Combinator;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Sys;

namespace VixenModules.Output.DebugController
{
	public class DebugControllerDescriptor : ControllerModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("f32bda77-28f0-46c4-a6bd-ba0ba73a1c13");

		public override string TypeName
		{
			get { return "Debugging Values"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(DebugControllerModule); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "A debugging controller that will display raw values in an output window."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
	



	public class DebugControllerModule : ControllerModuleInstanceBase
	{
		private DebugControllerOutputForm _form;

		public DebugControllerModule()
		{
			DataPolicyFactory = new DataPolicyFactory();
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			_form.UpdateState(outputStates);
		}

		public override void Start()
		{
			base.Start();

			if (_form != null) {
				_form.Dispose();
				_form = null;
			}

			_form = new DebugControllerOutputForm();
			_form.Show();
		}

		public override void Stop()
		{
			base.Stop();

			if (_form != null) {
				_form.Hide();
				_form.Dispose();
				_form = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_form != null && !_form.IsDisposed)
				{
					_form.Dispose();
				}
				_form = null;	
			}
		
			base.Dispose(disposing);
		}
	}




	internal class DataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new DataPolicy();
		}
	}




	internal class DataPolicy : ControllerDataPolicy
	{
		protected override IEvaluator GetEvaluator()
		{
			// TODO: really, for the debug controller, we probably want to be able to customize
			// it and select a different evaluator. Defaulting to 8-bit since it's the most common.
			return new _8BitEvaluator();
		}

		protected override ICombinator GetCombinator()
		{
			return new DiscardExcessCombinator();
		}
	}

}
