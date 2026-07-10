namespace Vixen.IO.Factory
{
	internal interface IObjectContentReaderFactory
	{
		IObjectContentReader CreateSystemConfigContentReader();
		IObjectContentReader CreateModuleStoreContentReader();
		IObjectContentReader CreateSystemContextContentReader();
		IObjectContentReader CreateProgramContentReader();
		IObjectContentReader CreateElementNodeTemplateContentReader();
		IObjectContentReader CreateSequenceContentReader(string fileType);
	}
}