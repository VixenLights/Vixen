using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Vixen.Common;
using Vixen.IO;
using CommandStandard;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Media;
using Vixen.Module.Effect;
using Vixen.Module.Sequence;

namespace Vixen.Sys {
	/// <summary>
	/// Base class for any sequence implementation.
	/// </summary>
	[Executor(typeof(SequenceExecutor))]
	abstract public class Sequence : Vixen.Sys.ISequence {
		private const string DIRECTORY_NAME = "Sequence";

		[DataPath]
		static private readonly string _directory = System.IO.Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		protected virtual string Directory {
			get { return _directory; }
		}

		virtual protected XElement _WriteXml() { return null; }
		virtual protected void _ReadXml(XElement element) { }


		/// <summary>
		/// Loads an existing instance.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		static public Sequence Load(string filePath) {
			// Get the sequence module manager.
			SequenceModuleManagement manager = Modules.GetModuleManager<ISequenceModuleInstance, SequenceModuleManagement>();
			// Get an instance of the appropriate sequence module.
			Sequence instance = manager.Get(filePath) as Sequence;
			if(instance == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

			// Load the sequence.
			SequenceReader reader = new SequenceReader();
			reader.Read(filePath, instance);
			instance.FilePath = filePath;

			return instance;
		}

		static public string[] GetAllFileNames() {
			// We can't assume where all of the sequence file types will exist, so to provide
			// this functionality we will have to do the following:

			// Iterate all of the sequence type descriptors and build a set of file types.
			HashSet<string> fileTypes = new HashSet<string>();
			IEnumerable<ISequenceModuleDescriptor> sequenceDescriptors = Modules.GetModuleDescriptors<ISequenceModuleInstance, ISequenceModuleDescriptor>();
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
			Name = "Unnamed sequence";
			InsertDataListener = new InsertDataListenerStack();
			InsertDataListener += _DataListener;
			Data = new InputChannels(this);
			TimingProvider = new TimingProviders(this);
			Media = new MediaCollection(ModuleDataSet);
			RuntimeBehaviors = Modules.ModuleManagement.GetAllRuntimeBehavior();
			// The runtime behavior module instances will need data in the sequence's
			// data set.
			_LoadRuntimeBehaviorModuleData();
		}

		private bool _DataListener(CommandNode commandNode) {
			Data.AddCommand(commandNode);
			// Do not cancel the event.
			return false;
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");
			filePath = Path.Combine(this.Directory, Path.GetFileName(filePath));
			SequenceWriter writer = new SequenceWriter();
			writer.Write(filePath, this);
			this.FilePath = filePath;
		}

		public void Save() {
			Save(FilePath);
		}

		/// <summary>
		/// Set to the file name when deserialized and serialized. 
		/// </summary>
		public string Name { get; set; }

		public IModuleDataSet ModuleDataSet { get; private set; }

		public long Length { get; set; }

		public string FilePath { get; set; }

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

		public MediaCollection Media { get; private set; }

		private void _LoadRuntimeBehaviorModuleData() {
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in RuntimeBehaviors) {
				ModuleDataSet.GetModuleTypeData(runtimeBehavior);
			}
		}

		#region WriteXml
		static public XElement WriteXml(Sequence sequence) {
			Dictionary<Guid, int> effectTableIndex;
			Dictionary<Guid, int> targetIdTableIndex;

			XElement element = new XElement("Sequence",
				new XAttribute("length", sequence.Length),
				_WriteTimingSource(sequence),
				_WriteModuleData(sequence),
				//_WriteIntervals(sequence),
				_WriteEffectTable(sequence, out effectTableIndex),
				_WriteTargetIdTable(sequence, out targetIdTableIndex),
				_WriteDataNodes(sequence, effectTableIndex, targetIdTableIndex),
				_WriteImplementationContent(sequence));

			return element;
		}

		static private XElement _WriteTimingSource(Sequence sequence) {
			return new XElement("TimingSource", TimingProviders.WriteXml(sequence.TimingProvider));
		}

		static private XElement _WriteModuleData(Sequence sequence) {
			return new XElement("ModuleData", sequence.ModuleDataSet.Serialize());
		}

		static private XElement _WriteEffectTable(Sequence sequence, out Dictionary<Guid, int> effectTableIndex) {
			// Effects are implemented by modules which are referenced by GUID ids.
			// To avoid having every serialized effect include a big fat GUID, which
			// would cause a lot of disk bloat, we're going to have a table of
			// command GUIDs that the data will reference by an index.

			List<Guid> effectTable;
			IEffectModuleInstance[] effects = Modules.ModuleManagement.GetAllEffect();

			// All command specs in the system.
			effectTable = effects.Select(x => x.Descriptor.TypeId).ToList();
			// Command spec type id : index within the table
			effectTableIndex = effectTable.Select((id, index) => new { Id = id, Index = index }).ToDictionary(x => x.Id, x => x.Index);

			return new XElement("EffectTable", effectTable.Select(x => new XElement("Effect", x.ToString())));
		}

		static private XElement _WriteTargetIdTable(Sequence sequence, out Dictionary<Guid, int> targetIdTableIndex) {
			List<Guid> targetTable = Vixen.Sys.Execution.Nodes.Select(x => x.Id).ToList();
			// Channel id : index within the table
			targetIdTableIndex = targetTable.Select((id, index) => new { Id = id, Index = index }).ToDictionary(x => x.Id, x => x.Index);

			return new XElement("TargetIdTable", targetTable.Select(x => new XElement("Target", x)));
		}

		static private XElement _WriteDataNodes(Sequence sequence, Dictionary<Guid, int> effectTableIndex, Dictionary<Guid, int> targetIdTableIndex) {
			// Going to serialize data by channel.  Each channel will be represented.
			// Intervals will be referenced by the time of each command serialized
			// within a given channel.
			// Store all in a binary stream converted to base-64.
			string data = null;

			using(MemoryStream stream = new System.IO.MemoryStream()) {
				using(BinaryWriter dataWriter = new BinaryWriter(stream)) {
					foreach(CommandNode commandNode in sequence.Data.GetCommands()) {
						// Index of the command spec id from the command table (word).
						dataWriter.Write((ushort)effectTableIndex[commandNode.Command.EffectId]);
						// Referenced target count (word).
						dataWriter.Write((ushort)commandNode.TargetNodes.Length);
						// Parameter count (byte)
						dataWriter.Write((byte)commandNode.Command.ParameterValues.Length);

						// Start time (long).
						dataWriter.Write(commandNode.StartTime);

						// Time span (long).
						dataWriter.Write(commandNode.TimeSpan);

						// Referenced targets (index into target table, word).
						foreach(ChannelNode node in commandNode.TargetNodes) {
							dataWriter.Write((ushort)targetIdTableIndex[node.Id]);
						}

						dataWriter.Flush();

						// Parameters (various)
						foreach(object parameterValue in commandNode.Command.ParameterValues) {
							ParameterValue.WriteToStream(stream, parameterValue);
						}
					}
					data = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
				}
			}

			return new XElement("Data", data);
		}

		static private XElement _WriteImplementationContent(Sequence sequence) {
			return new XElement("Implementation", sequence._WriteXml());
		}
		#endregion

		#region ReadXml
		static public void ReadXml(XElement element, Sequence sequence) {
			Guid[] effectTable;
			Guid[] targetIdTable;

			//Already referencing the doc element.
			sequence.Length = long.Parse(element.Attribute("length").Value);

			// Timing
			_ReadTimingSource(element, sequence);

			// Module data
			_ReadModuleData(element, sequence);

			// Command table
			_ReadEffectTable(element, out effectTable);

			// Target id table
			_ReadTargetIdTable(element, out targetIdTable);

			// Data nodes
			_ReadDataNodes(element, sequence, effectTable, targetIdTable);

			// Things that need to wait for other sequence data:

			// Runtime behavior module data
			_ReadBehaviorData(element, sequence);

			// Media module data
			_ReadMedia(element, sequence);

			// Subclass implementation data
			_ReadImplementationContent(element, sequence);
		}

		private static void _ReadTimingSource(XElement element, Sequence sequence) {
			element = element.Element("TimingSource");
			sequence.TimingProvider = TimingProviders.ReadXml(element, sequence);
		}

		private static void _ReadModuleData(XElement element, Sequence sequence) {
			string moduleDataString = element.Element("ModuleData").Value;
			sequence.ModuleDataSet.Deserialize(moduleDataString);
		}

		private static void _ReadEffectTable(XElement element, out Guid[] effectTable) {
			effectTable = element
				.Element("EffectTable")
				.Elements("Effect")
				.Select(x => new Guid(x.Value))
				.ToArray();
		}

		private static void _ReadTargetIdTable(XElement element, out Guid[] targetIdTable) {
			targetIdTable = element.Element("TargetIdTable").Elements("Target").Select(x => new Guid(x.Value)).ToArray();
		}

		private static void _ReadDataNodes(XElement element, Sequence sequence, Guid[] effectTable, Guid[] targetIdTable) {
			byte[] bytes = new byte[sizeof(long)];
			int effectIdIndex;
			int targetIdIndex;
			int targetIdCount;
			Guid effectId;
			List<ChannelNode> nodes = new List<ChannelNode>();
			byte parameterCount;
			List<object> parameters = new List<object>();
			byte[] data;
			long dataLength;
			long startTime, timeSpan;

			// Data is stored as a base-64 stream from a binary stream.
			string dataString = element.Element("Data").Value;
			data = Convert.FromBase64String(dataString);
			using(MemoryStream dataStream = new MemoryStream(data)) {
				dataLength = dataStream.Length;
				while(dataStream.Position < dataLength) {
					nodes.Clear();
					parameters.Clear();

					// Index of the command spec id from the command table (word).
					dataStream.Read(bytes, 0, sizeof(ushort));
					effectIdIndex = BitConverter.ToUInt16(bytes, 0);
					// Referenced channel count (word).
					dataStream.Read(bytes, 0, sizeof(ushort));
					targetIdCount = BitConverter.ToUInt16(bytes, 0);
					// Parameter count (byte)
					parameterCount = (byte)dataStream.ReadByte();

					// Start time (long).
					dataStream.Read(bytes, 0, sizeof(long));
					startTime = BitConverter.ToInt64(bytes, 0);

					// Time span (long).
					dataStream.Read(bytes, 0, sizeof(long));
					timeSpan = BitConverter.ToInt64(bytes, 0);

					// Referenced nodes (index into target table, word).
					var targets = Vixen.Sys.Execution.Nodes; // ChannelNodes
					ChannelNode node;
					while(targetIdCount-- > 0) {
						dataStream.Read(bytes, 0, sizeof(ushort));
						targetIdIndex = BitConverter.ToUInt16(bytes, 0);
						// Channel may no longer exist.
						node = targets.FirstOrDefault(x => x.Id == targetIdTable[targetIdIndex]);
						if(node != null) {
							nodes.Add(node);
						}
					}
					// Parameters (various)
					while(parameterCount-- > 0) {
						parameters.Add(ParameterValue.ReadFromStream(dataStream));
					}

					// Get the effect inserted into the sequence.
					if(effectIdIndex < effectTable.Length) {
						effectId = effectTable[effectIdIndex];
						if(Modules.IsValidId(effectId)) {
							sequence.InsertData(nodes.ToArray(), startTime, timeSpan, new Command(effectId, parameters.ToArray()));
						}
					}
				}
			}
		}

		private static void _ReadBehaviorData(XElement element, Sequence sequence) {
			sequence._LoadRuntimeBehaviorModuleData();
		}

		private static void _ReadMedia(XElement element, Sequence sequence) {
			sequence.Media = new MediaCollection(sequence.ModuleDataSet);
		}

		private static void _ReadImplementationContent(XElement element, Sequence sequence) {
			sequence._ReadXml(element.Element("Implementation"));
		}

		#endregion
	}
}