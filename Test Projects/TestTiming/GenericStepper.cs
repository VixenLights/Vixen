using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module;
using Vixen.Module.Timing;

namespace TestTiming {
	public class GenericStepper : ITimingModuleInstance {
		private long _timingOffset = 0;
		private GenericStepperForm _form;

		public void SetOutputCount(int outputCount) { }

		public void UpdateState(CommandData[] outputStates) { }

		public void Initialize(long startTime) {
			_timingOffset = startTime;
		}

		public void Start() {
			_form = new GenericStepperForm();
			_form.Show();
		}

		public void Stop() {
			_DestroyForm();
		}

		public void Pause() { }

		public void Resume() { }

		private void _DestroyForm() {
			if(_form != null) {
				_form.Hide();
				_form.Dispose();
				_form = null;
			}
		}

		public Guid TypeId {
			get { return GenericStepperModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		public void Dispose() {
			_DestroyForm();
			GC.SuppressFinalize(this);
		}

		~GenericStepper() {
			_DestroyForm();
		}

		public long Position {
			get { return _form.Position; }
			set { _form.Position = value; }
		}
	}
}
