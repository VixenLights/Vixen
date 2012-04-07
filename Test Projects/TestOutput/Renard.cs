using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using System.Diagnostics;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;

namespace TestOutput {
	public class Renard : ControllerModuleInstanceBase {
	    private List<string> _output = new List<string>();
        private RenardForm _form;
        private Stopwatch _sw;
        private int _updateCount;
		//private Command[] lastCommands;
		private RenardData _data;

        public Renard() {
			_form = new RenardForm();
            _sw = new Stopwatch();
			//lastCommands = new Command[0];
        }

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as RenardData;
				_form.renderingStyle = _data.RenderStyle;
			}
		}

		override protected void _SetOutputCount(int outputCount) {
            _form.OutputCount = outputCount;
        }

		override protected void _UpdateState(Command[] outputStates) {
            if(_updateCount++ == 0) {
                _sw.Reset();
                _sw.Start();
            }

			//for (int i = 0; i < outputStates.Length; i++) {
			//    if (i < lastCommands.Length && lastCommands[i] != outputStates[i]) {
			//        if (lastCommands[i] != null && outputStates[i] != null) {
			//            if (lastCommands[i].Identifier == outputStates[i].Identifier)
			//                continue;

			//            if (lastCommands[i].GetParameterValue(0) == outputStates[i].GetParameterValue(0))
			//                continue;
			//        }

			//        string id = "null";
			//        if (outputStates[i] != null)
			//            id = outputStates[i].Identifier;

			//        string pv = "null";
			//        if (outputStates[i] != null)
			//            pv = outputStates[i].GetParameterValue(0).ToString();

			//        lock (VixenSystem.Logging) {
			//            VixenSystem.Logging.Debug(Execution.CurrentExecutionTimeString + ": Renard Update: command for #" + i + " changed: " + id + ", " + pv);
			//        }
			//    }
			//}
			//lastCommands = outputStates;

			_form.UpdateState(1000 * ((double)_updateCount / _sw.ElapsedMilliseconds), outputStates);
        }

		override public void Start() {
            _form.Show();
            _updateCount = 0;
        }

		override public void Stop() {
            _form.Hide();
            _sw.Stop();
        }

		override public bool HasSetup {
			get {
				return true;
			}
		}

		override public bool Setup() {
			RenardSetup setup = new RenardSetup();
			setup.RenderStyle = _form.renderingStyle;
			DialogResult result = setup.ShowDialog();
			if (result == DialogResult.OK) {
				_data.RenderStyle = setup.RenderStyle;
				_form.renderingStyle = setup.RenderStyle;
				return true;
			}
			return false;
        }

		override public bool IsRunning {
			get { return _form != null && _form.Visible; }
		}

		override public void Dispose() {
            _form.Dispose();
            _form = null;
			GC.SuppressFinalize(this);
        }

		~Renard() {
			_form = null;
		}
	}
}
