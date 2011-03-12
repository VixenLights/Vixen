using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Sequence;
using Vixen.Module.Effect;
using Vixen.Module.Input;

namespace Vixen.IO {
	public class SequenceReader : SequenceReader<ISequence> {
		protected override void ReadSequenceAttributes(XmlReader reader, ISequence sequence) { }
		protected override void ReadSequenceBody(XmlReader reader, ISequence sequence) { }
	}

	abstract public class SequenceReader<T> : ISequenceReader<T>
		where T : class, ISequence {
		protected T Sequence { get; private set; }

		public bool Read(string filePath, ISequence sequence) {
			return Read(filePath, sequence as T);
		}

		public bool Read(string filePath, T sequence) {
			if(sequence != null) {
				this.Sequence = sequence;
				using(FileStream stream = new FileStream(filePath, FileMode.Open)) {
					XmlReaderSettings settings = new XmlReaderSettings();
					settings.IgnoreWhitespace = true;
					using(XmlReader reader = XmlReader.Create(stream, settings)) {
						try {
							reader.Read(); // Need to start with this to seed the parser.

							if(reader.NodeType == XmlNodeType.XmlDeclaration) {
								reader.Read();
							}

							// A sequence's name is coupled to its file name.
							sequence.Name = Path.GetFileName(filePath);

							// Type is already set by the subclass.
							sequence.TimingSourceId = new Guid(reader.GetAttribute("timingSourceId"));
							sequence.Length = int.Parse(reader.GetAttribute("length"));

							ReadSequenceAttributes(reader, sequence);

							if(reader.ElementsExistWithin("Sequence")) { // Entity element
								// Module data
								_ReadModuleData(reader, sequence);

								// Fixtures
								_ReadFixtures(reader, sequence);

								// Intervals
								_ReadIntervals(reader, sequence);

								// Command table
								Guid[] commandTable;
								_ReadEffectTable(reader, sequence, out commandTable);

								// Channel id table
								Guid[] channelIdTable;
								_ReadChannelIdTable(reader, sequence, out channelIdTable);

								// Data nodes
								_ReadDataNodes(reader, sequence, commandTable, channelIdTable);

								ReadSequenceBody(reader, sequence);

								reader.ReadEndElement(); // Sequence
							}

							return true;
						} catch {
						}
						return false;
					}
				}
			} else {
				return false;
			}
		}

		private void _ReadEffectTable(XmlReader reader, T sequence, out Guid[] effectTable) {
			List<Guid> effectIds = new List<Guid>();
			reader.ReadStartElement("EffectTable");
			while(reader.IsStartElement("Effect")) {
				effectIds.Add(new Guid(reader.ReadElementString()));
			}
			reader.ReadEndElement(); // Effect

			effectTable = effectIds.ToArray();
		}

		private void _ReadChannelIdTable(XmlReader reader, T sequence, out Guid[] channelIdTable) {
			List<Guid> channelIds = new List<Guid>();
			reader.ReadStartElement("ChannelIdTable");
			while(reader.IsStartElement("Channel")) {
				channelIds.Add(new Guid(reader.ReadElementString()));
			}
			reader.ReadEndElement(); // ChannelIdTable

			channelIdTable = channelIds.ToArray();
		}

		private void _ReadIntervals(XmlReader reader, T sequence) {
			string intervalTimesString = reader.ReadElementString("Intervals");
			int[] intervalTimes = Array.ConvertAll(intervalTimesString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), x => int.Parse(x));
			sequence.Data.InsertIntervals(intervalTimes);
		}

		private void _ReadFixtures(XmlReader reader, T sequence) {
			if(reader.ElementsExistWithin("Fixtures")) { // Container element for child entity
				FixtureReader fixtureReader = new FixtureReader();
				while(fixtureReader.Read(reader)) {
					sequence.InsertFixture(fixtureReader.Fixture, false);
				}
				reader.ReadEndElement(); // Fixtures
			}
		}

		private void _ReadModuleData(XmlReader reader, T sequence) {
			reader.ReadStartElement("ModuleData");
			sequence.ModuleDataSet.Deserialize(reader.ReadOuterXml());
			reader.ReadEndElement(); // ModuleData
		}

		private void _ReadDataNodes(XmlReader reader, T sequence, Guid[] commandTable, Guid[] channelIdTable) {
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
			string dataString = reader.ReadElementString("Data");
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
					while(channelIdCount-- > 0) {
						dataStream.Read(bytes, 0, sizeof(ushort));
						channelIdIndex = BitConverter.ToUInt16(bytes, 0);
						channels.Add(sequence.OutputChannels.FirstOrDefault(x => x.Id == channelIdTable[channelIdIndex]));
					}
					// Parameters (various)
					while(parameterCount-- > 0) {
						parameters.Add(ParameterValue.ReadFromStream(dataStream));
					}

					if(effectIdIndex < commandTable.Length) {
						effectId = commandTable[effectIdIndex];
						if(Modules.IsValidId(effectId)) {
							sequence.InsertData(channels.ToArray(), startTime, timeSpan, new Command(effectId, parameters.ToArray()));
						}
					}
				}
			}
		}

		// To avoid the confusion of whether or not the Sequence element
		// is still available for attributes or not.
		abstract protected void ReadSequenceAttributes(XmlReader reader, T sequence);
		abstract protected void ReadSequenceBody(XmlReader reader, T sequence);
	}
}
