using Vixen.IO.Binary.SequenceCache;
using Vixen.IO.Xml.ElementNodeTemplate;
using Vixen.IO.Xml.ModuleStore;
using Vixen.IO.Xml.Program;
using Vixen.IO.Xml.Sequence;
using Vixen.IO.Xml.SystemConfig;
using Vixen.IO.Xml.SystemContext;

namespace Vixen.IO.Factory
{
	internal class ObjectContentReaderFactory : IObjectContentReaderFactory
	{
		private static ObjectContentReaderFactory _instance;

		private ObjectContentReaderFactory()
		{
		}

		public static ObjectContentReaderFactory Instance
		{
			get { return _instance ?? (_instance = new ObjectContentReaderFactory()); }
		}

		public IObjectContentReader CreateSystemConfigContentReader()
		{
			return new SystemConfigXElementReader();
		}

		public IObjectContentReader CreateModuleStoreContentReader()
		{
			return new ModuleStoreXElementReader();
		}

		public IObjectContentReader CreateSystemContextContentReader()
		{
			return new SystemContextXElementReader();
		}

		public IObjectContentReader CreateProgramContentReader()
		{
			return new ProgramXElementReader();
		}

		public IObjectContentReader CreateElementNodeTemplateContentReader()
		{
			return new ElementNodeTemplateXElementReader();
		}

		public IObjectContentReader CreateSequenceContentReader(string fileType)
		{
			return new SequenceXElementReader(fileType);
		}

		public IObjectContentReader CreateSequenceCacheContentReader(string fileType)
		{
			return new SequenceCacheBinaryReader();
		}
	}
}