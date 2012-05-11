using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.IO;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Sequence;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	/// <summary>
	/// Base class for any sequence implementation.
	/// </summary>
	[Executor(typeof(SequenceExecutor))]
	abstract public class Sequence : Vixen.Sys.ISequence {
		private ModuleLocalDataSet _moduleDataSet;
		private Guid _sequenceFilterStreamId;
		private MediaCollection _media;

		private const string DIRECTORY_NAME = "Sequence";

		[DataPath]
		static private readonly string _directory = Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		/// <summary>
		/// The directory that this sequence type will be saved in.
		/// </summary>
		protected virtual string Directory {
			get { return _directory; }
		}

		/// <summary>
		/// The generic default directory for all sequence types.
		/// </summary>
		static public string DefaultDirectory { get { return _directory; } }

		static public Sequence Create(string fileType) {
			// Get the specific sequence module manager.
			SequenceModuleManagement manager = Modules.GetManager<ISequenceModuleInstance, SequenceModuleManagement>();

			// Get an instance of the appropriate sequence module.
			Sequence sequence = manager.Get(fileType) as Sequence;

			return sequence;
		}

		static public string[] GetAllFileNames() {
			// We can't assume where all of the sequence file types will exist, so to provide
			// this functionality we will have to do the following:

			// Iterate all of the sequence type descriptors and build a set of file types.
			HashSet<string> fileTypes = new HashSet<string>();
			IEnumerable<ISequenceModuleDescriptor> sequenceDescriptors = Modules.GetDescriptors<ISequenceModuleInstance, ISequenceModuleDescriptor>();
			foreach(ISequenceModuleDescriptor descriptor in sequenceDescriptors) {
				fileTypes.Add(descriptor.FileExtension);
			}
			// Find all files of those types in the data branch.
			return fileTypes.SelectMany(x => System.IO.Directory.GetFiles(Paths.DataRootPath, "*" + x, SearchOption.AllDirectories)).ToArray();
		}

		/// <summary>
		/// Use this to set the sequence's length when the sequence is untimed.
		/// </summary>
		static public readonly TimeSpan Forever = TimeSpan.MaxValue;

		protected Sequence() {
			FilePath = "";
			InsertDataListener = new InsertDataListenerStack();
			InsertDataListener += _DataListener;
			Data = new DataStreams();
			_sequenceFilterStreamId = Data.CreateStream("SequenceFilter");
			TimingProvider = new TimingProviders(this);
			RuntimeBehaviors = Modules.ModuleManagement.GetAllRuntimeBehavior();
			ModuleDataSet = new ModuleLocalDataSet();
			_media = new MediaCollection();
		}

		protected Sequence(Sequence original) {
			FilePath = original.FilePath;
			InsertDataListener = new InsertDataListenerStack();
			InsertDataListener += _DataListener;
			Data = new DataStreams(original.Data);
			_sequenceFilterStreamId = original._sequenceFilterStreamId;
			TimingProvider = new TimingProviders(this, original.TimingProvider);
			RuntimeBehaviors = Modules.ModuleManagement.GetAllRuntimeBehavior();
			ModuleDataSet = (ModuleLocalDataSet)original.ModuleDataSet.Clone();
			Length = original.Length;
		}

		private bool _DataListener(IEffectNode effectNode) {
			Data.AddData(effectNode);
			ModuleDataSet.AssignModuleInstanceData(effectNode.Effect);
			// Do not cancel the event.
			return false;
		}

		virtual public void Save(string filePath) {
			FileSerializer<Sequence> serializer = SerializerFactory.Instance.CreateStandardSequenceSerializer();
			serializer.Write(this, filePath);
		}

		virtual public void Save() {
			Save(FilePath);
		}

		virtual public string Name {
			get { return Path.GetFileNameWithoutExtension(FilePath); }
		}

		public ModuleLocalDataSet ModuleDataSet {
			get { return _moduleDataSet; }
			set {
				if(_moduleDataSet != value) {
					_moduleDataSet = value;
					// The runtime behavior module instances will need data in the sequence's
					// data set.
					foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in RuntimeBehaviors) {
						_moduleDataSet.AssignModuleTypeData(runtimeBehavior);
					}
				}
			}
		}

		virtual public SequenceType SequenceType {
			get { return SequenceType.Standard; }
		}

		public TimeSpan Length { get; set; }

		virtual public string FilePath { get; set; }

		public InsertDataListenerStack InsertDataListener { get; set; }

		public void InsertData(IEffectNode effectNode)
		{
			InsertDataListener.InsertData(effectNode);
		}

		public void InsertData(IEnumerable<IEffectNode> effectNodes)
		{
			InsertDataListener.InsertData(effectNodes);
		}

		public bool RemoveData(IEffectNode effectNode)
		{
			return Data.RemoveData(effectNode);
		}


		#region IHasMedia
		public void AddMedia(IEnumerable<IMediaModuleInstance> modules) {
			foreach(IMediaModuleInstance module in modules) {
				AddMedia(module);
			}
		}

		public void AddMedia(IMediaModuleInstance module) {
			_moduleDataSet.AssignModuleInstanceData(module);
			_media.Add(module);
		}

		public IMediaModuleInstance AddMedia(string filePath) {
			MediaModuleManagement manager = Modules.GetManager<IMediaModuleInstance, MediaModuleManagement>();
			IMediaModuleInstance module = manager.Get(filePath);
			if(module != null) {
				// Set the file in the instance.
				module.MediaFilePath = filePath;
				AddMedia(module);
			}

			return module;
		}

		public bool RemoveMedia(IMediaModuleInstance module) {
			_moduleDataSet.RemoveModuleInstanceData(module);
			return _media.Remove(module);
		}

		public IEnumerable<IMediaModuleInstance> GetAllMedia() {
			return _media;
		}

		public void ClearMedia() {
			_media.Clear();
		}
		#endregion

		#region IHasSequenceFilters
		public void AddSequenceFilters(IEnumerable<ISequenceFilterNode> filterNodes) {
			foreach(SequenceFilterNode filterNode in filterNodes) {
				AddSequenceFilter(filterNode);
			}
		}

		public void AddSequenceFilter(ISequenceFilterNode sequenceFilterNode) {
			ModuleDataSet.AssignModuleInstanceData(sequenceFilterNode.Filter);
			Data.AddData(_sequenceFilterStreamId, sequenceFilterNode);
		}

		public bool RemoveSequenceFilter(ISequenceFilterNode sequenceFilterNode) {
			ModuleDataSet.RemoveModuleInstanceData(sequenceFilterNode.Filter);
			return Data.RemoveData(sequenceFilterNode);
		}

		public void ClearSequenceFilters() {
			foreach(ISequenceFilterNode filterNode in GetAllSequenceFilters()) {
				RemoveSequenceFilter(filterNode);
			}
		}
		#endregion

		public bool IsUntimed
		{
			get { return Length == Forever; }
			set { Length = value ? Forever : TimeSpan.Zero; }
		}

		public TimingProviders TimingProvider { get; protected set; }

		public DataStreams Data { get; private set; }

		// Every sequence will get a collection of all available runtime behaviors.
		public IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; private set; }

		public IEnumerable<IEffectNode> GetData() {
			return Data.GetMainStreamData().Cast<IEffectNode>();
		}

		public IEnumerable<ISequenceFilterNode> GetAllSequenceFilters() {
			return Data.GetStreamData(_sequenceFilterStreamId).Cast<ISequenceFilterNode>();
		}

		public ITiming GetTiming() {
			return TimingProvider.GetSelectedSource();
		}

		public override string ToString() {
			return Name;
		}
	}
}