using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.IO;
using CommandStandard;
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

		protected virtual string Directory {
			get { return _directory; }
		}

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
		public const long Forever = long.MaxValue;

		protected Sequence() {
			ModuleDataSet = new ModuleDataSet();
			
			FilePath = "";

			InsertDataListener = new InsertDataListenerStack();
			InsertDataListener += _DataListener;
			Data = new InputChannels(this);
			TimingProvider = new TimingProviders(this);
			Media = new MediaCollection(ModuleDataSet);
			RuntimeBehaviors = Modules.ModuleManagement.GetAllRuntimeBehavior();
			// The runtime behavior module instances will need data in the sequence's
			// data set.
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in RuntimeBehaviors) {
				ModuleDataSet.GetModuleTypeData(runtimeBehavior);
			}
		}

		private bool _DataListener(CommandNode commandNode) {
			Data.AddCommand(commandNode);
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

		public long Length { get; set; }

		virtual public string FilePath { get; set; }

		public InsertDataListenerStack InsertDataListener { get; set; }

		public void InsertData(ChannelNode[] targetNodes, long startTime, long timeSpan, Command command) {
			InsertDataListener.InsertData(targetNodes, startTime, timeSpan, command);
		}

		public bool IsUntimed {
			get { return Length == Forever; }
			set { Length = value ? Forever : 0; }
		}

		public TimingProviders TimingProvider { get; protected set; }

		public InputChannels Data { get; private set; }

		// Every sequence will get a collection of all available runtime behaviors.
		public IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; private set; }

		public MediaCollection Media { get; set; }

		virtual public int Version {
			get { return VERSION; }
		}
	}
}