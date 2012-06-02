using System;
using Vixen.Sys;

namespace Vixen.IO {
	class SerializerFactory : ISerializerFactory {
		static private SerializerFactory _instance;
		static private ISerializerFactory _factory;

		private SerializerFactory() { }

		public static SerializerFactory Instance {
			get { return _instance ?? (_instance = new SerializerFactory()); }
		}

		public static ISerializerFactory Factory {
			get {
				if(_factory == null) throw new Exception("Serializer factory has not been set.");
				return _factory;
			}
			set { _factory = value; }
		}

		public FileSerializer<Sequence> CreateStandardSequenceSerializer() {
			return Factory.CreateStandardSequenceSerializer();
		}

		public FileSerializer<ScriptSequence> CreateScriptSequenceSerializer() {
			return Factory.CreateScriptSequenceSerializer();
		}

		public FileSerializer<SystemConfig> CreateSystemConfigSerializer() {
			return Factory.CreateSystemConfigSerializer();
		}

		public FileSerializer<ModuleStore> CreateModuleStoreSerializer() {
			return Factory.CreateModuleStoreSerializer();
		}

		public FileSerializer<SystemContext> CreateSystemContextSerializer() {
			return Factory.CreateSystemContextSerializer();
		}

		public FileSerializer<Program> CreateProgramSerializer() {
			return Factory.CreateProgramSerializer();
		}

		public FileSerializer<ChannelNodeTemplate> CreateChannelNodeTemplateSerializer() {
			return Factory.CreateChannelNodeTemplateSerializer();
		}

		public FileSerializer<OutputFilterTemplate> CreateOutputFilterTemplateSerializer() {
			return Factory.CreateOutputFilterTemplateSerializer();
		}
	}
}
