using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using System.Diagnostics;
using Vixen.Module;
using Vixen.Module.Output;

namespace TestOutput {
	public class Renard : OutputModuleInstanceBase {
	    private List<string> _output = new List<string>();
        private RenardForm _form;
        private Stopwatch _sw;
        private int _updateCount;

        public Renard() {
			_form = new RenardForm();
            _sw = new Stopwatch();
        }

		override public void SetOutputCount(int outputCount) {
            _form.OutputCount = outputCount;
        }

		override public void UpdateState(CommandData[] outputStates) {
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

		override public bool Setup() {
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
