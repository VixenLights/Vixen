using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module.Effect;

namespace Vixen.IO.Xml {
	class XmlSequenceWriter : IWriter {
		private const string ELEMENT_SEQUENCE = "Sequence";
		private const string ELEMENT_TIMING_SOURCE = "TimingSource";
		private const string ELEMENT_MODULE_DATA = "ModuleData";
		private const string ELEMENT_EFFECT_TABLE = "EffectTable";
		private const string ELEMENT_EFFECT = "Effect";
		private const string ELEMENT_TARGET_ID_TABLE = "TargetIdTable";
		private const string ELEMENT_TARGET = "Target";
		private const string ELEMENT_DATA = "Data";
		private const string ELEMENT_IMPLEMENTATION_CONTENT = "Implementation";
		private const string ELEMENT_SELECTED_TIMING = "Selected";
		private const string ATTR_LENGTH = "length";
		private const string ATTR_SELECTED_TIMING_TYPE = "type";
		private const string ATTR_SELECTED_TIMING_SOURCE = "source";

		public void Write(string filePath, object value) {
			if(!(value is Sequence)) throw new InvalidOperationException("Attempt to serialize a " + value.GetType().ToString() + " as a Sequence.");

			Sequence controller = (Sequence)value;
			XElement doc = CreateContent(controller);
			doc.Save(filePath);
		}

		public XElement CreateContent(Sequence sequence) {
			Dictionary<Guid, int> effectTableIndex;
			Dictionary<Guid, int> targetIdTableIndex;

			XElement element = new XElement(ELEMENT_SEQUENCE,
				new XAttribute(ATTR_LENGTH, sequence.Length),
				_WriteTimingSource(sequence),
				_WriteModuleData(sequence),
				_WriteEffectTable(sequence, out effectTableIndex),
				_WriteTargetIdTable(sequence, out targetIdTableIndex),
				_WriteDataNodes(sequence, effectTableIndex, targetIdTableIndex),
				_WriteImplementationContent(sequence));

			return element;
		}

		private XElement _WriteTimingSource(Sequence sequence) {
			string providerType;
			string sourceName;

			sequence.TimingProvider.GetSelectedSource(out providerType, out sourceName);

			return new XElement(ELEMENT_TIMING_SOURCE,
				new XElement(ELEMENT_SELECTED_TIMING,
					new XAttribute(ATTR_SELECTED_TIMING_TYPE, providerType ?? string.Empty),
					new XAttribute(ATTR_SELECTED_TIMING_SOURCE, sourceName ?? string.Empty)));
		}

		private XElement _WriteModuleData(Sequence sequence) {
			return new XElement(ELEMENT_MODULE_DATA, sequence.ModuleDataSet.ToXElement());
		}

		private XElement _WriteEffectTable(Sequence sequence, out Dictionary<Guid, int> effectTableIndex) {
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

			return new XElement(ELEMENT_EFFECT_TABLE, effectTable.Select(x => new XElement(ELEMENT_EFFECT, x.ToString())));
		}

		private XElement _WriteTargetIdTable(Sequence sequence, out Dictionary<Guid, int> targetIdTableIndex) {
			List<Guid> targetTable = Vixen.Sys.Execution.Nodes.Select(x => x.Id).ToList();
			// Channel id : index within the table
			targetIdTableIndex = targetTable.Select((id, index) => new { Id = id, Index = index }).ToDictionary(x => x.Id, x => x.Index);

			return new XElement(ELEMENT_TARGET_ID_TABLE, targetTable.Select(x => new XElement(ELEMENT_TARGET, x)));
		}

		private XElement _WriteDataNodes(Sequence sequence, Dictionary<Guid, int> effectTableIndex, Dictionary<Guid, int> targetIdTableIndex) {
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

			return new XElement(ELEMENT_DATA, data);
		}

		private XElement _WriteImplementationContent(Sequence sequence) {
			return new XElement(ELEMENT_IMPLEMENTATION_CONTENT, _WriteContent(sequence));
		}

		virtual protected XElement _WriteContent(Sequence sequence) {
			return null;
		}

	}
}
