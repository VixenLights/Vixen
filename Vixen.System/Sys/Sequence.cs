using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using Vixen.Module.PreFilter;
using Vixen.Module.Timing;
using Vixen.IO;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Effect;
using Vixen.Module.Sequence;
using Vixen.IO.Xml;

namespace Vixen.Sys {
	/// <summary>
	/// Base class for any sequence implementation.
	/// </summary>
	[Executor(typeof(SequenceExecutor))]
	[SequenceReader(typeof(XmlSequenceReader))]
	abstract public class Sequence : Vixen.Sys.ISequence, IVersioned {
		private IModuleDataSet _moduleDataSet;
		private Guid _preFilterStreamId;

		private const string DIRECTORY_NAME = "Sequence";
		private const int VERSION = 1;

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

		/// <summary>
		/// Loads an existing instance.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		static public Sequence Load(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) return null;

			IReader reader = new XmlAnySequenceReader();
			if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(DefaultDirectory, filePath);
			Sequence instance = (Sequence)reader.Read(filePath);
			return instance;
		}

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
			_preFilterStreamId = Data.CreateStream("PreFilter");
			TimingProvider = new TimingProviders(this);
			RuntimeBehaviors = Modules.ModuleManagement.GetAllRuntimeBehavior();
			ModuleDataSet = new ModuleLocalDataSet();
			// Media set in ModuleDataSet setter.
			// Runtime behaviors set in ModuleDataSet setter.
		}

		protected Sequence(Sequence original) {
			FilePath = original.FilePath;
			InsertDataListener = new InsertDataListenerStack();
			InsertDataListener += _DataListener;
			Data = new DataStreams(original.Data);
			_preFilterStreamId = original._preFilterStreamId;
			TimingProvider = new TimingProviders(this, original.TimingProvider);
			RuntimeBehaviors = Modules.ModuleManagement.GetAllRuntimeBehavior();
			ModuleDataSet = original.ModuleDataSet.Clone();

			Length = original.Length;
		}

		private bool _DataListener(EffectNode effectNode) {
			Data.AddData(effectNode);
			ModuleDataSet.GetModuleInstanceData(effectNode.Effect);
			// Do not cancel the event.
			return false;
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");
			filePath = Path.Combine(this.Directory, Path.GetFileName(filePath));
			IWriter writer = _GetSequenceWriter();
			writer.Write(filePath, this);
			this.FilePath = filePath;
		}

		virtual protected IWriter _GetSequenceWriter() {
			return new XmlSequenceWriter();
		}

		public void Save() {
			Save(FilePath);
		}

		virtual public string Name {
			get { return Path.GetFileNameWithoutExtension(FilePath); }
		}

		public IModuleDataSet ModuleDataSet {
			get { return _moduleDataSet; }
			set {
				if(_moduleDataSet != value) {
					_moduleDataSet = value;
					Media = new MediaCollection(_moduleDataSet);
					// The runtime behavior module instances will need data in the sequence's
					// data set.
					foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in RuntimeBehaviors) {
						_moduleDataSet.GetModuleTypeData(runtimeBehavior);
					}
				}
			}
		}

		public TimeSpan Length { get; set; }

		virtual public string FilePath { get; set; }

		public InsertDataListenerStack InsertDataListener { get; set; }

		public void InsertData(EffectNode effectNode)
		{
			InsertDataListener.InsertData(effectNode);
		}

		public void InsertData(IEnumerable<EffectNode> effectNodes)
		{
			InsertDataListener.InsertData(effectNodes);
		}

		public EffectNode InsertData(IEffectModuleInstance effect, TimeSpan startTime)
		{
			EffectNode cn = new EffectNode(effect, startTime);
			InsertData(cn);
			return cn;
		}

		public bool RemoveData(EffectNode effectNode)
		{
			return Data.RemoveData(effectNode);
		}

		public PreFilterNode AddPreFilter(IPreFilterModuleInstance preFilter, TimeSpan startTime) {
			PreFilterNode preFilterNode = new PreFilterNode(preFilter, startTime);
			Data.AddData(_preFilterStreamId, preFilterNode);
			return preFilterNode;
		}

		public bool RemovePreFilter(PreFilterNode preFilterNode) {
			return Data.RemoveData(preFilterNode);
		}

		public bool IsUntimed
		{
			get { return Length == Forever; }
			set { Length = value ? Forever : TimeSpan.Zero; }
		}

		public TimingProviders TimingProvider { get; protected set; }

		public DataStreams Data { get; private set; }

		// Every sequence will get a collection of all available runtime behaviors.
		public IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; private set; }

		public MediaCollection Media { get; set; }

		public IEnumerable<EffectNode> GetData() {
			return Data.GetMainStreamData().Cast<EffectNode>();
		}

		public IEnumerable<PreFilterNode> GetPreFilters() {
			return Data.GetStreamData(_preFilterStreamId).Cast<PreFilterNode>();
		}

		public ITiming GetTiming() {
			return TimingProvider.GetSelectedSource();
		}

		public override string ToString() {
			return Name;
		}

		virtual public int Version {
			get { return VERSION; }
		}
	}
}