using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Vixen.IO.Result;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSequenceListSerializer : IXmlSerializer<IEnumerable<ISequence>> {
		private const string ELEMENT_SEQUENCES = "Sequences";
		private const string ELEMENT_SEQUENCE = "Sequence";
		private const string ATTR_SEQUENCE_TYPE = "type";
		private const string ATTR_FILE_NAME = "fileName";

		public XElement WriteObject(IEnumerable<ISequence> value) {
			return new XElement(ELEMENT_SEQUENCES,
				value.Select(x =>
					new XElement(ELEMENT_SEQUENCE,
						new XAttribute(ATTR_SEQUENCE_TYPE, 
						new XAttribute(ATTR_FILE_NAME, Path.GetFileName(x.FilePath))))));
		}

		public IEnumerable<ISequence> ReadObject(XElement element) {
			List<ISequence> sequences = new List<ISequence>();

			XElement sequencesElement = element.Element(ELEMENT_SEQUENCES);
			if(sequencesElement != null) {
				foreach(XElement sequenceElement in sequencesElement.Elements(ELEMENT_SEQUENCE)) {
					string sequenceType = XmlHelper.GetAttribute(sequenceElement, ATTR_SEQUENCE_TYPE);
					if(sequenceType == null) continue;

					string fileName = XmlHelper.GetAttribute(sequenceElement, ATTR_FILE_NAME);
					if(fileName == null) continue;

					SequenceType type = (SequenceType)Enum.Parse(typeof(SequenceType), sequenceType);

					ISequence sequence;
					switch(type) {
						case SequenceType.Standard:
							FileSerializer<Sequence> sequenceSerializer = SerializerFactory.Instance.CreateStandardSequenceSerializer();
							SerializationResult<Sequence> sequenceResult = sequenceSerializer.Read(fileName);
							sequence = sequenceResult.Success ? sequenceResult.Object : null;
							break;
						case SequenceType.Script:
							FileSerializer<ScriptSequence> scriptSerializer = SerializerFactory.Instance.CreateScriptSequenceSerializer();
							SerializationResult<ScriptSequence> scriptResult = scriptSerializer.Read(fileName);
							sequence = scriptResult.Success ? scriptResult.Object : null;
							break;
						default:
							continue;
					}

					sequences.Add(sequence);
				}
			}

			return sequences;
		}
	}
}
