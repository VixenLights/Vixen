using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Output;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Commands;
using System.Diagnostics;
using System.Windows.Forms;

namespace VixenModules.Output.DummyLighting
{
	public class DummyLighting : OutputModuleInstanceBase
	{
		private List<string> _output = new List<string>();
		private DummyLightingOutputForm _form;
		private Stopwatch _sw;
		private int _updateCount;
		private DummyLightingData _data;

		public DummyLighting()
		{
			_form = new DummyLightingOutputForm();
			_sw = new Stopwatch();
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as DummyLightingData;
				_form.renderingStyle = _data.RenderStyle;
			}
		}

		override protected void _SetOutputCount(int outputCount)
		{
			_form.OutputCount = outputCount;
		}

		override protected void _UpdateState(Command[] outputStates)
		{
			if (_updateCount++ == 0) {
				_sw.Reset();
				_sw.Start();
			}

			_form.UpdateState(1000 * ((double)_updateCount / _sw.ElapsedMilliseconds), outputStates);
		}

		override public void Start()
		{
			_form.Show();
			_updateCount = 0;
		}

		override public void Stop()
		{
			_form.Hide();
			_sw.Stop();
		}

		override public bool HasSetup
		{
			get
			{
				return true;
			}
		}

		override public bool Setup()
		{
			DummyLightingSetup setup = new DummyLightingSetup();
			setup.RenderStyle = _form.renderingStyle;
			DialogResult result = setup.ShowDialog();
			if (result == DialogResult.OK) {
				_data.RenderStyle = setup.RenderStyle;
				_form.renderingStyle = setup.RenderStyle;
				return true;
			}
			return false;
		}

		override public bool IsRunning
		{
			get { return _form != null && _form.Visible; }
		}

		override public void Dispose()
		{
			_form.Dispose();
			_form = null;
			GC.SuppressFinalize(this);
		}

		~DummyLighting()
		{
			_form = null;
		}
	}
}
