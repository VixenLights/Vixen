using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Sys;
using Vixen.Common;
using System.IO;
using Vixen.Sequence;
using Vixen.Module.CommandSpec;
using Vixen.Module.Input;

namespace Vixen.IO {
	public class SequenceWriter : SequenceWriter<ISequence> {
		protected override void WriteSequenceAttributes(XmlWriter writer) { }
		protected override void WriteSequenceBody(XmlWriter writer) { }
	}

	abstract public class SequenceWriter<T> : ISequenceWriter<T>
		where T : class, ISequence {
		protected T Sequence { get; private set; }

		public void Write(string filePath, ISequence sequence) {
			Write(filePath, sequence as T);
		}

		public void Write(string filePath, T sequence) {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			using(XmlWriter writer = XmlWriter.Create(filePath, settings)) {
				if(sequence != null) {
					this.Sequence = sequence;

					// A sequence's name is coupled to its file name.
					sequence.Name = System.IO.Path.GetFileName(filePath);

					writer.WriteStartElement("Sequence");
					writer.WriteAttributeString("timingSourceId", Sequence.TimingSourceId.ToString());
					writer.WriteAttributeString("length", Sequence.Length.ToString());

					WriteSequenceAttributes(writer);

					// Module data
					_WriteModuleData(writer);

					// Fixtures
					_WriteFixtures(writer);

					// Intervals
					_WriteIntervals(writer);

					// Command table
					Dictionary<Guid, int> commandTableIndex;
					_WriteCommandTable(writer, out commandTableIndex);

					// Channel id table
					Dictionary<Guid, int> channelIdTableIndex;
					_WriteChannelIdTable(writer, out channelIdTableIndex);

					// Data nodes
					_WriteDataNodes(writer, commandTableIndex, channelIdTableIndex);

					WriteSequenceBody(writer);

					writer.WriteEndElement(); // Sequence
				}
			}
		}

		private void _WriteIntervals(XmlWriter writer) {
			writer.WriteElementString("Intervals", string.Join(",", Sequence.Data.IntervalValues));
		}

		private void _WriteFixtures(XmlWriter writer) {
			FixtureWriter fixtureWriter = new FixtureWriter();
			writer.WriteStartElement("Fixtures");
			foreach(Fixture fixture in Sequence.Fixtures) {
				fixtureWriter.Fixture = fixture;
				fixtureWriter.Write(writer);
			}
			writer.WriteEndElement(); // Fixtures
		}

		private void _WriteModuleData(XmlWriter writer) {
			writer.WriteStartElement("ModuleData");
			writer.WriteRaw(Sequence.ModuleDataSet.Serialize());
			writer.WriteEndElement(); // ModuleData
		}

		private void _WriteCommandTable(XmlWriter writer, out Dictionary<Guid, int> commandTableIndex) {
			// Commands are implemented by modules which are referenced by GUID ids.
			// To avoid having every serialized command include a big fat GUID, which
			// would cause a lot of disk bloat, we're going to have a table of
			// command GUIDs that the data will reference by an index.

			List<Guid> commandTable;
			ICommandSpecModuleInstance[] commands = Server.ModuleManagement.GetAllCommandSpec();

			// All command specs in the system.
			commandTable = commands.Select(x => x.TypeId).ToList();
			// Command spec type id : index within the table
			commandTableIndex = commandTable.Select((id, index) => new { Id = id, Index = index }).ToDictionary(x => x.Id, x => x.Index);
			writer.WriteStartElement("CommandTable");
			foreach(Guid commandSpecTypeId in commandTable) {
				writer.WriteElementString("CommandSpec", commandSpecTypeId.ToString());
			}
			writer.WriteEndElement(); // CommandTable
		}

		private void _WriteChannelIdTable(XmlWriter writer, out Dictionary<Guid, int> channelIdTableIndex) {
			List<Guid> channelIdTable = Sequence.OutputChannels.Select(x => x.Id).ToList();
			// Channel id : index within the table
			channelIdTableIndex = channelIdTable.Select((id, index) => new { Id = id, Index = index }).ToDictionary(x => x.Id, x => x.Index);
			writer.WriteStartElement("ChannelIdTable");
			foreach(Guid channelId in channelIdTable) {
				writer.WriteElementString("Channel", channelId.ToString());
			}
			writer.WriteEndElement(); // ChannelIdTable
		}

		private void _WriteDataNodes(XmlWriter writer, Dictionary<Guid, int> commandTableIndex, Dictionary<Guid, int> channelIdTableIndex) {
			// Going to serialize data by channel.  Each channel will be represented.
			// Intervals will be referenced by the time of each command serialized
			// within a given channel.
			writer.WriteStartElement("Data");
			// Store all in a binary stream converted to base-64.
			using(MemoryStream stream = new System.IO.MemoryStream()) {
				using(BinaryWriter dataWriter = new BinaryWriter(stream)) {
					//foreach(CommandNode commandNode in Sequence.Commands) {
					foreach(CommandNode commandNode in Sequence.Data.GetCommands()) {
						// Index of the command spec id from the command table (word).
						dataWriter.Write((ushort)commandTableIndex[commandNode.Command.Spec.TypeId]);
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
					writer.WriteString(Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length));
				}
			}
			writer.WriteEndElement(); // Data
		}

		// To avoid the confusion of whether or not the Sequence element
		// is still available for attributes or not.
		abstract protected void WriteSequenceAttributes(XmlWriter writer);
		abstract protected void WriteSequenceBody(XmlWriter writer);
	}
}
