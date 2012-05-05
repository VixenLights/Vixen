using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Data.Policy;
using Vixen.Module.PostFilter;

namespace Vixen.Sys.Output {
	public class CommandOutput : Output, IHasPostFilters {
		private PostFilterCollection _postFilters;
		private ICommand _command;

		public CommandOutput() {
			_postFilters = new PostFilterCollection();
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

		public void AddPostFilter(IPostFilterModuleInstance module) {
			_postFilters.Add(module);
		}

		public void InsertPostFilter(int index, IPostFilterModuleInstance module) {
			_postFilters.Insert(index, module);
		}

		public void RemovePostFilter(IPostFilterModuleInstance module) {
			_postFilters.Remove(module);
		}

		public void RemovePostFilterAt(int index) {
			_postFilters.RemoveAt(index);
		}

		public void ClearPostFilters() {
			_postFilters.Clear();
		}

		// For the interface.
		public IEnumerable<IPostFilterModuleInstance> GetAllPostFilters() {
			return _postFilters;
		}

		// For the serializer
		public PostFilterCollection PostFilters {
			get { return _postFilters; }
		}

		//*** Not yet any way to set this for an output.
		//    It is intended to allow an output to override the controller's data policy.
		public OutputDataPolicy DataPolicy { get; set; }

		private void _FilterState() {
			if(VixenSystem.AllowFilterEvaluation && Command != null) {
				foreach(IPostFilterModuleInstance postFilter in _postFilters) {
					_command = postFilter.Affect(_command);
					if(_command == null) return;
				}
			}
		}
	}
}
