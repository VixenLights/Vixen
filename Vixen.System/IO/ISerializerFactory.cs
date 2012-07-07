namespace Vixen.IO {
	interface ISerializerFactory {
		IVersionedFileSerializer CreateSequenceSerializer(string fileType);
		IVersionedFileSerializer CreateSystemConfigSerializer();
		IVersionedFileSerializer CreateModuleStoreSerializer();
		IVersionedFileSerializer CreateSystemContextSerializer();
		IVersionedFileSerializer CreateProgramSerializer();
		IVersionedFileSerializer CreateChannelNodeTemplateSerializer();
		IVersionedFileSerializer CreateOutputFilterTemplateSerializer();
	}
}
