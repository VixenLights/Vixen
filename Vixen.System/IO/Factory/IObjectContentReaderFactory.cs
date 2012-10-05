namespace Vixen.IO.Factory {
	interface IObjectContentReaderFactory {
		IObjectContentReader CreateSystemConfigContentReader();
		IObjectContentReader CreateModuleStoreContentReader();
		IObjectContentReader CreateSystemContextContentReader();
		IObjectContentReader CreateProgramContentReader();
		IObjectContentReader CreateChannelNodeTemplateContentReader();
		IObjectContentReader CreateSequenceContentReader(string fileType);
	}
}
