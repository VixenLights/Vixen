using Vixen.Sys;

namespace Vixen.IO {
	interface ISerializerFactory {
		FileSerializer<Sequence> CreateStandardSequenceSerializer();
		FileSerializer<ScriptSequence> CreateScriptSequenceSerializer();
		FileSerializer<SystemConfig> CreateSystemConfigSerializer();
		FileSerializer<ModuleStore> CreateModuleStoreSerializer();
		FileSerializer<SystemContext> CreateSystemContextSerializer();
		FileSerializer<Program> CreateProgramSerializer();
	}
}
