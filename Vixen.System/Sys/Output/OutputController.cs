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
		//private ModuleInstanceSpecification<int> _outputTransforms = new ModuleInstanceSpecification<int>();
		private IModuleDataSet _moduleDataSet = new ModuleLocalDataSet();
		private Output[] _outputArray = new Output[0];

		//private CommandStateSourceCollection<int> _outputStates;

		public OutputController(string name, int outputCount, Guid outputModuleId)
			//: this(Guid.NewGuid(), Guid.NewGuid(), name, outputCount, outputModuleId) {
			: this(Guid.NewGuid(), name, outputCount, outputModuleId) {
		}

		//public OutputController(Guid id, Guid instanceId, string name, int outputCount, Guid outputModuleId) {
		public OutputController(Guid id, string name, int outputCount, Guid outputModuleId) {
			Id = id;
			//InstanceId = instanceId;
			Name = name;
			OutputModuleId = outputModuleId;
			OutputCount = outputCount;

			//_outputStates = new CommandStateSourceCollection<int>();
		}

		override protected void _Start() {
			OutputModule.Start();
		}

		//override protected void _Pause() {
		//    OutputModule.Pause();
		//}

		//override protected void _Resume() {
		//    OutputModule.Resume();
		//}

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
					//_SetOutputModuleTransforms();
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

		//private void _SetOutputModuleTransforms() {
		//    if(_outputModule != null) {
		//        // _outputTransforms is an index of transforms for a given output.
		//        // Create transforms of the types in the tuple (Item1) and give them
		//        // the specified instance id (Item2).
		//        foreach(int outputIndex in _outputTransforms.Keys) {
		//            IEnumerable<ITransformModuleInstance> outputTransforms =
		//                _outputTransforms[outputIndex].Select(x => {
		//                    ITransformModuleInstance transformModule = Modules.ModuleManagement.GetTransform(x.Item1);
		//                    transformModule.InstanceId = x.Item2;
		//                    return transformModule;
		//                });
		//            _outputModule.SetTransforms(outputIndex, outputTransforms);
		//        }
		//    }
		//}

		//public ModuleInstanceSpecification<int> OutputTransforms {
		//    get { return _outputTransforms; }
		//    set {
		//        _outputTransforms = value;
		//        _SetOutputModuleTransforms();
		//    }
		//}

		public IModuleDataSet ModuleDataSet {
			get { return _moduleDataSet; }
			set {
				_moduleDataSet = value;
				_SetModuleData();
			}
		}

		//public void Update() {
		//    // First, get what we pull from to update...
		//    Execution.UpdateState();
		//    // Then we update ourselves from that.
		//    _UpdateState();
		//}

		override protected void _UpdateState() {
			if(VixenSystem.ControllerLinking.IsRootController(this) && _ControllerChainOutputModule != null) {
				// All controllers in this chain update in parallel.
				Parallel.ForEach(this, x =>
				    {
				        // All outputs for a controller update in parallel.
				        Parallel.ForEach(x._outputs, y => y.UpdateState());
				        // Apply post-filters to the output states.
						// (User may be allowed to skip this step in the future).
						Parallel.ForEach(x._outputs, y => y.FilterState());
				    });

				// Latch out the new state.
				// This must be done in order of the chain links so that data
				// goes out the port in the correct order.
				foreach(OutputController controller in this) {
					// A single port may be used to service multiple physical controllers,
					// such as daisy-chained Renard controllers.  Tell the module where
					// it is in that chain.
					//controller._ControllerChainOutputModule.ChainIndex = controller.ChainIndex;
					controller._ControllerChainOutputModule.ChainIndex = VixenSystem.ControllerLinking.GetChainIndex(controller.Id);
					Command[] outputStates = controller._outputs.Select(x => x.CurrentState).ToArray();
					controller._ControllerChainOutputModule.UpdateState(outputStates);
				}
			}

			//// Updates start at the root controllers and cascade from there.
			//// Non-root controllers are not directly updated; they are only updated
			//// from a root controller.
			//if(IsRootController && _ControllerChainOutputModule != null) {
			//    // States need to be pulled in order, so we're getting them to update
			//    // in parallel with no need to properly collate the results, then iterating
			//    // the output in order.

			//    // Get the outputs of all controllers in the chain to update their state.
			//    Parallel.ForEach(this, x =>
			//        Parallel.ForEach(x._outputs, y => y.UpdateState())
			//        );
			//    // Latch out the new state.
			//    // This must be done in order of the chain links so that data
			//    // goes out the port in the correct order.
			//    foreach(OutputController controller in this) {
			//        // A single port may be used to service multiple physical controllers,
			//        // such as daisy-chained Renard controllers.  Tell the module where
			//        // it is in that chain.
			//        controller._ControllerChainOutputModule.ChainIndex = controller.ChainIndex;
			//        controller._ControllerChainOutputModule.UpdateState(controller._outputs.Select(x => x.CurrentState).ToArray());
			//    }
			//}
		}

		//public OutputController Clone() {
		//    // Doing a MemberwiseClone does NOT call the constructor.
		//    OutputController controller = (OutputController)MemberwiseClone();

		//    // Wipe out instance link references or the stale references will prevent links.
		//    controller.Prior = null;
		//    controller.Next = null;
		//    controller._outputs = _outputs.Select(x => new Output()).ToList();

		//    if(_outputModule != null) {
		//        controller._outputModule = Modules.ModuleManagement.GetOutput(_outputModule.Descriptor.TypeId);
		//    }

		//    controller.InstanceId = Guid.NewGuid();

		//    return controller;
		//}

		public Output[] Outputs {
			get { return _outputArray; }
		}

		public int OutputCount {
			get { return _outputs.Count; }
			set {
				// Adjust the outputs list.
				if(value < _outputs.Count) {
					_outputs.RemoveRange(value, _outputs.Count - value);
				} else {
					while(_outputs.Count < value) {
						// Create a new output.
						Output output = new Output();
						_outputs.Add(output);
					}
				}

				_outputArray = _outputs.ToArray();

				_SetOutputModuleOutputCount();
			}
		}

		///// <summary>
		///// States if this output controller instance can be a child of the specified output controller.
		///// </summary>
		///// <param name="otherController"></param>
		///// <returns></returns>
		//public bool CanLinkTo(OutputController otherController) {
		//    // A controller can link to a parent controller if:
		//    // The other controller doesn't already have a child link.

		//    // If the other controller is null, they're trying to break the link so pass
		//    // it through.
		//    return
		//        otherController == null ||
		//        otherController.Next == null;
		//}

		///// <summary>
		///// Links the output controller to another output controller.
		///// </summary>
		///// <param name="controller"></param>
		///// <returns>True if controller could be successfully linked.</returns>
		//public bool LinkTo(OutputController controller) {
		//    if(CanLinkTo(controller)) {
		//        if(Prior != null) {
		//            Prior.Next = null;
		//        }
				
		//        Prior = controller;
				
		//        if(Prior != null) {
		//            Prior.Next = this;
		//        }

		//        ChainIndex = _GetChainIndex();
		//        return true;
		//    }
		//    return false;
		//}

		////public Guid LinkedTo { get; set; }
		//public Guid LinkedTo {
		//    get { return (Prior != null) ? Prior.Id : Guid.Empty; }
		//}

		//public int ChainIndex { get; private set; }

		//public bool IsRootController {
		//    get { return Prior == null && LinkedTo == Guid.Empty; }
		//}

		//private int _GetChainIndex() {
		//    int count = 0;
		//    OutputController controller = this;

		//    while(controller.Prior != null) {
		//        count++;
		//        controller = controller.Prior;
		//    }

		//    return count;
		//}

		private IOutputModuleInstance _ControllerChainOutputModule {
			get {
				// When output controllers are linked, only the root controller will be
				// connected to the port, therefore only it will have the output module
				// used during execution.
				//return (Prior != null) ? Prior._ControllerChainOutputModule : _outputModule;
				OutputController priorController = VixenSystem.Controllers.GetPrior(this);
				return (priorController != null) ? priorController._ControllerChainOutputModule : _outputModule;
			}
		}

		//virtual protected void CommitState() {
		//    LinkedTo = (Prior != null) ? Prior.Id : Guid.Empty;
		//}

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

		//public void Commit() {
		//    // The data set that the data model was pulled from has a reference to the data
		//    // model object and pulls it in upon Serialize.  So it's serialized when its
		//    // container is saved.
		//    // Commit derivative changes.
		//    CommitState();
		//}

		public bool AddSource(IStateSource<Command> source, int outputIndex) {
			if(source != null && outputIndex < OutputCount) {
				return _outputs[outputIndex].AddSource(source);
			}
			return false;
		}

		public void RemoveSource(IStateSource<Command> source, int outputIndex) {
			if(source != null && outputIndex < OutputCount) {
				_outputs[outputIndex].RemoveSource(source);
			}
		}

		public void AddPostFilter(int outputIndex, IPostFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				_outputs[outputIndex].AddPostFilter(filter);
			}
		}

		public void AddPostFilters(int outputIndex, IEnumerable<IPostFilterModuleInstance> filters) {
			if(filters != null && !filters.Any(x => x == null) && outputIndex < OutputCount) {
				_outputs[outputIndex].AddPostFilters(filters);
			}
		}

		public void InsertPostFilter(int outputIndex, int index, IPostFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				_outputs[outputIndex].InsertPostFilter(index, filter);
			}
		}

		public void RemovePostFilter(int outputIndex, IPostFilterModuleInstance filter) {
			if(filter != null && outputIndex < OutputCount) {
				_outputs[outputIndex].RemovePostFilter(filter);
			}
		}

		public void RemovePostFilterAt(int outputIndex, int index) {
			if(outputIndex < OutputCount) {
				_outputs[outputIndex].RemovePostFilterAt(index);
			}
		}

		public void ClearPostFilters(int outputIndex) {
			if(outputIndex < OutputCount) {
				_outputs[outputIndex].ClearPostFilters();
			}
		}

		//public Guid Id { get; set; }

		//public string Name { get; set; }

		//public Guid InstanceId { get; private set; }

		//public OutputController Prior { get; private set; }

		//public OutputController Next { get; private set; }

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
		public class Output {
			//private OutputController _owner;
			//private LinkedList<IOutputStateSource> _sources = new LinkedList<IOutputStateSource>();
			private LinkedList<IStateSource<Command>> _sources = new LinkedList<IStateSource<Command>>();
			private CommandStateAggregator _stateAggregator;
			private List<IPostFilterModuleInstance> _postFilters;

			public Output() {
				Name = "Unnamed";
				_stateAggregator = new CommandStateAggregator();
				_postFilters = new List<IPostFilterModuleInstance>();
			}
			//public Output(OutputController owner) {
			//    _owner = owner;
			//    CurrentState = null;
			//    Name = "Unnamed";
			//}

			// Completely independent; nothing is current dependent upon this value.
			public string Name { get; set; }

			public bool AddSource(IStateSource<Command> source) {
				if(!_sources.Contains(source)) {
					_sources.AddLast(source);
					return true;
				}
				return false;
			}

			public void RemoveSource(IStateSource<Command> source) {
				_sources.Remove(source);
			}

			public void ClearSources() {
				_sources.Clear();
			}

			public void AddPostFilter(IPostFilterModuleInstance filter) {
				_postFilters.Add(filter);
			}

			public void AddPostFilters(IEnumerable<IPostFilterModuleInstance> filters) {
				_postFilters.AddRange(filters);
			}

			public void InsertPostFilter(int index, IPostFilterModuleInstance filter) {
				_postFilters.Insert(index, filter);
			}

			public void RemovePostFilter(IPostFilterModuleInstance filter) {
				_postFilters.Remove(filter);
			}

			public void RemovePostFilterAt(int index) {
				_postFilters.RemoveAt(index);
			}

			public void ClearPostFilters() {
				_postFilters.Clear();
			}

			public IEnumerable<IPostFilterModuleInstance> PostFilters {
				get { return _postFilters; }
			}

			public void UpdateState() {
				// Aggregate a state.
				//if(_sources.Count > 0) {

				//    if(_sources.Count == 1) {
				//        CurrentState = _sources.First.Value.SourceState;
				//    } else {
				//        CurrentState = Command.Combine(_sources.Select(x => x.SourceState));
				//    }
				//}
				//Does the first output have the first channel from the context as a source?
				//-> There are no sources.  It should have the live context as a source.
				_stateAggregator.Aggregate(_sources);
				CurrentState = _stateAggregator.Value;
			}

			public void FilterState() {
				foreach(IPostFilterModuleInstance postFilter in _postFilters) {
					CurrentState = postFilter.Affect(CurrentState);
					if(CurrentState == null) break;
				}
			}

			public Command CurrentState { get; private set; }

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

		//private class OutputStateSourceCollection : IStateSourceCollection<int, Command> {
		//    private Dictionary<int, OutputCommandState> _outputCommandStates;

		//    public OutputStateSourceCollection() {
		//        _outputCommandStates = new Dictionary<int, OutputCommandState>();
		//    }

		//    //public OutputStateSourceCollection(int outputCount) {
		//    //    OutputCount = outputCount;
		//    //}

		//    //public int OutputCount {
		//    //    get { return _outputCommandStates.Count; }
		//    //    set { 
		//    //    }
		//    //}

		//    public void SetValue(int key, Command value) {
		//        OutputCommandState outputCommandState;
		//        if(!_outputCommandStates.TryGetValue(key, out outputCommandState)) {
		//            outputCommandState = new OutputCommandState();
		//            _outputCommandStates[key] = outputCommandState;
		//        }
		//        outputCommandState.Value = value;
		//    }

		//    public IStateSource<Command> GetValue(int key) {
		//        OutputCommandState outputCommandState;
		//        _outputCommandStates.TryGetValue(key, out outputCommandState);
		//        return outputCommandState;
		//    }

		//    private class OutputCommandState : IStateSource<Command> {
		//        public Command Value { get; set; }
		//    }
		//}
	}
}
