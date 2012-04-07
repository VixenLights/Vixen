using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using System.Diagnostics;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Commands;

namespace TestOutput {
    public class _595 : ControllerModuleInstanceBase {
        private List<string> _output = new List<string>();
        private _595_Form _form;
        private Stopwatch _sw;
        private int _updateCount;

        public _595() {
            _form = new _595_Form();
            _sw = new Stopwatch();
        }

		override protected void _SetOutputCount(int outputCount) {
            _form.OutputCount = outputCount;
        }

		override protected void _UpdateState(Command[] outputStates) {
            if(_updateCount++ == 0) {
                _sw.Reset();
                _sw.Start();
            }
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

		//override public bool Setup() {
		//    return false;
		//}

		override public bool IsRunning {
			get { return _form != null && _form.Visible; }
		}

		override public void Dispose() {
            _form.Dispose();
            _form = null;
			GC.SuppressFinalize(this);
        }

		~_595() {
			_form.Dispose();
			_form = null;
		}
    }
}
