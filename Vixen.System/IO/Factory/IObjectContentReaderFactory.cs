namespace Vixen.IO.Factory {
	interface IObjectContentReaderFactory {
		IObjectContentReader CreateSystemConfigContentReader();
		IObjectContentReader CreateModuleStoreContentReader();
		IObjectContentReader CreateSystemContextContentReader();
		IObjectContentReader CreateProgramContentReader();
		IObjectContentReader CreateElementNodeTemplateContentReader();
		IObjectContentReader CreateSequenceContentReader(string fileType);
	}
}
