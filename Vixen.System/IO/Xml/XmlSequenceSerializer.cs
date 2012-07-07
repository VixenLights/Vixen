using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSequenceSerializer : IVersionedFileSerializer {
		private string _fileType;

		public int FileVersion { get; private set; }

		public int ClassVersion {
			get {
				// This cannot be known until a file is read or written.
				if(string.IsNullOrEmpty(_fileType)) throw new InvalidOperationException("Attempt to get sequence class version without a sequence type.");

				var sequenceType = SequenceTypeService.Instance.CreateSequenceFactory(_fileType);
				return sequenceType.ClassVersion;
			}
		}

		public object Read(string filePath) {
			_fileType = filePath;

			// Create a sequence instance.
			var sequenceType = SequenceTypeService.Instance.CreateSequenceFactory(filePath);
			ISequence sequence = sequenceType.CreateSequence();

			if(sequenceType.Descriptor.ModuleDataClass != null) {
				// Load the data.
				using(FileStream stream = new FileStream(filePath, FileMode.Open)) {
					DataContractSerializer serializer = new DataContractSerializer(sequenceType.Descriptor.ModuleDataClass, ApplicationServices.GetTypesOfModules().SelectMany(Modules.GetDescriptors).Select(x => x.ModuleDataClass).NotNull());
					var sequenceData = serializer.ReadObject(stream);
					if(!(sequenceData is ISequenceTypeDataModel)) {
						VixenSystem.Logging.Warning("Could not assign sequence data when reading sequence due to the object type.  File: " + filePath);
						FileVersion = 0;
					} else {
						sequence.SequenceData = (ISequenceTypeDataModel)sequenceData;
						FileVersion = sequence.SequenceData.Version;
					}
				}
			}

			return sequence;
		}

		public void Write(object value, string filePath) {
			_fileType = filePath;

			ISequence sequence = (ISequence)value;
			using(FileStream stream = new FileStream(filePath, FileMode.Create)) {
				DataContractSerializer serializer = new DataContractSerializer(sequence.SequenceData.GetType(), ApplicationServices.GetTypesOfModules().SelectMany(Modules.GetDescriptors).Select(x => x.ModuleDataClass).NotNull());
				sequence.SequenceData.Version = ClassVersion;
				//serializer.WriteObject(stream, sequence.SequenceData);
				using(XmlWriter xmlWriter = XmlWriter.Create(stream)) {
					serializer.WriteStartObject(xmlWriter, sequence.SequenceData);
					_WriteKnownNamespaces(xmlWriter);
					serializer.WriteObjectContent(xmlWriter, sequence.SequenceData);
					serializer.WriteEndObject(xmlWriter);
				}
			}
		}

		private void _WriteKnownNamespaces(XmlWriter xmlWriter) {
			List<string> namespaces = new List<string> {
				"http://www.w3.org/2001/XMLSchema"
			};

			int aliasIndex = 0;
			foreach(string ns in namespaces) {
				string alias = _GetAlias(aliasIndex++);
				xmlWriter.WriteAttributeString("xmlns", alias, null, ns);
			}
		}

		//http://stackoverflow.com/questions/297213/translate-an-index-into-an-excel-column-name
		private string _GetAlias(int aliasIndex) {
			int quotient = aliasIndex / 26;
			if(quotient > 0) {
				return _GetAlias(quotient - 1) + (char)(aliasIndex % 26 + 'a');
			} else {
				return ((char)(aliasIndex + 'a')).ToString();
			}
		}
	}
}
