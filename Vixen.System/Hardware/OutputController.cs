using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.IO;
using Vixen.IO.Xml;

namespace Vixen.Hardware {
	public class OutputController : IEnumerable<OutputController>, IVersioned {
		private Guid _outputModuleId;
		private IOutputModuleInstance _outputModule = null;
		private List<Output> _outputs = new List<Output>();

		// Controller id : Controller
		static private Dictionary<Guid, OutputController> _instances = new Dictionary<Guid, OutputController>();

		private enum ExecutionState { Stopped, Starting, Started, Stopping };
		static private ExecutionState _stateAll = ExecutionState.Stopped;
		static private Dictionary<Guid, HardwareUpdateThread> _updateThreads = new Dictionary<Guid, HardwareUpdateThread>();

		private const string FILE_EXT = ".out";
		private const string DIRECTORY_NAME = "Controller";
		private const int VERSION = 1;

		static public void ReloadAll() {
			// Stop the controllers.
			StopAll();

			// Clear our references.
			_instances.Clear();

			// Reload.
			foreach(string filePath in System.IO.Directory.GetFiles(Directory, "*" + FILE_EXT)) {
				_AddInstance(OutputController.Load(filePath));
			}
		}

		// Taking an id instead of a reference to avoid giving the caller the idea
		// that the reference will still be valid.
		static public void Reload(Guid controllerId) {
			OutputController controller = Get(controllerId);
			if(controller == null) throw new InvalidOperationException("Controller does not exist.");

			// Stop the controller.
			_StopInstance(controller);
			// Remove the instance.
			_RemoveInstance(controller);
			// Reload it.
			controller = OutputController.Load(controller.FilePath);
			// Re-add it.
			_AddInstance(controller);
			// Restart it.
			_StartInstance(controller);
		}

		public OutputController(string name, int outputCount, Guid outputModuleId)
			: this(name, outputCount, outputModuleId, CommandStandard.Standard.CombinationOperation.HighestWins) {
		}

		public OutputController(string name, int outputCount, Guid outputModuleId, CommandStandard.Standard.CombinationOperation combinationStrategy)
			: this(Guid.NewGuid(), Guid.NewGuid(), name, outputCount, outputModuleId, combinationStrategy) {
		}

		public OutputController(Guid id, Guid instanceId, string name, int outputCount, Guid outputModuleId, CommandStandard.Standard.CombinationOperation combinationStrategy = CommandStandard.Standard.CombinationOperation.HighestWins) {
			Id = id;
			InstanceId = instanceId;
			name = _Uniquify(name);
			FilePath = Path.Combine(Directory, Path.ChangeExtension(name, FILE_EXT));
			OutputModuleId = outputModuleId;
			OutputCount = outputCount;
			CombinationStrategy = combinationStrategy;
		}

		public void Save() {
			Save(FilePath);
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");
			filePath = Path.Combine(Directory, Path.GetFileName(filePath));
			IWriter writer = new XmlControllerWriter();
			writer.Write(filePath, this);
			this.FilePath = filePath;
		}

		public string FilePath { get; private set; }

		// Must be a property for data binding.
		public Guid OutputModuleId {
			get { return _outputModuleId; }
			set {
				_outputModuleId = value;
				// Go throught the module type manager always.
				_outputModule = Modules.ModuleManagement.GetOutput(value);
				if(OutputCount != 0) {
					_outputModule.OutputCount = OutputCount;
				}
			}
		}

		public IOutputModuleInstance OutputModule {
			get { return _outputModule; }
		}

		public CommandStandard.Standard.CombinationOperation CombinationStrategy { get; set; }

		[DataPath]
		static public string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		static public OutputController Get(Guid id) {
			return _GetInstance(id);
		}

		static private IEnumerable<OutputController> _GetRootControllers() {
			return _instances.Values.Where(x => x.IsRootController);
		}

		static public void PauseControllers() {
			foreach(OutputController controller in _GetRootControllers()) {
				controller._outputModule.Pause();
			}
		}

		static public void ResumeControllers() {
			foreach(OutputController controller in _GetRootControllers()) {
				controller._outputModule.Resume();
			}
		}

		/// <summary>
		/// Returns the collection of available controllers.  There is no configuration
		/// data loaded for these instances.
		/// </summary>
		/// <returns></returns>
		static public IEnumerable<OutputController> GetAll() {
			return _instances.Values;
		}

		public void Update() {
			// Updates start at the root controllers and cascade from there.
			// Non-root controllers are not directly updated; they are only updated
			// from a previous-linked controller.
			if(IsRootController && _ControllerChainOutputModule != null) {
				// States need to be pulled in order, so we're getting them to update
				// in parallel with no need to properly collate the results, then iterating
				// the output in order.

				// Get the outputs of all controllers in the chain to update their state.
				Parallel.ForEach(this, x =>
					Parallel.ForEach(_outputs, y => y.UpdateState())
					);
				// Latch out the new state.
				// This must be done in order of the chain links so that data
				// goes out the port in the correct order.
				foreach(OutputController controller in this) {
					controller._ControllerChainOutputModule.UpdateState(_outputs.Select(x => x.CurrentState).ToArray());
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
				// Update the hardware.
				if(_outputModule != null) {
					_outputModule.OutputCount = value;
				}
				// Adjust the outputs list.
				if(value < _outputs.Count) {
					_outputs.RemoveRange(value, _outputs.Count - value);
				} else {
					Output output;
					while(_outputs.Count < value) {
						// Create a new output.
						output = new Output(this);
						_outputs.Add(output);
					}
				}
			}
		}

		/// <summary>
		/// States if this output controller instance can be a child of the specified output controller.
		/// </summary>
		/// <param name="otherController"></param>
		/// <returns></returns>
		public bool CanLinkTo(OutputController otherController) {
			// A controller can link to a parent controller if:
			// Neither controller is running.
			// The other controller doesn't already have a child link.
			//*** Raise an exception if they try to execute with a bad linking scheme?

			// If the other controller is null, they're trying to break the link so pass
			// it through.
			return
				otherController == null ||
				(!this.IsRunning &&
				!otherController.IsRunning &&
				otherController.Next == null);
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
				return true;
			}
			return false;
		}

		public Guid LinkedTo { get; set; }

		public bool IsRootController {
			get { return Prior == null && LinkedTo == Guid.Empty; }
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

		static protected void _AddInstance(OutputController controller) {
			// Reference the instance.
			_instances[controller.Id] = controller;

			//Clean this up.
			// Fix up any unresolved references.
			List<Tuple<IOutputStateSource, ControllerReference>> references;
			if(_unresolvedReferences.TryGetValue(controller.Id, out references)) {
				foreach(Tuple<IOutputStateSource, ControllerReference> reference in references) {
					AddSource(reference.Item1, reference.Item2);
				}
				_unresolvedReferences.Remove(controller.Id);
			}

			// Make sure the instance is running/not running like all the others.
			if(_stateAll == ExecutionState.Started) {
				_StartInstance(controller);
			} else {
				_StopInstance(controller);
			}
		}

		/// <summary>
		/// Removes the instance from the currently loaded set.  The controller is not deleted.
		/// </summary>
		/// <param name="controller"></param>
		static protected void _RemoveInstance(OutputController controller) {
			if(_instances.Remove(controller.Id)) {
				// Stop the controller.
				_StopInstance(controller);
				// Remove it from any patching.
				foreach(ChannelNode node in Vixen.Sys.Execution.Nodes) {
					if(node.Channel != null) {
						foreach(ControllerReference cr in node.Channel.Patch.ToArray()) {
							if(cr.ControllerId == controller.Id) {
								node.Channel.Patch.Remove(cr);
							}
						}
					}
				}
			}
		}

		static protected OutputController _GetInstance(Guid controllerId) {
			OutputController controller = null;
			if(_instances.TryGetValue(controllerId, out controller)) {
				return controller;
			}
			return null;
		}

		static public void StartAll() {
			if(_stateAll == ExecutionState.Stopped) {
				_stateAll = ExecutionState.Starting;

				// Start the hardware.
				// Running in parallel to prevent a bad actor from screwing up the other
				// controllers' ability to start.
				Parallel.ForEach(_instances.Values, _StartInstance);

				_stateAll = ExecutionState.Started;
			}
		}

		static public void StopAll() {
			if(_stateAll == ExecutionState.Started) {
				_stateAll = ExecutionState.Stopping;

				// Stop the hardware.
				// Running in parallel to prevent a bad actor from screwing up the other
				// controllers' ability to stop.
				Parallel.ForEach(_instances.Values, _StopInstance);

				_stateAll = ExecutionState.Stopped;
			}
		}

		static private void _StartInstance(OutputController controller) {
			if(!controller.IsRunning) {
				// Fixup link to another controller.
				OutputController parentController = _instances.Values.FirstOrDefault(x => x.Id == controller.LinkedTo);
				if(parentController == null || controller.CanLinkTo(parentController)) {
					controller.LinkTo(parentController);
				} else {
					VixenSystem.Logging.Error("Controller " + controller.Name + " is linked to controller " + parentController.Name + ", but it's an invalid link.");
				}

				if(controller._outputModule != null) {
					// Start the output module.
					controller._outputModule.Start();
				}

				// Create / Start the thread that updates the hardware.
				HardwareUpdateThread thread = new HardwareUpdateThread(controller);
				_updateThreads[controller.Id] = thread;
				thread.Start();
			}
		}

		static private void _StopInstance(OutputController controller) {
			if(controller.IsRunning) {
				// Stop the thread that updates the hardware.
				HardwareUpdateThread thread;
				if(_updateThreads.TryGetValue(controller.Id, out thread)) {
					thread.Stop();
					thread.WaitForFinish();
				}

				// Stop the hardware.
				if(controller._outputModule != null) {
					controller._outputModule.Stop();
				}
			}
		}

		//Clean this up.
		// Controller id : List of (source, reference)
		static private Dictionary<Guid, List<Tuple<IOutputStateSource,ControllerReference>>> _unresolvedReferences = new Dictionary<Guid,List<Tuple<IOutputStateSource,ControllerReference>>>();
		static public void AddSource(IOutputStateSource source, ControllerReference controllerReference) {
			OutputController controller;
			if(_instances.TryGetValue(controllerReference.ControllerId, out controller)) {
				controller.AddSource(source, controllerReference.OutputIndex);
			} else {
				List<Tuple<IOutputStateSource, ControllerReference>> references;
				if(!_unresolvedReferences.TryGetValue(controllerReference.ControllerId, out references)) {
					_unresolvedReferences[controllerReference.ControllerId] = references = new List<Tuple<IOutputStateSource, ControllerReference>>();
				}
				references.Add(new Tuple<IOutputStateSource,ControllerReference>(source, controllerReference));
			}
		}

		static public void RemoveSource(IOutputStateSource source, ControllerReference controllerReference) {
			OutputController controller;
			if(_instances.TryGetValue(controllerReference.ControllerId, out controller)) {
				controller.RemoveSource(source, controllerReference.OutputIndex);
			}
		}

		static public bool IsValidReference(ControllerReference controllerReference) {
			OutputController controller;
			if(_instances.TryGetValue(controllerReference.ControllerId, out controller)) {
				return controllerReference.OutputIndex < controller.OutputCount;
			}
			return false;
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
			// Commit to the instance collection as the new original instance.
			_AddInstance(this);
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

		// Must be properties for data binding.
		public Guid Id { get; set; }

		public void Rename(string newName) {
			newName = _Uniquify(newName);
			string newPath = Path.Combine(Directory, newName + FILE_EXT);
			Save();
			File.Move(FilePath, newPath);
			FilePath = newPath;
		}

		public string Name {
			get { return Path.GetFileNameWithoutExtension(FilePath); }
			set { Rename(value); }
		}

		private string _Uniquify(string name) {
			if(_instances.Values.Any(x => x.Name == name)) {
				string originalName = name;
				bool unique;
				int counter = 2;
				do {
					name = originalName + "-" + counter++;
					unique = !_instances.Values.Any(x => x.Name == name);
				} while(!unique);
			}
			return name;
		}

		public Guid InstanceId { get; private set; }

		public OutputController Prior { get; private set; }

		public OutputController Next { get; private set; }

		public bool IsRunning {
			get { return (_outputModule != null) ? _outputModule.IsRunning : false; }
		}

		public int UpdateInterval {
			//*** module will be null if it's missing after the controller's been created
			get { return _outputModule.UpdateInterval; }
		}

		static public OutputController Load(string filePath) {
			IReader reader = new XmlControllerReader();
			OutputController controller = reader.Read(filePath) as OutputController;
			return controller;
		}

		public void Delete() {
			// Stop  and remove the controller.
			_RemoveInstance(this);
			// Delete it.
			if(File.Exists(FilePath)) {
				File.Delete(FilePath);
			}
		}

		public int Version {
			get { return VERSION; }
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
				CurrentState = Command.Empty;
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

			public void UpdateState() {
				// Aggregate a state.
				if(_sources.Count > 0) {

					TimeSpan startTime = TimeSpan.Zero;
					TimeSpan endTime = TimeSpan.Zero;
					CommandStandard.CommandIdentifier commandIdentifier = null;
					object[] parameters = new object[0];

					if(_sources.Count == 1 && _sources.First.Value.SourceState.Length == 1) {
						Command seed = _sources.First.Value.SourceState[0];
						startTime = seed.StartTime;
						endTime = seed.EndTime;
						commandIdentifier = seed.CommandIdentifier;
						parameters = seed.ParameterValues;
					} else {
						foreach(IOutputStateSource source in _sources) {
							foreach(Command sourceCommand in source.SourceState) {
								startTime = startTime < sourceCommand.StartTime ? startTime : sourceCommand.StartTime;
								endTime = endTime > sourceCommand.EndTime ? endTime : sourceCommand.EndTime;
								//*** need a better resolution for multiple commands than this
								// First command wins
								commandIdentifier = commandIdentifier ?? sourceCommand.CommandIdentifier;
								//// Last command wins
								//commandIdentifier = source.SourceState.CommandIdentifier ?? commandIdentifier;
								parameters = CommandStandard.Standard.Combine(commandIdentifier, parameters, sourceCommand.ParameterValues, _owner.CombinationStrategy);
							}
						}
					}
					CurrentState = new Command(startTime, endTime, commandIdentifier, parameters);
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

		#region class HardwareUpdateThread
		class HardwareUpdateThread {
			private Thread _thread;
			private OutputController _controller;
			private ExecutionState _threadState = ExecutionState.Stopped;
			private EventWaitHandle _finished;

			private const int STOP_TIMEOUT = 4000;   // Four seconds should be plenty of time for a thread to stop.

			public HardwareUpdateThread(OutputController controller) {
				_controller = controller;
				_thread = new Thread(_ThreadFunc);
				_finished = new EventWaitHandle(false, EventResetMode.ManualReset);
			}

			public ExecutionState State { get { return _threadState; } }

			public void Start() {
				if(_threadState == ExecutionState.Stopped) {
					_threadState = ExecutionState.Started;
					_finished.Reset();
					_thread.Start();
				}
			}

			public void Stop() {
				if(_threadState == ExecutionState.Started) {
					_threadState = ExecutionState.Stopping;
				}
			}

			public void WaitForFinish() {
				if(!_finished.WaitOne(STOP_TIMEOUT)) {
					// Timed out waiting for a stop.
					//(This will prevent hangs in stopping, due to controller code failing to stop).
					throw new TimeoutException("Controller " + _controller.Name + " failed to stop in the required amount of time.");
				}
			}

			private void _ThreadFunc() {
				long frameStart, frameEnd, timeLeft;
				Stopwatch currentTime = Stopwatch.StartNew();

				// Thread main loop
				while(_threadState != ExecutionState.Stopping) {
					frameStart = currentTime.ElapsedMilliseconds;
					frameEnd = frameStart + _controller.UpdateInterval;

					_controller.Update();

					timeLeft = frameEnd - currentTime.ElapsedMilliseconds;

					if(timeLeft > 1) {
						Thread.Sleep((int)timeLeft);
					}
				}

				_threadState = ExecutionState.Stopped;
				_finished.Set();
			}
		}
		#endregion
	}
}
