using Vixen.IO.Xml;

namespace Vixen.IO.Factory {
	class XmlSerializerFactory : ISerializerFactory {
		public IVersionedFileSerializer CreateSequenceSerializer(string fileType) {
			return new XmlSequenceSerializer();
		}

		public IVersionedFileSerializer CreateSystemConfigSerializer() {
			return new XmlSystemConfigSerializer();
		}

		public IVersionedFileSerializer CreateModuleStoreSerializer() {
			return new XmlModuleStoreSerializer();
		}

		public IVersionedFileSerializer CreateSystemContextSerializer() {
			return new XmlSystemContextSerializer();
		}

		public IVersionedFileSerializer CreateProgramSerializer() {
			return new XmlProgramSerializer();
		}

		public IVersionedFileSerializer CreateChannelNodeTemplateSerializer() {
			return new XmlChannelNodeTemplateSerializer();
		}

		public IVersionedFileSerializer CreateOutputFilterTemplateSerializer() {
			return new XmlOutputFilterTemplateSerializer();
		}
	}
}
