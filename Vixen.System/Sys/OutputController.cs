using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Module.Transform;
using Vixen.Commands;

namespace Vixen.Sys {
	public class OutputController : IEnumerable<OutputController>, Vixen.Execution.IExecutionControl {
		private Guid _outputModuleId;
		private IOutputModuleInstance _outputModule;
		private List<Output> _outputs = new List<Output>();
		private ModuleInstanceSpecification<int> _outputTransforms = new ModuleInstanceSpecification<int>();
		private IModuleDataSet _moduleDataSet = new ModuleLocalDataSet();

		public OutputController(string name, int outputCount, Guid outputModuleId)
			: this(Guid.NewGuid(), Guid.NewGuid(), name, outputCount, outputModuleId) {
		}

		public OutputController(Guid id, Guid instanceId, string name, int outputCount, Guid outputModuleId) {
			Id = id;
			InstanceId = instanceId;
			Name = name;
			OutputModuleId = outputModuleId;
			OutputCount = outputCount;
		}

		public void Start() {
			if(!IsRunning && OutputModule != null) {
				OutputModule.Start();
			}
		}

		public void Pause() {
			if(IsRunning && OutputModule != null) {
				OutputModule.Pause();
			}
		}

		public void Resume() {
			if(IsRunning && OutputModule != null) {
				OutputModule.Resume();
			}
		}

		public void Stop() {
			if(IsRunning && OutputModule != null) {
				OutputModule.Stop();
			}
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
					_SetOutputModuleTransforms();
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

		private void _SetOutputModuleTransforms() {
			if(_outputModule != null) {
				// _outputTransforms is an index of transforms for a given output.
				// Create transforms of the types in the tuple (Item1) and give them
				// the specified instance id (Item2).
				foreach(int outputIndex in _outputTransforms.Keys) {
					IEnumerable<ITransformModuleInstance> outputTransforms =
						_outputTransforms[outputIndex].Select(x => {
							ITransformModuleInstance transformModule = Modules.ModuleManagement.GetTransform(x.Item1);
							transformModule.InstanceId = x.Item2;
							return transformModule;
						});
					_outputModule.SetTransforms(outputIndex, outputTransforms);
				}
			}
		}

		public ModuleInstanceSpecification<int> OutputTransforms {
			get { return _outputTransforms; }
			set {
				_outputTransforms = value;
				_SetOutputModuleTransforms();
			}
		}

		public IModuleDataSet ModuleDataSet {
			get { return _moduleDataSet; }
			set {
				_moduleDataSet = value;
				_SetModuleData();
			}
		}

		public void Update() {
			// Updates start at the root controllers and cascade from there.
			// Non-root controllers are not directly updated; they are only updated
			// from a root controller.
			if(IsRootController && _ControllerChainOutputModule != null) {
				// States need to be pulled in order, so we're getting them to update
				// in parallel with no need to properly collate the results, then iterating
				// the output in order.

				// Get the outputs of all controllers in the chain to update their state.
				Parallel.ForEach(this, x =>
					Parallel.ForEach(x._outputs, y => y.UpdateState())
					);
				// Latch out the new state.
				// This must be done in order of the chain links so that data
				// goes out the port in the correct order.
				foreach(OutputController controller in this) {
					// A single port may be used to service multiple physical controllers,
					// such as daisy-chained Renard controllers.  Tell the module where
					// it is in that chain.
					controller._ControllerChainOutputModule.ChainIndex = controller.ChainIndex;
					controller._ControllerChainOutputModule.UpdateState(controller._outputs.Select(x => x.CurrentState).ToArray());
				}
			}
		}

		public OutputController Clone() {
			// Doing a MemberwiseClone does NOT call the constructor.
			OutputController controller = this.MemberwiseClone() as OutputController;

			// Wipe out instance link references or the stale references will prevent links.
			controller.Prior = null;
			controller.Next = null;
			controller._outputs = this._outputs.Select(x => new Output(this)).ToList();

			if(_outputModule != null) {
				controller._outputModule = Modules.ModuleManagement.GetOutput(_outputModule.Descriptor.TypeId);
			}

			controller.InstanceId = Guid.NewGuid();

			return controller;
		}

		public Output[] Outputs {
			get { return _outputs.ToArray(); }
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
						Output output = new Output(this);
						_outputs.Add(output);
					}
				}

				_SetOutputModuleOutputCount();
			}
		}

		/// <summary>
		/// States if this output controller instance can be a child of the specified output controller.
		/// </summary>
		/// <param name="otherController"></param>
		/// <returns></returns>
		public bool CanLinkTo(OutputController otherController) {
			// A controller can link to a parent controller if:
			// The other controller doesn't already have a child link.

			// If the other controller is null, they're trying to break the link so pass
			// it through.
			return
				otherController == null ||
				otherController.Next == null;
		}

		/// <summary>
		/// Links the output controller to another output controller.
		/// </summary>
		/// <param name="controller"></param>
		/// <returns>True if controller could be successfully linked.</returns>
		public bool LinkTo(OutputController controller) {
			if(CanLinkTo(controller)) {
				if(Prior != null) {
					Prior.Next = null;
				}
				
				Prior = controller;
				
				if(Prior != null) {
					Prior.Next = this;
				}

				ChainIndex = _GetChainIndex();
				return true;
			}
			return false;
		}

		public Guid LinkedTo { get; set; }
		public int ChainIndex { get; private set; }

		public bool IsRootController {
			get { return Prior == null && LinkedTo == Guid.Empty; }
		}

		private int _GetChainIndex() {
			int count = 0;
			OutputController controller = this;

			while(controller.Prior != null) {
				count++;
				controller = controller.Prior;
			}

			return count;
		}

		private IOutputModuleInstance _ControllerChainOutputModule {
			get {
				// When output controllers are linked, only the root controller will be
				// connected to the port, therefore only it will have the output module
				// used during execution.
				if(Prior != null) return Prior._ControllerChainOutputModule;
				return _outputModule;
			}
		}

		virtual protected void CommitState() {
			if(Prior != null) {
				LinkedTo = Prior.Id;
			} else {
				LinkedTo = Guid.Empty;
			}
		}

		public bool HasSetup {
			get { return _outputModule.HasSetup; }
		}

		/// <summary>
		/// Runs the controller setup and commits it upon success.
		/// </summary>
		/// <returns>True if the setup was successful and committed.  False if the user canceled.</returns>
		public bool Setup() {
			if(_outputModule != null) {
				if(_outputModule.Setup()) {
					Commit();
					return true;
				}
			}
			return false;
		}

		public void Commit() {
			// The data set that the data model was pulled from has a reference to the data
			// model object and pulls it in upon Serialize.  So it's serialized when its
			// container is saved.
			// Commit derivative changes.
			CommitState();
		}

		public bool AddSource(IOutputStateSource source, int outputIndex) {
			if(outputIndex < OutputCount) {
				return _outputs[outputIndex].AddSource(source);
			}
			return false;
		}

		public void RemoveSource(IOutputStateSource source, int outputIndex) {
			if(outputIndex < OutputCount) {
				_outputs[outputIndex].RemoveSource(source);
			}
		}

		// Must be a property for data binding.
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid InstanceId { get; private set; }

		public OutputController Prior { get; private set; }

		public OutputController Next { get; private set; }

		public bool IsRunning {
			get { return _outputModule != null && _outputModule.IsRunning; }
		}

		public int UpdateInterval {
			get { return OutputModule.UpdateInterval; }
		}

		public override string ToString() {
			return Name;
		}

		#region IEnumerable<OutputController>
		public IEnumerator<OutputController> GetEnumerator() {
			if(IsRootController) {
				return new ChainEnumerator(this);
			}
			return null;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion

		#region class Output
		public class Output {
			private OutputController _owner;
			private LinkedList<IOutputStateSource> _sources = new LinkedList<IOutputStateSource>();

			public Output(OutputController owner) {
				_owner = owner;
				CurrentState = null;
				Name = "Unnamed";
			}

			// Completely independent; nothing is current dependent upon this value.
			public string Name { get; set; }

			public bool AddSource(IOutputStateSource source) {
				if(!_sources.Contains(source)) {
					_sources.AddLast(source);
					return true;
				}
				return false;
			}

			public void RemoveSource(IOutputStateSource source) {
				_sources.Remove(source);
			}

			public void ClearSources() {
				_sources.Clear();
			}

			public void UpdateState() {
				// Aggregate a state.
				if(_sources.Count > 0) {

					if(_sources.Count == 1) {
						CurrentState = _sources.First.Value.SourceState;
					} else {
						CurrentState = Command.Combine(_sources.Select(x => x.SourceState));
					}
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
					_next = _current.Next;
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
