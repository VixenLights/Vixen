using System;

namespace Vixen.IO {
	class SerializerFactory : ISerializerFactory {
		static private SerializerFactory _instance;
		static private ISerializerFactory _factory;

		private SerializerFactory() { }

		public static SerializerFactory Instance {
			get { return _instance ?? (_instance = new SerializerFactory()); }
		}

		internal static ISerializerFactory Factory {
			get {
				if(_factory == null) throw new Exception("Serializer factory has not been set.");
				return _factory;
			}
			set { _factory = value; }
		}

		public IVersionedFileSerializer CreateSequenceSerializer(string fileType) {
			return Factory.CreateSequenceSerializer(fileType);
		}

		public IVersionedFileSerializer CreateSystemConfigSerializer() {
			return Factory.CreateSystemConfigSerializer();
		}

		public IVersionedFileSerializer CreateModuleStoreSerializer() {
			return Factory.CreateModuleStoreSerializer();
		}

		public IVersionedFileSerializer CreateSystemContextSerializer() {
			return Factory.CreateSystemContextSerializer();
		}

		public IVersionedFileSerializer CreateProgramSerializer() {
			return Factory.CreateProgramSerializer();
		}

		public IVersionedFileSerializer CreateChannelNodeTemplateSerializer() {
			return Factory.CreateChannelNodeTemplateSerializer();
		}
	}
}
