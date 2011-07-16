using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Transform;
using Vixen.Module.Output;
using Vixen.Module.FileTemplate;
using Vixen.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Vixen.Hardware {
	public class OutputController : IEqualityComparer<ITransformModuleInstance>, IEnumerable<OutputController> {
		private Guid _outputModuleId;
		private IOutputModuleInstance _outputModule = null;
		private List<Output> _outputs = new List<Output>();
		private Guid _linkedTo = Guid.Empty;

		private const string FILE_EXT = ".out";
		private const string DIRECTORY_NAME = "Controller";
		private const string ELEMENT_ROOT = "Controller";
		private const string ATTR_ID = "id";
		private const string ATTR_COMB_STRATEGY = "strategy";
		private const string ATTR_LINKED_TO = "linkedTo";
		private const string ATTR_OUTPUT_COUNT = "outputCount";
		private const string ATTR_HARDWARE_ID = "hardwareId";

		// Controller id : Controller
		static private Dictionary<Guid, OutputController> _instances = new Dictionary<Guid, OutputController>();
		static private Thread _updateThread;

		private enum ExecutionState { Starting, Started, Stopping, Stopped };
		static private ExecutionState _state = ExecutionState.Stopped;

		static public void ReloadAll() {
			// Stop the controllers.
			foreach(OutputController controller in _instances.Values) {
				_StopInstance(controller);
			}
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

		private OutputController() { }

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="name"></param>
		public OutputController(string name, int outputCount, Guid outputModuleId)
			: this(name, outputCount, outputModuleId, CommandStandard.Standard.CombinationOperation.HighestWins) {
		}

		public OutputController(string name, int outputCount, Guid outputModuleId, CommandStandard.Standard.CombinationOperation combinationStrategy) {
			Id = Guid.NewGuid();
			FilePath = Path.Combine(Directory, name + FILE_EXT);
			OutputCount = outputCount;
			OutputModuleId = outputModuleId;

			// Affect the instance with the controller template last.
			FileTemplateModuleManagement manager = Modules.GetModuleManager<IFileTemplateModuleInstance, FileTemplateModuleManagement>();
			manager.ProjectTemplateInto(FILE_EXT, this);
		}

		public void Save() {
			Save(FilePath);
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");
			filePath = Path.Combine(Directory, Path.GetFileName(filePath));
			OutputControllerWriter writer = new OutputControllerWriter();
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
			}
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
			// Need to manually create the output collection so transforms get in
			// and we're not referencing the same collection.
			controller._outputs = new List<Output>();
			Output newOutput;
			foreach(Output output in this._outputs) {
				//*** make clone call to output and have it handle the transforms and patching
				newOutput = new Output(this);
				newOutput.TransformModuleData.Deserialize(output.TransformModuleData.Serialize());
				foreach(ITransformModuleInstance transform in output.DataTransforms) {
					newOutput.AddTransform(transform);
				}
				controller._outputs.Add(newOutput);
			}

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
					_outputModule.SetOutputCount(value);
				}
				// Adjust the outputs list.
				if(value < _outputs.Count) {
					_outputs.RemoveRange(value, _outputs.Count - value);
				} else {
					//*** When an existing controller has its output count initially set when it's being
					//    loaded, the template affects it.  This should only happen when initially creating
					//    the controller?
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

		public Guid LinkedTo {
			get { return _linkedTo; }
		}

		public bool IsRootController {
			get { return Prior == null && _linkedTo == Guid.Empty; }
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
				_linkedTo = Prior.Id;
			} else {
				_linkedTo = Guid.Empty;
			}
		}

		static protected void _AddInstance(OutputController controller) {
			// Reference the instance.
			_instances[controller.Id] = controller;

			// Make sure the instance is running/not running like all the others.
			if(_state == ExecutionState.Started) {
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
				var references = Vixen.Sys.Execution.Nodes.SelectMany(x => x).SelectMany(x => x.Patch.ControllerReferences.Where(y => y.ControllerId == controller.Id).Select(z => new { x.Patch, ControllerReference = z }));
				foreach(var reference in references) {
					reference.Patch.Remove(reference.ControllerReference.ControllerId, reference.ControllerReference.OutputIndex);
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
			if(_state == ExecutionState.Stopped) {
				_state = ExecutionState.Starting;
				// Start the hardware.
				foreach(OutputController controller in _instances.Values) {
					_StartInstance(controller);
				}

				// Start the thread that updates the hardware.
				_updateThread = new Thread(_HardwareUpdateThread);
				_updateThread.Start();
				_state = ExecutionState.Started;
			}
		}

		static public void StopAll() {
			if(_state == ExecutionState.Started) {
				// Stop the thread that updates the hardware.
				_state = ExecutionState.Stopping;
				while(_state != ExecutionState.Stopped) Thread.Sleep(1);

				// Stop the hardware.
				foreach(OutputController controller in _instances.Values) {
					_StopInstance(controller);
				}
			}
		}

		static private void _HardwareUpdateThread() {
			//*** this will be user data
			//*** this is at least partially pointless; Windows is going to do what it damn well pleases
			int refreshRate = 50; // Updates/second
			double frameLength = 1000d / refreshRate;
			double frameStart;
			double frameEnd;
			Stopwatch currentTime = Stopwatch.StartNew();
			double timeLeft;

			while(_state != ExecutionState.Stopping) {
				frameStart = currentTime.ElapsedMilliseconds;
				frameEnd = frameStart + frameLength;

				foreach(OutputController controller in _instances.Values) {
					controller.Update();
				}

				timeLeft = frameEnd - currentTime.ElapsedMilliseconds;
				if(timeLeft > 1) {
					Thread.Sleep((int)timeLeft);
				}
			}
			_state = ExecutionState.Stopped;
		}

		static private void _StartInstance(OutputController controller) {
			if(!controller.IsRunning) {
				// Fixup link to another controller.
				controller.LinkTo(_instances.Values.FirstOrDefault(x => x.Id == controller._linkedTo));
				if(controller._outputModule != null) {
					// Get the module data for the controller's output module.
					VixenSystem.ModuleData.GetModuleTypeData(controller._outputModule);
					// Start the output module.
					controller._outputModule.Start();
				}
			}
		}

		static private void _StopInstance(OutputController controller) {
			if(controller.IsRunning && controller._outputModule != null) {
				controller._outputModule.Stop();
			}
		}

		static public bool AddSource(IOutputStateSource source, ControllerReference controllerReference) {
			OutputController controller;
			if(_instances.TryGetValue(controllerReference.ControllerId, out controller)) {
				return controller.AddSource(source, controllerReference.OutputIndex);
			}
			return false;
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
		
		public string Name {
			get { return Path.GetFileNameWithoutExtension(FilePath); }
		}
		
		public Guid InstanceId { get; private set; }

		public OutputController Prior { get; private set; }
		
		public OutputController Next { get; private set; }
		
		public bool IsRunning {
			get { return (_outputModule != null) ? _outputModule.IsRunning : false; }
		}

		static public OutputController Load(string filePath) {
			OutputControllerReader reader = new OutputControllerReader();
			OutputController controller = reader.Read(filePath) as OutputController;
			controller.FilePath = filePath;

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

		static public XElement WriteXml(OutputController controller) {
			controller.Commit();
			return new XElement(ELEMENT_ROOT,
				new XAttribute(ATTR_HARDWARE_ID, controller.OutputModuleId),
				new XAttribute(ATTR_OUTPUT_COUNT, controller.OutputCount),
				new XAttribute(ATTR_ID, controller.Id),
				new XAttribute(ATTR_COMB_STRATEGY, controller.CombinationStrategy),
				new XAttribute(ATTR_LINKED_TO, controller._linkedTo),
				new XElement("Outputs",
					controller._outputs.Select(x =>
						new XElement("Output",
							new XAttribute("name", x.Name),
							XElement.Parse(x.TransformModuleData.Serialize()), // First element within Output
							new XElement("Transforms",
								x.DataTransforms.Select(y =>
									new XElement("Transform",
										new XAttribute("typeId", y.Descriptor.TypeId),
										new XAttribute("instanceId", y.InstanceId))))))));
		}

		static public OutputController ReadXml(XElement element) {
			OutputController controller = new OutputController();
			controller.OutputModuleId = new Guid(element.Attribute(ATTR_HARDWARE_ID).Value);
			controller.OutputCount = int.Parse(element.Attribute(ATTR_OUTPUT_COUNT).Value);
			controller.Id = Guid.Parse(element.Attribute(ATTR_ID).Value);
			controller.InstanceId = Guid.NewGuid();
			controller.CombinationStrategy = (CommandStandard.Standard.CombinationOperation)Enum.Parse(typeof(CommandStandard.Standard.CombinationOperation), element.Attribute(ATTR_COMB_STRATEGY).Value);
			controller._linkedTo = Guid.Parse(element.Attribute(ATTR_LINKED_TO).Value);

			int outputIndex = 0;
			foreach(XElement outputElement in element.Element("Outputs").Elements("Output")) {
				// Data persisted in the controller instance may exceed the
				// output count.
				if(outputIndex >= controller.OutputCount) break;
				
				// The outputs were created when the output count was set.
				Output output = controller._outputs[outputIndex++];

				output.Name = outputElement.Attribute("name").Value;

				// Deserialize the output's transform dataset.
				output.TransformModuleData.Deserialize(outputElement.FirstNode.ToString());

				// Create transform instances for the outputs.
				foreach(XElement transformElement in outputElement.Element("Transforms").Elements("Transform")) {
					Guid moduleTypeId = Guid.Parse(transformElement.Attribute("typeId").Value);
					Guid moduleInstanceId = Guid.Parse(transformElement.Attribute("instanceId").Value);
					output.AddTransform(moduleTypeId, moduleInstanceId);
				}
			}

			return controller;
		}

		#region IEqualityComparer<ITransformModuleInstance>
		public bool Equals(ITransformModuleInstance x, ITransformModuleInstance y) {
			return x.Descriptor.TypeId == y.Descriptor.TypeId && x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(ITransformModuleInstance obj) {
			return (obj.Descriptor.TypeId.ToString() + obj.InstanceId.ToString()).GetHashCode();
		}
		#endregion

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
			private LinkedList<ITransformModuleInstance> _dataTransforms = new LinkedList<ITransformModuleInstance>();
			private LinkedList<IOutputStateSource> _sources = new LinkedList<IOutputStateSource>();

			public Output(OutputController owner) {
				_owner = owner;
				CurrentState = CommandData.Empty;
				TransformModuleData = new ModuleDataSet();
				Name = "Unnamed";
			}

			// Completely independent; nothing is current dependent upon this value.
			public string Name { get; set; }

			public ModuleDataSet TransformModuleData { get; private set; }

			public ITransformModuleInstance[] DataTransforms {
				get { return _dataTransforms.ToArray(); }
			}

			public void RemoveTransform(Guid transformTypeId, Guid transformInstanceId) {
				ITransformModuleInstance instance = _dataTransforms.FirstOrDefault(x => x.Descriptor.TypeId == transformTypeId && x.InstanceId == transformInstanceId);
				if(instance != null) {
					// Remove from the transform list.
					// (I believe that LinkedList<T>.Remove does not use an equality comparer
					// and goes by reference equality.)
					_dataTransforms.Remove(instance);
					// Remove from the transform module data.
					TransformModuleData.Remove(transformTypeId, transformInstanceId);
				}
			}

			//Not sure about this; may not want all of this behavior anytime a transform
			//is added to an output
			/// <summary>
			/// Adds an instance of a transform to the output.
			/// </summary>
			/// <param name="transformTypeId">The module type id of the transform.</param>
			/// <param name="transformInstanceId">Provide if known.  For example, for existing module instance data.  Otherwise, provide Guid.Emtpy.</param>
			public void AddTransform(Guid transformTypeId, Guid transformInstanceId) {
				// Create a new instance.
				ITransformModuleInstance instance = Modules.ModuleManagement.GetTransform(transformTypeId);
				// If an instance id is provided (creating an instance for existing module
				// data), assign it.
				if(transformInstanceId != Guid.Empty) {
					instance.InstanceId = transformInstanceId;
				}
				// Create data for the instance.
				TransformModuleData.GetModuleInstanceData(instance);
				// Add the instance.
				_dataTransforms.AddLast(instance);
			}

			/// <summary>
			/// Adds a new instance of a transform to the output.
			/// </summary>
			public void AddTransform(Guid transformTypeId) {
				AddTransform(transformTypeId, Guid.Empty);
			}

			/// <summary>
			/// Adds an existing instance of a transform to the output.
			/// </summary>
			/// <param name="instance"></param>
			public void AddTransform(ITransformModuleInstance instance) {
				// Allowing multiple instances of a transform type.
				// Create a new instance, but use the same data (clone).
				ITransformModuleInstance newInstance = Modules.GetModuleManager<ITransformModuleInstance>().Clone(instance) as ITransformModuleInstance;
				if(newInstance.ModuleData != null) {
					// Add the data to our transform dataset.
					this.TransformModuleData.Add(newInstance.ModuleData);
				} else {
					// Create data for the instance.
					TransformModuleData.GetModuleInstanceData(newInstance);
				}
				// Add the instance.
				_dataTransforms.AddLast(newInstance);
			}

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

					long startTime = 0;
					long endTime = 0;
					CommandStandard.CommandIdentifier commandIdentifier = null;
					object[] parameters = new object[0];
					
					if(_sources.Count == 1) {
						CommandData seed = _sources.First.Value.SourceState;
						startTime = seed.StartTime;
						endTime = seed.EndTime;
						commandIdentifier = seed.CommandIdentifier;
						parameters = seed.ParameterValues;
					} else {
						foreach(IOutputStateSource source in _sources) {
							startTime = Math.Min(startTime, source.SourceState.StartTime);
							endTime = Math.Max(endTime, source.SourceState.EndTime);
							//*** need a better resolution for multiple commands than this
							// First command wins
							commandIdentifier = commandIdentifier ?? source.SourceState.CommandIdentifier;
							//// Last command wins
							//commandIdentifier = source.SourceState.CommandIdentifier ?? commandIdentifier;
							parameters = CommandStandard.Standard.Combine(commandIdentifier, parameters, source.SourceState.ParameterValues, _owner.CombinationStrategy);
						}
					}
					CurrentState = new CommandData(startTime, endTime, commandIdentifier, parameters);
					// Transform it.
					if(parameters != null && parameters.Length > 0) {
						foreach(ITransformModuleInstance transform in _dataTransforms) {
							transform.Transform(CurrentState);
						}
					}
				}
			}

			public CommandData CurrentState { get; private set; }
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
