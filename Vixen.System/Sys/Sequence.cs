using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.IO;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Media;
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
		private const string DIRECTORY_NAME = "Sequence";
		private const int VERSION = 1;

		[DataPath]
		static private readonly string _directory = System.IO.Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

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
			IReader reader = new XmlAnySequenceReader();
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
			ModuleDataSet = new ModuleLocalDataSet();
			
			FilePath = "";

			InsertDataListener = new InsertDataListenerStack();
			InsertDataListener += _DataListener;
			Data = new EffectStreams(this);
			TimingProvider = new TimingProviders(this);
			Media = new MediaCollection(ModuleDataSet);
			RuntimeBehaviors = Modules.ModuleManagement.GetAllRuntimeBehavior();
			// The runtime behavior module instances will need data in the sequence's
			// data set.
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in RuntimeBehaviors) {
				ModuleDataSet.GetModuleTypeData(runtimeBehavior);
			}
		}

		private bool _DataListener(EffectNode commandNode) {
			Data.AddEffect(commandNode);
			ModuleDataSet.GetModuleInstanceData(commandNode.Effect);
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

		public IModuleDataSet ModuleDataSet { get; private set; }

		public TimeSpan Length { get; set; }

		virtual public string FilePath { get; set; }

		public InsertDataListenerStack InsertDataListener { get; set; }

		public void InsertData(EffectNode effectNode)
		{
			InsertDataListener.InsertData(effectNode);
		}

		public EffectNode InsertData(IEffectModuleInstance effect, TimeSpan startTime)
		{
			EffectNode cn = new EffectNode(effect, startTime);
			InsertData(cn);
			return cn;
		}

		public bool IsUntimed
		{
			get { return Length == Forever; }
			set { Length = value ? Forever : TimeSpan.Zero; }
		}

		public TimingProviders TimingProvider { get; protected set; }

		public EffectStreams Data { get; private set; }

		// Every sequence will get a collection of all available runtime behaviors.
		public IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; private set; }

		public MediaCollection Media { get; set; }

		virtual public int Version {
			get { return VERSION; }
		}
	}
}