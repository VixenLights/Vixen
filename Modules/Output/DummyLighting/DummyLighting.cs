using System;
using Vixen.Commands;
using Vixen.Module.Controller;
using Vixen.Module;
using Vixen.Sys;
using System.Diagnostics;
using System.Windows.Forms;

namespace VixenModules.Output.DummyLighting {
	public class DummyLighting : ControllerModuleInstanceBase {
		//private List<string> _output = new List<string>();
		private DummyLightingOutputForm _form;
		private Stopwatch _sw;
		private int _updateCount;
		private DummyLightingData _data;
		private IDataPolicy _dataPolicy;

		public DummyLighting() {
			_form = new DummyLightingOutputForm();
			_sw = new Stopwatch();
			_dataPolicy = new DataPolicy();
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set {
				_data = (DummyLightingData)value;
				_form.renderingStyle = _data.RenderStyle;
			}
		}

		override protected void _SetOutputCount(int outputCount) {
			_form.OutputCount = outputCount;
		}

		override public void UpdateState(ICommand[] outputStates) {
			if(_updateCount++ == 0) {
				_sw.Reset();
				_sw.Start();
			}

			_form.UpdateState(1000 * ((double)_updateCount / _sw.ElapsedMilliseconds), outputStates);
		}

		public override void Start() {
			//_formThread = new UIThread(() => {
			//    _form = new DummyLightingOutputForm();
			//    _form.renderingStyle = _data.RenderStyle;
			//    _form.OutputCount = outputCount;
			//    return _form;
			//});
			//_formThread.Start();
			_form.Show();
			_updateCount = 0;
		}
		//override public void Start() {
		//    //_formThread.Start();
		//    //_form.Show();
		//    //_updateCount = 0;
		//}

		override public void Stop() {
			//_formThread.Stop();
			_form.Hide();
			_sw.Stop();
		}

		override public bool HasSetup {
			get {
				return true;
			}
		}

		override public bool Setup() {
			DummyLightingSetup setup = new DummyLightingSetup();
			setup.RenderStyle = _form.renderingStyle;
			DialogResult result = setup.ShowDialog();
			if(result == DialogResult.OK) {
				_data.RenderStyle = setup.RenderStyle;
				_form.renderingStyle = setup.RenderStyle;
				return true;
			}
			return false;
		}

		override public bool IsRunning {
			get { return _form != null && (_form.Visible || _form.IsDisposed); }
		}

		override public IDataPolicy DataPolicy {
			get { return _dataPolicy; }
		}

		override public void Dispose() {
			if(!_form.IsDisposed) {
				_form.Dispose();
			}
			_form = null;
			GC.SuppressFinalize(this);
		}

		~DummyLighting() {
			_form = null;
		}
	}
}
