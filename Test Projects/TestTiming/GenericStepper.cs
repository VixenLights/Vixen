using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Timing;

namespace TestTiming {
	public class GenericStepper : TimingModuleInstanceBase {
		private GenericStepperForm _form;

		override public void Start() {
			_form = new GenericStepperForm();
			_form.Show();
		}

		override public void Stop() {
			_DestroyForm();
		}

		override public void Pause() { }

		override public void Resume() { }

		override public long Position {
			get { return _form.Position; }
			set { _form.Position = value; }
		}

		override public void Dispose() {
			_DestroyForm();
			GC.SuppressFinalize(this);
		}

		~GenericStepper() {
			_DestroyForm();
		}

		private void _DestroyForm() {
			if(_form != null) {
				_form.Hide();
				_form.Dispose();
				_form = null;
			}
		}
	}
}
