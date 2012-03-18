using Vixen.IO.Xml;
using Vixen.Sys;

namespace Vixen.IO.Factory {
	class XmlSerializerFactory : ISerializerFactory {
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

		public FileSerializer<ChannelNodeTemplate> CreateChannelNodeTemplateSerializer() {
			return new XmlChannelNodeTemplateSerializer();
		}

		public FileSerializer<PostFilterTemplate> CreatePostFilterTemplateSerializer() {
			return new XmlPostFilterTemplateSerializer();
		}

	}
}
