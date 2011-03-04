using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Transform;
using Vixen.Module.Output;
using Vixen.Module.FileTemplate;
using Vixen.Sequence;

namespace Vixen.Hardware {
	public class OutputController : IEqualityComparer<ITransformModuleInstance>, IEnumerable<OutputController> {
		private IOutput _outputHardware;
		private List<Output> _outputs = new List<Output>();
		private Guid _linkedTo = Guid.Empty;
		private HashSet<SequenceBuffer> _sequenceBuffers = new HashSet<SequenceBuffer>();
		private OutputControllerBuffer _buffer;
		private IControllerHardwareModule _hardwareModule = null;

		private const string FILE_EXT = ".out";
		private const string DIRECTORY_NAME = "Controller";
		private const string ELEMENT_ROOT = "Controller";
		private const string ATTR_DEFINITION_NAME = "definition";
		private const string ATTR_ID = "id";
		private const string ATTR_NAME = "name";
		private const string ATTR_SINGLETON = "singleton";

		// Controller id : Controller
		static private Dictionary<Guid, OutputController> _instances = new Dictionary<Guid, OutputController>();
		static private Dictionary<Guid, int> _refCounts = new Dictionary<Guid, int>();

		[ObjectPreload]
		static private void _Preload() {
			OutputController controller;
			// Need a controller to act as a generic timing source in case a sequence
			// doesn't provide a timing source.
			controller = _CreateGenericTimer();
			AddInstance(controller);

			foreach(string filePath in System.IO.Directory.GetFiles(Directory, "*" + FILE_EXT)) {
				controller = new OutputController(filePath);
				AddInstance(controller);
			}
		}

		protected OutputController() {
			IsSingleton = true;
			Id = Guid.NewGuid();
			InstanceId = Guid.NewGuid();
			// Need to set the output count to cause the instantiation of the buffer.
			OutputCount = 0;
		}

		private OutputController(string fileName)
			: this() {
			using(FileStream fileStream = new FileStream(fileName, FileMode.Open)) {
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.IgnoreWhitespace = true;
				using(XmlReader reader = XmlReader.Create(fileStream, settings)) {
					reader.Read();
					if(reader.NodeType == XmlNodeType.XmlDeclaration) {
						reader.Read();
					}

					// Get attributes before the element.
					DefinitionName = reader.GetAttribute(ATTR_DEFINITION_NAME);
					Id = new Guid(reader.GetAttribute(ATTR_ID));
					Name = reader.GetAttribute(ATTR_NAME);
					IsSingleton = bool.Parse(reader.GetAttribute(ATTR_SINGLETON));

					ReadAttributes(reader);
					if(reader.ElementsExistWithin(ELEMENT_ROOT)) // Entity element
					{
						ReadBody(reader);
						reader.ReadEndElement(); // Controller
					}
				}
			}
			// Anytime there is a file read or written...
			FileName = fileName;
		}

		public string FileName { get; private set; }

		public void Save(string fileName) {
			Commit();
			using(FileStream fileStream = new FileStream(fileName, FileMode.Create)) {
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				using(XmlWriter writer = XmlWriter.Create(fileStream, settings)) {
					writer.WriteStartElement(ELEMENT_ROOT);
					writer.WriteAttributeString(ATTR_DEFINITION_NAME, DefinitionName);
					writer.WriteAttributeString(ATTR_ID, Id.ToString());
					writer.WriteAttributeString(ATTR_NAME, Name);
					writer.WriteAttributeString(ATTR_SINGLETON, IsSingleton.ToString());
					WriteAttributes(writer);
					WriteBody(writer);
					writer.WriteEndElement();
				}
			}
			// Anytime there is a file read or written...
			FileName = fileName;
		}

		static private OutputController _CreateGenericTimer() {
			OutputController controller = new OutputController();

			// The output module implementing Guid.Empty as its type id becomes the
			// default timing source.
			controller.HardwareModule = Modules.GetById(Guid.Empty) as IControllerHardwareModule;
			if(controller.HardwareModule == null) {
				throw new Exception("No default timing source found.");
			}
			controller.Name = "Generic timing source";

			return controller;
		}


		virtual protected void ReadAttributes(XmlReader reader) {
			_linkedTo = new Guid(reader.GetAttribute("linkedTo"));
			_UseDefinition(DefinitionName);
		}

		virtual protected void ReadBody(XmlReader reader) {
			// Outputs
			Output output;
			int outputIndex = 0;

			if(reader.ElementsExistWithin("Outputs")) {
				while(reader.IsStartElement("Output")) {
					if(outputIndex >= _outputs.Count) break;
					output = _outputs[outputIndex++];

					// - Module data for all transforms FIRST
					reader.Read(); // Get to ModuleData
					string str = reader.ReadOuterXml();
					output.TransformModuleData.Deserialize(str);

					// - Transform module references
					if(reader.ElementsExistWithin("Transforms")) {
						Guid moduleTypeId;
						Guid moduleInstanceId;
						while(reader.IsStartElement("Transform")) {
							moduleTypeId = new Guid(reader.GetAttribute("typeId"));
							moduleInstanceId = new Guid(reader.GetAttribute("instanceId"));

							output.AddTransform(moduleTypeId, moduleInstanceId);

							reader.Skip(); // Transform
						}
						reader.Skip(); // Transforms
					}
					reader.Skip(); // Output
				}
			}
			reader.ReadEndElement(); // Outputs
		}

		virtual protected void WriteAttributes(XmlWriter writer) {
			writer.WriteAttributeString("linkedTo", _linkedTo.ToString());
		}

		virtual protected void WriteBody(XmlWriter writer) {
			// Outputs
			writer.WriteStartElement("Outputs");
			foreach(Output output in _outputs) {
				writer.WriteStartElement("Output");
				// - Transform module data FIRST
				writer.WriteRaw(output.TransformModuleData.Serialize());

				// - Transform module references
				writer.WriteStartElement("Transforms");
				foreach(ITransformModuleInstance transform in output.DataTransforms) {
					writer.WriteStartElement("Transform");
					writer.WriteAttributeString("typeId", transform.TypeId.ToString());
					writer.WriteAttributeString("instanceId", transform.InstanceId.ToString());
					writer.WriteEndElement(); // Transform
				}
				writer.WriteEndElement(); // Transforms

				writer.WriteEndElement(); // Output
			}
			writer.WriteEndElement(); // Outputs
		}

		[DataPath]
		static public string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		static public OutputController NewController(string name, string controllerDefinitionName) {
			OutputController controller = new OutputController();
			controller.Name = name;
			controller._UseDefinition(controllerDefinitionName);
			_ReflectTemplateInto(controller);
			OutputController.AddInstance(controller);
			controller.Save(Path.Combine(Directory, name + FILE_EXT));
			return controller;
		}

		static public OutputController Get(Guid id) {
			return GetInstance(id);
		}

		static public IEnumerable<OutputController> InitializeControllers(IModuleDataSet executableDataSet, int startTime) {
			// Only want to expose top-level root controllers to execution.
			// This is our hook into the process.
			// Get the set of output controllers we're going to be returning.
			OutputController[] controllers = Initialize(executableDataSet, startTime).ToArray();
			_FixupLinks(controllers);
			// Return all controllers so that all controllers can go to the update buffer for
			// lookup because all controllers will be updating.
			//*** would be better if the controller references didn't have to be provided, but
			//    patches provide only a controller id when updating and the buffer needs the
			//    controller reference to commit updates to
			return controllers;
		}

		static public void StartControllers(IEnumerable<OutputController> controllers) {
			// Only want to expose top-level root controllers to execution.
			// This is our hook into the process.
			Start(_GetRootControllers(controllers));
		}

		static public void StopControllers(IEnumerable<OutputController> controllers) {
			// Only want to expose top-level root controllers to execution.
			// This is our hook into the process.
			Stop(_GetRootControllers(controllers));
		}

		/// <summary>
		/// References the controllers without starting them.
		/// </summary>
		/// <param name="controllers"></param>
		static public void ReferenceControllers(IEnumerable<OutputController> controllers) {
			foreach(OutputController controller in _GetRootControllers(controllers)) {
				_IncrementRefCount(controller);
			}
		}

		static public void DereferenceControllers(IEnumerable<OutputController> controllers) {
			Stop(controllers);
		}

		static private IEnumerable<OutputController> _GetRootControllers(IEnumerable<OutputController> controllers) {
			if(controllers == null) return Enumerable.Empty<OutputController>();
			return controllers.Where(x => x.IsRootController);
		}

		static public void PauseControllers(IEnumerable<OutputController> controllers) {
			foreach(OutputController controller in _GetRootControllers(controllers)) {
				controller.HardwareModule.Pause();
			}
		}

		static public void ResumeControllers(IEnumerable<OutputController> controllers) {
			foreach(OutputController controller in _GetRootControllers(controllers)) {
				controller.HardwareModule.Resume();
			}
		}

		// This needs to be encapsulated by OutputController because any public method
		// to get a Controller reference is going cause it to be referenced and started.
		static public IEnumerable<Guid> GetTimingSources() {
			OutputModuleManagement manager = Server.Internal.GetModuleManager<IOutputModuleInstance, OutputModuleManagement>();
			return
				(from outputController in _GetCloneSet()
				 select outputController.HardwareModule.TypeId)
				 .Intersect((IEnumerable<Guid>)manager.GetAllTimingSources())
				 .Distinct(); // Two controllers can reference the same hardware module.
		}

		/// <summary>
		/// Returns the collection of available controllers.  There is no configuration
		/// data loaded for these instances.
		/// </summary>
		/// <returns></returns>
		static public IEnumerable<OutputController> GetAll() {
			// Even though the controllers aren't being initialized for execution, they still
			// need their links resolved within the set.
			OutputController[] controllers = _GetCloneSet().ToArray();
			_FixupLinks(controllers);
			return controllers;
		}

		/// <summary>
		/// Returns an appropriate collection of controllers.  Configuration data is loaded
		/// from the data set.
		/// </summary>
		/// <returns></returns>
		static public IEnumerable<OutputController> GetAll(IModuleDataSet executableDataSet) {
			// Get a clone set.
			// Need to have an enumerated set, otherwise the last instance will be what's
			// captured when the enumerator is run.
			OutputController[] controllers = GetAll().ToArray();
			// Get data for the output modules of the clone set.
			// Singletons will get their data from the application data store.
			// Non-singletons will get their data from the provided data set.
			// Trying to retrieve data from a data set that doesn't already data for
			// the module will result in data being created.
			foreach(OutputController controller in controllers) {
				GetHardwareSetup(controller, executableDataSet);
			}
			return controllers;
		}

		public void ApplyTransforms(CommandData command, int outputIndex) {
			if(!command.IsEmpty) {
				foreach(ITransformModuleInstance transform in _outputs[outputIndex].DataTransforms) {
					transform.Transform(command);
				}
			}
		}

		public void BeginUpdate() {
			if(IsRootController && OutputHardware != null) {
				// Need to begin a transaction for each controller in the chain.
				foreach(OutputController controller in this) {
					controller._buffer.BeginTransaction();
				}
			}
		}

		public void Update(int outputIndex, CommandData command) {
			_buffer.Write(outputIndex, command);
		}

		public void EndUpdate() {
			// Updates start at the root controllers and cascade from there.
			// Non-root controllers are not directly updated; they are only updated
			// from a previous-linked controller.
			if(IsRootController && OutputHardware != null) {
				// Need to end the transaction for each controller in the chain.
				// Update our whole chain.
				// Have all controllers in the chain commit first.
				// If any of them had changes, the whole chain needs to output.
				// (Cannot use .Any because it will stop after the first successful Commit.)
				if(this.Count(x => x._buffer.Commit()) > 0) {
					foreach(OutputController controller in this) {
						controller.OutputHardware.UpdateState(_buffer.GetOutputStates());
					}
				}
			}
		}

		public void UpdateFrom(SequenceBuffer sequenceBuffer) {
			// I CANNOT EXPLAIN THIS.
			// At this point, _sequenceBuffers is set.
			// By the time it reaches _sequenceBuffers.Add, the class variable is set to null.
			// Private class variable.
			// Nothing ever sets it to null.
			// Is always assigned in the constructor.
			// Running in the main thread; worker thread is waiting and does not touch this.
			//HashSet<SequenceBuffer> sequenceBuffers = _sequenceBuffers;
			// Add the buffer to the set.
			_sequenceBuffers.Add(sequenceBuffer);

			// Update the controller state from all contributing sequences.
			BeginUpdate();
			foreach(SequenceBuffer buffer in _sequenceBuffers.ToArray()) {
				// If it's no longer valid, drop the reference.
				if(buffer.IsDisposed) {
					_sequenceBuffers.Remove(buffer);
				} else {
					foreach(Tuple<ControllerReference, CommandData> update in buffer.GetControllerState(Id)) {
						//*** the buffer needs to merge the states somehow
						_buffer.Write(update.Item1.OutputIndex, update.Item2);
					}
				}
			}
			EndUpdate();
		}

		private void _UseDefinition(string name) {
			DefinitionName = name;
			OutputControllerDefinition controllerDefinition = OutputControllerDefinition.Get(name);
			OutputCount = controllerDefinition.OutputCount;
			HardwareModule = Modules.GetById(controllerDefinition.HardwareModuleId) as IControllerHardwareModule;
		}

		virtual protected void HardwareModuleUpdated() {
			OutputHardware = HardwareModule as IOutput;
			// Need to use the backing field because we want to affect the output module
			// used by this controller, not the root controller.
			if(_outputHardware != null) {
				_outputHardware.SetOutputCount(OutputCount);
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
				//*** make clone call to output and have it handle the transforms
				newOutput = new Output();
				newOutput.TransformModuleData.Deserialize(output.TransformModuleData.Serialize());
				foreach(ITransformModuleInstance transform in output.DataTransforms) {
					newOutput.AddTransform(transform);
				}
				controller._outputs.Add(newOutput);
			}

			controller.InstanceId = Guid.NewGuid();
			if(HardwareModule != null) {
				controller.HardwareModule = Modules.GetById(this.HardwareModule.TypeId) as IControllerHardwareModule;
			}
			return controller;
		}

		public Output[] Outputs {
			get { return _outputs.ToArray(); }
		}

		public int OutputCount {
			get { return _outputs.Count; }
			set {
				// Update the hardware.
				if(_outputHardware != null) {
					_outputHardware.SetOutputCount(value);
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
						output = new Output();
						_outputs.Add(output);
					}
				}
				// Once the output count is up-to-date, update the output buffer.
				_buffer = new OutputControllerBuffer(this);
			}
		}

		static private void _ReflectTemplateInto(OutputController outputController) {
			FileTemplateModuleManagement manager = Server.Internal.GetModuleManager<IFileTemplateModuleInstance, FileTemplateModuleManagement>();
			manager.ProjectTemplateInto(FILE_EXT, outputController);
		}

		public OutputController Prior { get; private set; }
		public OutputController Next { get; private set; }
		/// <summary>
		/// States if this output controller instance can be a child of the specified output controller.
		/// </summary>
		/// <param name="otherController"></param>
		/// <returns></returns>
		public bool CanLinkTo(OutputController otherController) {
			// This should only be called on an output controller that is part of a clone set.
			// Original instances are not available and the link editing will be done on a
			// clone set, so an OutputController instance should already have its references
			// fixed up.

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

		public bool IsRootController {
			get { return Prior == null && _linkedTo == Guid.Empty; }
		}

		private IOutput OutputHardware {
			get {
				// When output controllers are linked, only the root controller will be
				// connected to the port, therefore only it will have the output module
				// used during execution.
				if(Prior != null) return Prior.OutputHardware;
				return _outputHardware;
			}
			set { _outputHardware = value; }
		}

		virtual protected void CommitState() {
			if(Prior != null) {
				_linkedTo = Prior.Id;
			} else {
				_linkedTo = Guid.Empty;
			}
		}




		static protected void AddInstance(OutputController controller) {
			_instances[controller.Id] = controller;
		}

		static protected OutputController GetInstance(Guid controllerId) {
			OutputController controller = null;
			if(_instances.TryGetValue(controllerId, out controller)) {
				return _GetControllerInstance(controller);
			}
			return null;
		}

		/// <summary>
		/// Retrieves an appropriate set of controllers and initializes them for execution using
		/// module data.
		/// </summary>
		/// <param name="controllerTypeKey"></param>
		/// <param name="executableDataSet"></param>
		/// <param name="startTime"></param>
		/// <returns></returns>
		static protected IEnumerable<OutputController> Initialize(IModuleDataSet executableDataSet, int startTime) {
			OutputController instance;
			foreach(OutputController controller in _instances.Values) {
				instance = _GetControllerInstance(controller);
				// This needs to be done here because it's the only point between
				// the context-specific instance being created and the controller
				// being started.
				GetHardwareSetup(instance, executableDataSet);
				instance.HardwareModule.Initialize(startTime);
				yield return instance;
			}
		}

		static protected void GetHardwareSetup(OutputController controller, IModuleDataSet executableDataSet) {
			// This is the concentration point for the incoming data set.
			// Singletons must have a single set of data, which means it can only be stored in one
			// place, so that is the application-level store.  Otherwise it's the provided dataset.
			// (May be from this controller, may be a program, may be a sequence...anything.)
			IModuleDataSet data = controller.IsSingleton ? Server.ModuleData : executableDataSet;
			if(data != null) {
				data.GetModuleTypeData(controller.HardwareModule);
			}
		}

		static protected void Start(IEnumerable<OutputController> controllers) {
			foreach(OutputController controller in controllers) {
				if(_IncrementRefCount(controller) > 0 || !controller.IsSingleton) {
					_StartInstance(controller);
				}
			}
		}

		static protected void Stop(IEnumerable<OutputController> controllers) {
			foreach(OutputController controller in controllers) {
				if(_DecrementRefCount(controller) <= 0 || !controller.IsSingleton) {
					_StopInstance(controller);
				}
			}
		}

		static private void _StartInstance(OutputController controller) {
			if(!controller.IsRunning) {
				controller.HardwareModule.Startup();
			}
		}

		static private void _StopInstance(OutputController controller) {
			if(controller.IsRunning) {
				controller.HardwareModule.Shutdown();
			}
		}

		static private OutputController _GetControllerInstance(OutputController controller) {
			OutputController instance;
			if(controller == null || controller.IsSingleton) {
				instance = controller;
			} else {
				instance = controller.Clone();
			}
			return instance;
		}


		/// <summary>
		/// This gets a set of cloned controllers, but without incrementing reference
		/// counts.  The controllers will not be able to be used, they are for reference
		/// only.  No configuration data is loaded.
		/// </summary>
		/// <param name="controllerTypeKey"></param>
		/// <returns></returns>
		static protected IEnumerable<OutputController> _GetCloneSet() {
			foreach(OutputController controller in _instances.Values) {
				yield return controller.Clone();
			}
		}

		static private int _IncrementRefCount(OutputController controller) {
			int count;

			_refCounts.TryGetValue(controller.Id, out count);
			_refCounts[controller.Id] = ++count;

			return count;
		}

		static private int _DecrementRefCount(OutputController controller) {
			int count;
			_refCounts.TryGetValue(controller.Id, out count);
			if(count > 0) {
				// Otherwise should not be possible, but...
				count--;
			}
			_refCounts[controller.Id] = count;

			return count;
		}

		public IControllerHardwareModule HardwareModule {
			get { return _hardwareModule; }
			protected set {
				_hardwareModule = value;
				if(_hardwareModule != null) {
					this.IsSingleton = _hardwareModule.DefaultAsSingleton;
				}
				HardwareModuleUpdated();
			}
		}

		/// <summary>
		/// Runs the controller setup and commits it upon success.
		/// </summary>
		/// <returns>True if the setup was successful and committed.  False if the user canceled.</returns>
		public bool Setup() {
			if(HardwareModule != null) {
				if(HardwareModule.Setup()) {
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
			AddInstance(this);
		}



		static private void _FixupLinks(IEnumerable<OutputController> controllers) {
			// Fix up links within that set.
			foreach(OutputController controller in controllers) {
				controller.LinkTo(controllers.FirstOrDefault(x => x.Id == controller._linkedTo));
			}
		}

		// Must be properties for data binding.
		public string DefinitionName { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
		// Not in the definition because it's intially decided by the output module and then
		// each instance of a controller can override that.
		public bool IsSingleton { get; set; }
		public Guid InstanceId { get; private set; }
		public bool IsRunning {
			get { return (HardwareModule != null) ? HardwareModule.IsRunning : false; }
		}

		#region IEqualityComparer<ITransformModuleInstance>
		public bool Equals(ITransformModuleInstance x, ITransformModuleInstance y) {
			return x.TypeId == y.TypeId && x.InstanceId == y.InstanceId;
		}

		public int GetHashCode(ITransformModuleInstance obj) {
			return (obj.TypeId.ToString() + obj.InstanceId.ToString()).GetHashCode();
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
			private LinkedList<ITransformModuleInstance> _dataTransforms = new LinkedList<ITransformModuleInstance>();

			public Output() {
				TransformModuleData = new ModuleDataSet();
			}

			public ModuleDataSet TransformModuleData { get; private set; }

			public ITransformModuleInstance[] DataTransforms {
				get { return _dataTransforms.ToArray(); }
			}

			public void RemoveTransform(Guid transformTypeId, Guid transformInstanceId) {
				ITransformModuleInstance instance = _dataTransforms.FirstOrDefault(x => x.TypeId == transformTypeId && x.InstanceId == transformInstanceId);
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
			public void AddTransform(Guid transformTypeId, Guid transformInstanceId) {
				// Create a new instance.
				ITransformModuleInstance instance = Server.Internal.GetModuleManager<ITransformModuleInstance>().Get(transformTypeId) as ITransformModuleInstance;
				instance.InstanceId = transformInstanceId;
				// Create data for the instance.
				TransformModuleData.GetModuleInstanceData(instance);
				// Add the instance.
				_dataTransforms.AddLast(instance);
			}

			public void AddTransform(ITransformModuleInstance instance) {
				// Allowing multiple instances of a transform type.
				// Create a new instance, but use the same data (clone).
				ITransformModuleInstance newInstance = Server.Internal.GetModuleManager<ITransformModuleInstance>().Clone(instance) as ITransformModuleInstance;
				// Add the data to our transform dataset.
				this.TransformModuleData.Add(newInstance.ModuleData);
				// Add the instance.
				_dataTransforms.AddLast(newInstance);
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
