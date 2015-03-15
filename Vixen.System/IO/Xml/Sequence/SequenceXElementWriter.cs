using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using Vixen.Module.SequenceType;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.IO.Xml.Sequence
{
	internal class SequenceXElementWriter : IObjectContentWriter
	{
		private string _fileType;

		public SequenceXElementWriter(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException("filePath");

			_fileType = FileService.Instance.GetFileType(filePath);
		}

		public void WriteContentToObject(object content, object obj)
		{
			XElement xmlContent = content as XElement;
			ISequence sequence = obj as ISequence;

			if (xmlContent == null) throw new InvalidOperationException("Content must be an XElement.");
			if (sequence == null) throw new InvalidOperationException("Object must be an ISequence.");

			sequence.SequenceData = _GenerateSequenceData(xmlContent);
		}

		public int GetContentVersion(object content)
		{
			XElement xmlContent = content as XElement;
			if (xmlContent == null) throw new InvalidOperationException("Content must be an XElement.");

			int version = XmlRootAttributeVersion.GetVersion(xmlContent);

			return version > 0 ? version : 0;
		}

		private ISequenceTypeDataModel _GenerateSequenceData(XElement xmlContent)
		{
			ISequenceTypeModuleInstance sequenceTypeModule = _GetSequenceTypeModule(_fileType);
			return _GenerateSequenceDataModel(sequenceTypeModule, xmlContent);
		}

		private ISequenceTypeModuleInstance _GetSequenceTypeModule(string fileType)
		{
			return SequenceTypeService.Instance.CreateSequenceFactory(fileType);
		}

		private ISequenceTypeDataModel _GenerateSequenceDataModel(ISequenceTypeModuleInstance sequenceTypeModule,
		                                                          XElement xmlContent)
		{
			DataContractSerializer serializer = SequenceTypeService.GetSequenceTypeDataSerializer(sequenceTypeModule);
			if (serializer == null) return null;

			using (XmlReader xmlReader = xmlContent.CreateReader()) {
				object sequenceData = serializer.ReadObject(xmlReader);
				return (ISequenceTypeDataModel) sequenceData;
			}
		}
	}
}