using Vixen.IO.Xml;
using Vixen.Sys;

namespace Vixen.IO {
	class SerializerFactory {
		static private SerializerFactory _instance;

		private SerializerFactory() {
		}

		public static SerializerFactory Instance {
			get { return _instance ?? (_instance = new SerializerFactory()); }
		}

		public FileSerializer<Sequence> CreateStandardSequenceSerializer() {
			return new XmlSequenceSerializer();
		}

		public FileSerializer<ScriptSequence> CreateScriptSequenceSerializer() {
			return new XmlScriptSequenceSerializer();
		}

		public FileSerializer<SystemConfig> CreateSystemConfigSerializer() {
			return new XmlSystemConfigSerializer();
		}

		public FileSerializer<ModuleStore> CreateModuleStoreSerializer() {
			return new XmlModuleStoreSerializer();
		}

		public FileSerializer<SystemContext> CreateSystemContextSerializer() {
			return new XmlSystemContextSerializer();
		}

		public FileSerializer<Program> CreateProgramSerializer() {
			return new XmlProgramSerializer();
		}
	}
}
