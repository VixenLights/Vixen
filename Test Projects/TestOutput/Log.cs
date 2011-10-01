using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Commands;

namespace TestOutput {
	public class Log : OutputModuleInstanceBase {
		private bool _running = false;
		private string _filePath = @"C:\Users\Development\Desktop\Log.txt";
		private StreamWriter _file;

		override protected void _SetOutputCount(int outputCount) { }

		override protected void _UpdateState(Command[] outputStates) {
			Command data;
			for(int i = 0; i < outputStates.Length; i++) {
				data = outputStates[i];
				if(data != null) {
					_file.WriteLine("[" + i + "] " + data.Identifier + " ~ " + string.Join(" ", Enumerable.Range(0, data.Signature.Count).Select(x => data.GetParameterValue(x).ToString()).ToArray()));
				}
			}
		}

		override public bool IsRunning {
			get { return _running; }
		}

		override public bool Setup() {
			return false;
		}

		override public void Start() {
			_file = new StreamWriter(_filePath);
			_running = true;
		}

		override public void Stop() {
			_running = false;
			_file.Flush();
			_file.Dispose();
		}
	}
}
