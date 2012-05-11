using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Controller;
using Vixen.Commands;
using Vixen.Module.OutputFilter;

namespace Vixen.Sys.Output {
	public class OutputController : ModuleBasedController<IControllerModuleInstance, CommandOutput>, IEnumerable<OutputController> {
		public OutputController(string name, int outputCount, Guid moduleId)
			: this(Guid.NewGuid(), name, outputCount, moduleId) {
		}

		public OutputController(Guid id, string name, int outputCount, Guid moduleId)
			: base(id, name, outputCount, moduleId) {
		}

		protected override IControllerModuleInstance GetControllerModule(Guid moduleId) {
			IControllerModuleInstance module = Modules.ModuleManagement.GetController(moduleId);
			ResetDataPolicy(module);
			return module;
		}

		public IDataPolicy DataPolicy { get; set; }

		override protected void _UpdateState() {
			if(VixenSystem.ControllerLinking.IsRootController(this) && _ControllerChainModule != null) {
				BeginOutputChange();
				try {
					foreach(OutputController controller in this) {
						controller.UpdateOutputStates(x => x.Command = _GenerateOutputCommand(x));
					}

					// Latch out the new state.
					// This must be done in order of the chain links so that data
					// goes out the port in the correct order.
					foreach(OutputController controller in this) {
						// A single port may be used to service multiple physical controllers,
						// such as daisy-chained Renard controllers.  Tell the module where
						// it is in that chain.
						controller._ControllerChainModule.ChainIndex = VixenSystem.ControllerLinking.GetChainIndex(controller.Id);
						ICommand[] outputStates = controller.ExtractFromOutputs(x => x.Command).ToArray();
						controller._ControllerChainModule.UpdateState(outputStates);
					}
				} finally {
					EndOutputChange();
				}
			}
		}

		private IControllerModuleInstance _ControllerChainModule {
			get {
				// When output controllers are linked, only the root controller will be
				// connected to the port, therefore only it will have the output module
				// used during execution.
				OutputController priorController = VixenSystem.Controllers.GetPrior(this);
				return (priorController != null) ? priorController._ControllerChainModule : Module;
			}
		}

		public void AddOutputFilter(int outputIndex, IOutputFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				// Must be the controller store, and not the system store, because the system store
				// deals only with static data and there may be multiple instances of a type of filter.
				ModuleDataSet.AssignModuleInstanceData(filter);
				Outputs[outputIndex].AddOutputFilter(filter);
			}
		}

		public void InsertOutputFilter(int outputIndex, int index, IOutputFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				ModuleDataSet.AssignModuleInstanceData(filter);
				Outputs[outputIndex].InsertOutputFilter(index, filter);
			}
		}

		public void RemoveOutputFilter(int outputIndex, IOutputFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				ModuleDataSet.RemoveModuleInstanceData(filter);
				Outputs[outputIndex].RemoveOutputFilter(filter);
			}
		}

		public void ClearOutputFilters(int outputIndex) {
			if(outputIndex < OutputCount) {
				foreach(IOutputFilterModuleInstance filter in Outputs[outputIndex].GetAllOutputFilters().ToArray()) {
					RemoveOutputFilter(outputIndex, filter);
				}
			}
		}

		public void ResetDataPolicy(IControllerModuleInstance module) {
			if(module != null) {
				DataPolicy = module.DataPolicy;
			}
		}

		private ICommand _GenerateOutputCommand(CommandOutput output) {
			IDataPolicy effectiveDataPolicy = _GetOutputEffectiveDataPolicy(output);
			return effectiveDataPolicy.GenerateCommand(output.State);
		}

		private IDataPolicy _GetOutputEffectiveDataPolicy(CommandOutput output) {
			return output.DataPolicy ?? DataPolicy;
		}

		#region IEnumerable<OutputController>
		public IEnumerator<OutputController> GetEnumerator() {
			if(VixenSystem.ControllerLinking.IsRootController(this)) {
				return new ChainEnumerator(this);
			}
			return Enumerable.Empty<OutputController>().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion

		#region class ChainEnumerator
		class ChainEnumerator : IEnumerator<OutputController> {
			private OutputController _root;
			private OutputController _current;
			private OutputController _next;

			public ChainEnumerator(OutputController root) {
				_root = root;
				Reset();
			}

			public OutputController Current {
				get { return _current; }
			}

			public void Dispose() { }

			object System.Collections.IEnumerator.Current {
				get { return _current; }
			}

			public bool MoveNext() {
				if(_next != null) {
					_current = _next;
					//_next = _current.Next;
					_next = VixenSystem.Controllers.GetNext(_current);
					return true;
				}
				return false;
			}

			public void Reset() {
				_current = null;
				_next = _root;
			}
		}
		#endregion
	}
}
