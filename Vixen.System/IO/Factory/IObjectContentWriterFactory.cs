namespace Vixen.IO.Factory {
	interface IObjectContentWriterFactory {
		IObjectContentWriter CreateSystemConfigContentWriter();
		IObjectContentWriter CreateModuleStoreContentWriter();
		IObjectContentWriter CreateSystemContextContentWriter();
		IObjectContentWriter CreateProgramContentWriter();
		IObjectContentWriter CreateChannelNodeTemplateContentWriter();
		IObjectContentWriter CreateSequenceContentWriter(string filePath);
	}
}
