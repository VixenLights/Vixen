using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Common;
using Vixen.Sys;
using Vixen.Execution;
using Vixen.IO;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Module.RuntimeBehavior;

namespace Vixen.Module.Sequence {
	// This class exists as a facade implementation of the Sequence generic to provide the
	// reader and writer.

	//*** Leave ISequenceModuleInstance out of the base class, to be implemented by
	//    the sequence module explicitly
	/// <summary>
	/// Base class for sequence type module implementations.
	/// </summary>
	[Executor(typeof(SequenceExecutor))]
	//abstract public class SequenceBase : Sequence<SequenceReader, SequenceWriter, SequenceBase>, ISequenceModuleInstance {
	abstract public class SequenceBase : Sequence<SequenceReader, SequenceWriter, SequenceBase>, ISequenceModuleInstance {
		private const string DIRECTORY_NAME = "Sequence";

		// Has to be in the subclass because you can't perform the late-bound operations
		// on the generic base.
		[DataPath]
		static private readonly string _directory = System.IO.Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		protected override string Directory {
			get { return _directory; }
		}

		abstract public Guid TypeId { get; }

		public Guid InstanceId { get; set; }

		virtual public void Dispose() { }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		abstract public string FileExtension { get; }

		abstract protected XElement _WriteXml();
		abstract protected void _ReadXml(XElement sequenceElement);

		#region WriteXml
		static public XElement WriteXml(SequenceBase sequence) {
			Dictionary<Guid, int> effectTableIndex;
			Dictionary<Guid, int> channelIdTableIndex;

			XElement element = new XElement("Sequence",
				new XAttribute("length", sequence.Length),
				_WriteTimingSource(sequence),
				_WriteModuleData(sequence),
				_WriteIntervals(sequence),
				_WriteEffectTable(sequence, out effectTableIndex),
				_WriteChannelIdTable(sequence, out channelIdTableIndex),
				_WriteDataNodes(sequence, effectTableIndex, channelIdTableIndex),
				sequence._WriteXml());

			return element;
		}

		static private XElement _WriteTimingSource(SequenceBase sequence) {
			return new XElement("TimingSource", TimingProviders.WriteXml(sequence.TimingProvider));
		}

		static private XElement _WriteModuleData(SequenceBase sequence) {
			//*** this may cause problems
			return new XElement("ModuleData", sequence.ModuleDataSet.Serialize());
		}

		static private XElement _WriteIntervals(SequenceBase sequence) {
			return new XElement("Intervals", string.Join(",", sequence.Data.IntervalValues));
		}

		static private XElement _WriteEffectTable(SequenceBase sequence, out Dictionary<Guid, int> effectTableIndex) {
			// Effects are implemented by modules which are referenced by GUID ids.
			// To avoid having every serialized effect include a big fat GUID, which
			// would cause a lot of disk bloat, we're going to have a table of
			// command GUIDs that the data will reference by an index.

			List<Guid> effectTable;
			IEffectModuleInstance[] commands = VixenSystem.ModuleManagement.GetAllEffect();

			// All command specs in the system.
			effectTable = commands.Select(x => x.TypeId).ToList();
			// Command spec type id : index within the table
			effectTableIndex = effectTable.Select((id, index) => new { Id = id, Index = index }).ToDictionary(x => x.Id, x => x.Index);

			return new XElement("EffectTable", effectTable.Select(x => new XElement("Effect", x.ToString())));
		}

		static private XElement _WriteChannelIdTable(SequenceBase sequence, out Dictionary<Guid, int> channelIdTableIndex) {
			List<Guid> channelIdTable = Vixen.Sys.Execution.Fixtures.SelectMany(x => x.Channels).Select(x => x.Id).ToList();
			// Channel id : index within the table
			channelIdTableIndex = channelIdTable.Select((id, index) => new { Id = id, Index = index }).ToDictionary(x => x.Id, x => x.Index);

			return new XElement("ChannelIdTable", channelIdTable.Select(x => new XElement("Channel", x)));
		}

		static private XElement _WriteDataNodes(SequenceBase sequence, Dictionary<Guid, int> effectTableIndex, Dictionary<Guid, int> channelIdTableIndex) {
			// Going to serialize data by channel.  Each channel will be represented.
			// Intervals will be referenced by the time of each command serialized
			// within a given channel.
			// Store all in a binary stream converted to base-64.
			string data = null;

			using(MemoryStream stream = new System.IO.MemoryStream()) {
				using(BinaryWriter dataWriter = new BinaryWriter(stream)) {
					foreach(CommandNode commandNode in sequence.Data.GetCommands()) {
						// Index of the command spec id from the command table (word).
						dataWriter.Write((ushort)effectTableIndex[commandNode.Command.Effect.TypeId]);
						// Referenced channel count (word).
						dataWriter.Write((ushort)commandNode.TargetChannels.Length);
						// Parameter count (byte)
						dataWriter.Write((byte)commandNode.Command.ParameterValues.Length);

						// Start time (dword).
						dataWriter.Write(commandNode.StartTime);

						// Time span (dword).
						dataWriter.Write(commandNode.TimeSpan);

						// Referenced channels (index into channel table, word).
						foreach(Channel channel in commandNode.TargetChannels) {
							dataWriter.Write((ushort)channelIdTableIndex[channel.Id]);
						}

						dataWriter.Flush();

						// Parameters (various)
						foreach(object paramValue in commandNode.Command.ParameterValues) {
							ParameterValue.WriteToStream(stream, paramValue);
						}
					}
					data = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
				}
			}

			return new XElement("Data", data);
		}
		#endregion

		#region ReadXml
		//static public void ReadXml<T>(XElement element, T sequence)
		static public void ReadXml(XElement element, Vixen.Module.Sequence.SequenceBase sequence) {
			//where T : Vixen.Module.Sequence.SequenceBase {
			Guid[] effectTable;
			Guid[] channelIdTable;

			element = element.Element("Sequence");
			sequence.Length = int.Parse(element.Attribute("length").Value);

			// Timing
			_ReadTimingSource(element, sequence);

			// Module data
			_ReadModuleData(element, sequence);

			// Intervals
			_ReadIntervals(element, sequence);

			// Command table
			_ReadEffectTable(element, out effectTable);

			// Channel id table
			_ReadChannelIdTable(element, out channelIdTable);

			// Data nodes
			_ReadDataNodes(element, sequence, effectTable, channelIdTable);

			// Things that need to wait for other sequence data:

			// Runtime behavior module data
			_ReadBehaviorData(element, sequence);

			// Media module data
			_ReadMedia(element, sequence);

			sequence._ReadXml(element);
		}

		private static void _ReadTimingSource(XElement element, Vixen.Module.Sequence.SequenceBase sequence) {
			sequence.TimingProvider = TimingProviders.ReadXml(element, sequence);
		}

		private static void _ReadModuleData(XElement element, Vixen.Module.Sequence.SequenceBase sequence) {
			string moduleDataString = element.Element("ModuleData").Value;
			sequence.ModuleDataSet.Deserialize(moduleDataString);
		}

		private static void _ReadIntervals(XElement element, Vixen.Module.Sequence.SequenceBase sequence) {
			string[] intervalTimesString = element.Element("Intervals").Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			long[] intervalTimes = Array.ConvertAll(intervalTimesString, x => long.Parse(x));
			sequence.Data.InsertIntervals(intervalTimes);
		}

		private static void _ReadEffectTable(XElement element, out Guid[] effectTable) {
			effectTable = element
				.Element("EffectTable")
				.Elements("Effect")
				.Select(x => new Guid(x.Value))
				.ToArray();
		}

		private static void _ReadChannelIdTable(XElement element, out Guid[] channelIdTable) {
			channelIdTable = element.Element("ChannelIdTable").Elements("Channel").Select(x => new Guid(x.Value)).ToArray();
		}

		private static void _ReadDataNodes(XElement element, Vixen.Module.Sequence.SequenceBase sequence, Guid[] effectTable, Guid[] channelIdTable) {
			byte[] bytes = new byte[2 * sizeof(ushort) + sizeof(byte)];
			int effectIdIndex;
			int channelIdIndex;
			int channelIdCount;
			Guid effectId;
			List<OutputChannel> channels = new List<OutputChannel>();
			byte parameterCount;
			List<object> parameters = new List<object>();
			byte[] data;
			long dataLength;
			int startTime, timeSpan;

			// Data is stored as a base-64 stream from a binary stream.
			string dataString = element.Element("Data").Value;
			data = Convert.FromBase64String(dataString);
			using(MemoryStream dataStream = new MemoryStream(data)) {
				dataLength = dataStream.Length;
				while(dataStream.Position < dataLength) {
					channels.Clear();
					parameters.Clear();

					dataStream.Read(bytes, 0, bytes.Length);

					// Index of the command spec id from the command table (word).
					effectIdIndex = BitConverter.ToUInt16(bytes, 0 * sizeof(ushort));
					// Referenced channel count (word).
					channelIdCount = BitConverter.ToUInt16(bytes, 1 * sizeof(ushort));
					// Parameter count (byte)
					parameterCount = bytes[2 * sizeof(ushort)];

					// Start time (dword).
					dataStream.Read(bytes, 0, sizeof(int));
					startTime = BitConverter.ToInt32(bytes, 0);

					// Time span (dword).
					dataStream.Read(bytes, 0, sizeof(int));
					timeSpan = BitConverter.ToInt32(bytes, 0);

					// Referenced channels (index into channel table, word).
					var outputChannels = Vixen.Sys.Execution.Fixtures.SelectMany(x => x.Channels);
					OutputChannel channel;
					while(channelIdCount-- > 0) {
						dataStream.Read(bytes, 0, sizeof(ushort));
						channelIdIndex = BitConverter.ToUInt16(bytes, 0);
						// Channel may no longer exist.
						//channels.Add(sequence.OutputChannels.FirstOrDefault(x => x.Id == channelIdTable[channelIdIndex]));
						channel = outputChannels.FirstOrDefault(x => x.Id == channelIdTable[channelIdIndex]);
						if(channel != null) {
							channels.Add(channel);
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
							sequence.InsertData(channels.ToArray(), startTime, timeSpan, new Command(effectId, parameters.ToArray()));
						}
					}
				}
			}
		}

		private static void _ReadBehaviorData(XElement element, Vixen.Module.Sequence.SequenceBase sequence) {
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in sequence.RuntimeBehaviors) {
				sequence.ModuleDataSet.GetModuleTypeData(runtimeBehavior);
			}
		}

		private static void _ReadMedia(XElement element, Vixen.Module.Sequence.SequenceBase sequence) {
			foreach(IMediaModuleInstance media in sequence.Media) {
				sequence.ModuleDataSet.GetModuleInstanceData(media);
			}
		}

		#endregion
	}
}
