namespace Vixen.IO.Factory {
	interface IObjectContentWriterFactory {
		IObjectContentWriter CreateSystemConfigContentWriter();
		IObjectContentWriter CreateModuleStoreContentWriter();
		IObjectContentWriter CreateSystemContextContentWriter();
		IObjectContentWriter CreateProgramContentWriter();
		IObjectContentWriter CreateElementNodeTemplateContentWriter();
		IObjectContentWriter CreateSequenceContentWriter(string filePath);
	}
}
