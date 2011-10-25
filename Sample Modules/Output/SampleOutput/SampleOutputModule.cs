using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Commands;

namespace SampleOutput {
	public class SampleOutputModule : OutputModuleInstanceBase {
		private SampleOutputData _data;
		private DisplayForm _form;

		public SampleOutputModule() {
			_form = new DisplayForm();
		}

		protected override void _SetOutputCount(int outputCount) {
			_form.OutputCount = outputCount;
		}

		protected override void _UpdateState(Command[] outputStates) {
			_form.UpdateState(outputStates);
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as SampleOutputData; }
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override bool Setup() {
			bool result;

			using(SampleOutputSetupForm sampleOutputSetupForm = new SampleOutputSetupForm(_data)) {
				result = sampleOutputSetupForm.ShowDialog() == DialogResult.OK;
			}

			return result;
		}

		public override void Start() {
			_data.RunCount++;
			_form.Show();
		}

		public override void Stop() {
			_data.LastStartDate = DateTime.Now;
			_form.Hide();
		}

		public override bool IsRunning {
			get { return _form != null && _form.Visible; }
		}

		public override void Dispose() {
			_form.Dispose();
			_form = null;
			GC.SuppressFinalize(this);
		}

		~SampleOutputModule() {
			_form = null;
		}
	}
}
