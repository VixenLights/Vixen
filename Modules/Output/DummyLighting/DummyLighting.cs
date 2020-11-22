using System;
using Vixen.Commands;
using Vixen.Module.Controller;
using Vixen.Module;
using System.Diagnostics;
using System.Windows.Forms;
using VixenModules.Controller.DummyLighting;

namespace VixenModules.Output.DummyLighting
{
	public class DummyLighting : ControllerModuleInstanceBase
	{
		private DummyLightingOutputForm _form;
		private Stopwatch _sw;
		private int _updateCount;
		private DummyLightingData _data;

		public DummyLighting()
		{
			_sw = new Stopwatch();
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (DummyLightingData) value;
				_SetDataPolicy();
			}
		}

		public override int OutputCount { get; set; }
		

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (_updateCount++ == 0) {
				_sw.Reset();
				_sw.Start();
			}

			if (_form.OutputCount != OutputCount)
			{
				_form.OutputCount = OutputCount;
			}
			_form.UpdateState(1000*((double) _updateCount/_sw.ElapsedMilliseconds), outputStates);
		}

		public override void Start()
		{
			_form = new DummyLightingOutputForm();
			_form.renderingStyle = _data.RenderStyle;
			_form.Text = _data.FormTitle;
			_form.OutputCount = OutputCount;
			_form.Show();
			_updateCount = 0;
		}

		public override void Stop()
		{
			_form.Hide();
			_sw.Stop();
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			DummyLightingSetup setup = new DummyLightingSetup(_form.renderingStyle, _form.Text);
			DialogResult result = setup.ShowDialog();
			if (result == DialogResult.OK) {
				_data.RenderStyle = setup.RenderStyle;
				_form.renderingStyle = setup.RenderStyle;
				_data.FormTitle = setup.FormTitle;
				_form.Text = setup.FormTitle;
				_SetDataPolicy();
				return true;
			}
			return false;
		}

		public override bool IsRunning
		{
			get { return _form != null && (_form.Visible || _form.IsDisposed); }
		}

		private void _SetDataPolicy()
		{
			switch (_data.RenderStyle) {
				case RenderStyle.Monochrome:
					DataPolicyFactory = new MonochromeDataPolicyFactory();
					break;
				case RenderStyle.RGBSingleChannel:
					DataPolicyFactory = new OneChannelColorDataPolicyFactory();
					break;
				case RenderStyle.RGBMultiChannel:
					DataPolicyFactory = new ThreeChannelColorDataPolicyFactory();
					break;
				default:
					throw new InvalidOperationException("Unknown render style: " + _data.RenderStyle);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!_form.IsDisposed) {
				_form.Dispose();
			}
			_form = null;
			base.Dispose(disposing);
		}

	}
}