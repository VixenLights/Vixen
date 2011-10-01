using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vixen.Sys;
using Vixen.Module.App;
using Vixen.Commands;

namespace TestAppModule {
	public class ExecutionValueMonitorModule : AppModuleInstanceBase {
		private const bool RUN = false;
		private Queue<ExecutionStateValues> _queue = new Queue<ExecutionStateValues>();

		public override void Loading() {
			if(RUN) {
				Vixen.Sys.Execution.ValuesChanged += new Action<ExecutionStateValues>(Execution_ValuesChanged);
			}
		}

		public override void Unloading() {
			if(RUN) {
				Vixen.Sys.Execution.ValuesChanged -= new Action<ExecutionStateValues>(Execution_ValuesChanged);

				string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				using(StreamWriter writer = new StreamWriter(Path.Combine(desktop, "values.txt"))) {
					int count = _queue.Count;
					for(int i = 1; i <= count; i++) {
						ExecutionStateValues values = _queue.Dequeue();
						writer.WriteLine("[" + i + "/" + count + "] " + values.Time);
						foreach(Command command in values.Values) {
							writer.WriteLine(_FormatCommand(command));
						}
						writer.WriteLine();
					}
				}
			}
		}

		public override IApplication Application {
			set {  }
		}

		private void Execution_ValuesChanged(ExecutionStateValues obj) {
			_queue.Enqueue(obj);
		}

		private string _FormatCommand(Command command) {
			if(command != null) {
				return command.Identifier.ToString() + " " + string.Join(",", Enumerable.Range(0, command.Signature.Count).Select(command.GetParameterValue));
			}
			return "(Empty)";
		}
	}
}
