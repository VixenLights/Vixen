using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Module.PostFilter;
using Vixen.Commands;

namespace Vixen.Sys.Output {
	public class OutputController : OutputDeviceBase, IEnumerable<OutputController> {
		private Guid _outputModuleId;
		private IOutputModuleInstance _outputModule;
		private List<Output> _outputs = new List<Output>();
		private ModuleLocalDataSet _moduleDataSet = new ModuleLocalDataSet();
		private Output[] _outputArray = new Output[0];

		public OutputController(string name, int outputCount, Guid outputModuleId)
			: this(Guid.NewGuid(), name, outputCount, outputModuleId) {
		}

		public OutputController(Guid id, string name, int outputCount, Guid outputModuleId) {
			Id = id;
			Name = name;
			OutputModuleId = outputModuleId;
			OutputCount = outputCount;
		}

		override protected void _Start() {
			OutputModule.Start();
		}

		override protected void _Stop() {
			OutputModule.Stop();
		}

		// Must be a property for data binding.
		public Guid OutputModuleId {
			get { return _outputModuleId; }
			set {
				_outputModuleId = value;
				_outputModule = null;
			}
		}

		public IOutputModuleInstance OutputModule {
			get {
				if(_outputModule == null) {
					_outputModule = Modules.ModuleManagement.GetOutput(_outputModuleId);

					_SetOutputModuleOutputCount();
					_SetModuleData();
				}
				return _outputModule;
			}
		}

		private void _SetOutputModuleOutputCount() {
			if(_outputModule != null && OutputCount != 0) {
				_outputModule.OutputCount = OutputCount;
			}
		}

		private void _SetModuleData() {
			// Normally, we'd be responsible for providing the module with its
			// data object and the module wouldn't have to worry about it, but
			// the output module is going to store transform module data in the
			// same dataset, so we'll give it the dataset and let it handle both.
			if(_outputModule != null) {
				_outputModule.ModuleDataSet = ModuleDataSet;
			}
		}

		public ModuleLocalDataSet ModuleDataSet {
			get { return _moduleDataSet; }
			set {
				_moduleDataSet = value;
				_SetModuleData();
			}
		}

		override protected void _UpdateState() {
			if(VixenSystem.ControllerLinking.IsRootController(this) && _ControllerChainOutputModule != null) {
				lock(_outputs) {
					foreach(OutputController controller in this) {
						// All outputs for a controller update in parallel.
						Parallel.ForEach(controller._outputs, x =>
						                                      	{
						                                      		x.UpdateState();
						                                      		// (User may be allowed to skip this step in the future).
						                                      		x.FilterState();
						                                      		//*** don't like Output.Command
						                                      		x.Command = _GenerateCommand(x.State);
						                                      	});
					}

					// Latch out the new state.
					// This must be done in order of the chain links so that data
					// goes out the port in the correct order.
					foreach(OutputController controller in this) {
						// A single port may be used to service multiple physical controllers,
						// such as daisy-chained Renard controllers.  Tell the module where
						// it is in that chain.
						controller._ControllerChainOutputModule.ChainIndex = VixenSystem.ControllerLinking.GetChainIndex(controller.Id);

						ICommand[] outputStates = controller._outputs.Select(x => x.Command).ToArray();
						controller._ControllerChainOutputModule.UpdateState(outputStates);
					}
				}
			}
		}

		private ICommand _GenerateCommand(IEnumerable<IIntentState> outputState) {
			return OutputModule.DataPolicy.GenerateCommand(outputState);
		}

		public Output[] Outputs {
			get { return _outputArray; }
		}

		public int OutputCount {
			get { return _outputs.Count; }
			set {
				// Adjust the outputs list.
				lock(_outputs) {
					if(value < _outputs.Count) {
						_outputs.RemoveRange(value, _outputs.Count - value);
					} else {
						while(_outputs.Count < value) {
							// Create a new output.
							Output output = new Output();
							_outputs.Add(output);
						}
					}
				}

				_outputArray = _outputs.ToArray();

				_SetOutputModuleOutputCount();
			}
		}

		private IOutputModuleInstance _ControllerChainOutputModule {
			get {
				// When output controllers are linked, only the root controller will be
				// connected to the port, therefore only it will have the output module
				// used during execution.
				OutputController priorController = VixenSystem.Controllers.GetPrior(this);
				return (priorController != null) ? priorController._ControllerChainOutputModule : _outputModule;
			}
		}

		override public bool HasSetup {
			get { return _outputModule.HasSetup; }
		}

		/// <summary>
		/// Runs the controller setup.
		/// </summary>
		/// <returns>True if the setup was successful and committed.  False if the user canceled.</returns>
		override public bool Setup() {
			if(_outputModule != null) {
				if(_outputModule.Setup()) {
					//Commit();
					return true;
				}
			}
			return false;
		}

		public void AddSource(IOutputStateSource source, int outputIndex) {
			if(source != null && outputIndex < OutputCount) {
				_outputs[outputIndex].AddSource(source);
			}
		}

		public void RemoveSource(IOutputStateSource source, int outputIndex) {
			if(source != null && outputIndex < OutputCount) {
				_outputs[outputIndex].RemoveSource(source);
			}
		}

		public void AddPostFilter(int outputIndex, IPostFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				// Must be the controller store, and not the system store, because the system store
				// deals only with static data and there may be multiple instances of a type of filter.
				ModuleDataSet.AssignModuleInstanceData(filter);
				_outputs[outputIndex].AddPostFilter(filter);
			}
		}

		public void InsertPostFilter(int outputIndex, int index, IPostFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				ModuleDataSet.AssignModuleInstanceData(filter);
				_outputs[outputIndex].InsertPostFilter(index, filter);
			}
		}

		public void RemovePostFilter(int outputIndex, IPostFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				ModuleDataSet.RemoveModuleInstanceData(filter);
				_outputs[outputIndex].RemovePostFilter(filter);
			}
		}

		public void ClearPostFilters(int outputIndex) {
			if(outputIndex < OutputCount) {
				foreach(IPostFilterModuleInstance filter in _outputs[outputIndex].PostFilters) {
					RemovePostFilter(outputIndex, filter);
				}
			}
		}

		override public bool IsRunning {
			get { return _outputModule != null && _outputModule.IsRunning; }
		}

		override public int UpdateInterval {
			get { return OutputModule.UpdateInterval; }
		}

		public override string ToString() {
			return Name;
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

		#region class Output
		public class Output : IHasPostFilters, IHasOutputSources {
			private LinkedList<IOutputStateSource> _sources;
			private PostFilterCollection _postFilters;
			private OutputIntentStateList _state;

			public Output() {
				Name = "Unnamed";
				_postFilters = new PostFilterCollection();
				_sources = new LinkedList<IOutputStateSource>();
			}

			//temporary
			public ICommand Command;
			// Completely independent; nothing is current dependent upon this value.
			public string Name { get; set; }

			public void AddSource(IOutputStateSource source) {
				if(!_sources.Contains(source)) {
					_sources.AddLast(source);
				}
			}

			public void RemoveSource(IOutputStateSource source) {
				_sources.Remove(source);
			}

			public void ClearSources() {
				_sources.Clear();
			}

			public void AddPostFilter(IPostFilterModuleInstance filter) {
				_postFilters.Add(filter);
			}

			public void InsertPostFilter(int index, IPostFilterModuleInstance filter) {
				_postFilters.Insert(index, filter);
			}

			public void RemovePostFilter(IPostFilterModuleInstance filter) {
				_postFilters.Remove(filter);
			}

			public void ClearPostFilters() {
				_postFilters.Clear();
			}

			public PostFilterCollection PostFilters {
				get { return _postFilters; }
			}

			public void UpdateState() {
				_state = _GetOutputStateData();
			}

			private OutputIntentStateList _GetOutputStateData() {
				IEnumerable<IIntentState> intentStates = _sources.SelectMany(_GetSourceData).NotNull();
				IIntentState[] states = intentStates.ToArray();
				return new OutputIntentStateList(states);
			}

			private IIntentStateList _GetSourceData(IOutputStateSource source) {
				return source.State;
			}

			public void FilterState() {
				_AppendOutputFilters();
			}

			private void _AppendOutputFilters() {
				IEnumerable<IFilterState> filterStates = _postFilters.Select(x => x.CreateFilterState());
				_state.AddFilters(filterStates);
			}

			public IIntentStateList State {
				get { return _state; }
			}

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
