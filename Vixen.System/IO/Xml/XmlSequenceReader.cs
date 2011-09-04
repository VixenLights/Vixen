using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Module.Sequence;
using Vixen.Module.RuntimeBehavior;

namespace Vixen.IO.Xml {
	class XmlSequenceReader : XmlReaderBase<Sequence> {
		private const string ELEMENT_SEQUENCE = "Sequence";
		private const string ATTR_LENGTH = "length";
		private const string ELEMENT_TIMING_SOURCE = "TimingSource";
		private const string ELEMENT_MODULE_DATA = "ModuleData";
		private const string ELEMENT_EFFECT_TABLE = "EffectTable";
		private const string ELEMENT_EFFECT = "Effect";
		private const string ELEMENT_TARGET_ID_TABLE = "TargetIdTable";
		private const string ELEMENT_TARGET = "Target";
		private const string ELEMENT_DATA = "Data";
		private const string ELEMENT_IMPLEMENTATION_CONTENT = "Implementation";
		private const string ELEMENT_SELECTED_TIMING = "Selected";
		private const string ATTR_SELECTED_TIMING_TYPE = "type";
		private const string ATTR_SELECTED_TIMING_SOURCE = "source";

		override protected Sequence _CreateObject(XElement element, string filePath) {
			// Get the specific sequence module manager.
			SequenceModuleManagement manager = Modules.GetManager<ISequenceModuleInstance, SequenceModuleManagement>();
			// Get an instance of the appropriate sequence module.
			Sequence sequence = manager.Get(filePath) as Sequence;
			if(sequence == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

			sequence.FilePath = filePath;

			return sequence;
		}

		private void _ReadTimingSource(XElement element, Sequence sequence) {
			element = element.Element(ELEMENT_TIMING_SOURCE).Element(ELEMENT_SELECTED_TIMING);

			string providerType = element.Attribute(ATTR_SELECTED_TIMING_TYPE).Value;
			string sourceName = element.Attribute(ATTR_SELECTED_TIMING_SOURCE).Value;

			if(providerType.Length == 0) providerType = null;
			if(sourceName.Length == 0) sourceName = null;

			sequence.TimingProvider.SetSelectedSource(providerType, sourceName);
		}

		private void _ReadModuleData(XElement element, Sequence sequence) {
			string moduleDataString = element.Element(ELEMENT_MODULE_DATA).InnerXml();
			sequence.ModuleDataSet.Deserialize(moduleDataString);
		}

		private void _ReadEffectTable(XElement element, out Guid[] effectTable) {
			effectTable = element
				.Element(ELEMENT_EFFECT_TABLE)
				.Elements(ELEMENT_EFFECT)
				.Select(x => new Guid(x.Value))
				.ToArray();
		}

		private void _ReadTargetIdTable(XElement element, out Guid[] targetIdTable) {
			targetIdTable = element
				.Element(ELEMENT_TARGET_ID_TABLE)
				.Elements(ELEMENT_TARGET)
				.Select(x => new Guid(x.Value))
				.ToArray();
		}

		private void _ReadDataNodes(XElement element, Sequence sequence, Guid[] effectTable, Guid[] targetIdTable) {
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
			string dataString = element.Element(ELEMENT_DATA).Value;
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

		private  void _ReadBehaviorData(XElement element, Sequence sequence) {
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in sequence.RuntimeBehaviors) {
				sequence.ModuleDataSet.GetModuleTypeData(runtimeBehavior);
			}
		}

		private  void _ReadMedia(XElement element, Sequence sequence) {
			sequence.Media = new MediaCollection(sequence.ModuleDataSet);
		}

		private void _ReadImplementationContent(XElement element, Sequence sequence) {
			element = element.Element(ELEMENT_IMPLEMENTATION_CONTENT);
			_ReadContent(element, sequence);
		}

		virtual protected void _ReadContent(XElement element, Sequence sequence) { }

		protected override void _PopulateObject(Sequence obj, XElement element) {
			Guid[] effectTable;
			Guid[] targetIdTable;

			//Already referencing the doc element.
			obj.Length = long.Parse(element.Attribute("length").Value);

			// Timing
			_ReadTimingSource(element, obj);

			// Module data
			_ReadModuleData(element, obj);

			// Command table
			_ReadEffectTable(element, out effectTable);

			// Target id table
			_ReadTargetIdTable(element, out targetIdTable);

			// Data nodes
			_ReadDataNodes(element, obj, effectTable, targetIdTable);

			// Things that need to wait for other sequence data:

			// Runtime behavior module data
			_ReadBehaviorData(element, obj);

			// Media module data
			_ReadMedia(element, obj);

			// Subclass implementation data
			_ReadImplementationContent(element, obj);
		}

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			return new Func<XElement, XElement>[] { };
		}
	}
}
