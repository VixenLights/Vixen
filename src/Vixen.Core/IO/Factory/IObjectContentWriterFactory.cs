namespace Vixen.IO.Factory
{
	internal interface IObjectContentWriterFactory
	{
		IObjectContentWriter CreateSystemConfigContentWriter();
		IObjectContentWriter CreateModuleStoreContentWriter();
		IObjectContentWriter CreateSystemContextContentWriter();
		IObjectContentWriter CreateProgramContentWriter();
		IObjectContentWriter CreateElementNodeTemplateContentWriter();
		IObjectContentWriter CreateSequenceContentWriter(string filePath);
		IObjectContentWriter CreateSequenceCacheContentWriter(string filePath);
	}
}