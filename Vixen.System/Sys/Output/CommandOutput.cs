using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Data.Policy;
using Vixen.Module.OutputFilter;

namespace Vixen.Sys.Output {
	public class CommandOutput : Output, IHasOutputFilters {
		private OutputFilterCollection _filters;
		private ICommand _command;

		public CommandOutput() {
			_filters = new OutputFilterCollection();
		}

		public ICommand Command {
			get { return _command; }
			set {
				if(!Equals(_command, value)) {
					_command = value;
					_FilterState();
				}
			}
		}

		public void AddOutputFilter(IOutputFilterModuleInstance module) {
			_filters.Add(module);
		}

		public void InsertOutputFilter(int index, IOutputFilterModuleInstance module) {
			_filters.Insert(index, module);
		}

		public void RemoveOutputFilter(IOutputFilterModuleInstance module) {
			_filters.Remove(module);
		}

		public void RemoveOutputFilterAt(int index) {
			_filters.RemoveAt(index);
		}

		public void ClearOutputFilters() {
			_filters.Clear();
		}

		// For the interface.
		public IEnumerable<IOutputFilterModuleInstance> GetAllOutputFilters() {
			return _filters;
		}

		// For the serializer
		public OutputFilterCollection OutputFilters {
			get { return _filters; }
		}

		//*** Not yet any way to set this for an output.
		//    It is intended to allow an output to override the controller's data policy.
		public OutputDataPolicy DataPolicy { get; set; }

		private void _FilterState() {
			if(VixenSystem.AllowFilterEvaluation && Command != null) {
				foreach(IOutputFilterModuleInstance filter in _filters) {
					_command = filter.Affect(_command);
					if(_command == null) return;
				}
			}
		}
	}
}
