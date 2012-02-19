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

		public IFileSerializer<Sequence, SequenceSerializationResult> CreateStandardSequenceSerializer() {
			return new XmlSequenceSerializer();
		}

		public IFileSerializer<ScriptSequence, ScriptSequenceSerializationResult> CreateScriptSequenceSerializer() {
			return new XmlScriptSequenceSerializer();
		}
	}
}
