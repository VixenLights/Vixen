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
	}
}
