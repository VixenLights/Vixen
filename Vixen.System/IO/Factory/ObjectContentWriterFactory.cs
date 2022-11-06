﻿using Vixen.IO.Binary.SequenceCache;
using Vixen.IO.Xml.ElementNodeTemplate;
using Vixen.IO.Xml.ModuleStore;
using Vixen.IO.Xml.Program;
using Vixen.IO.Xml.Sequence;
using Vixen.IO.Xml.SystemConfig;
using Vixen.IO.Xml.SystemContext;

namespace Vixen.IO.Factory
{
	internal class ObjectContentWriterFactory : IObjectContentWriterFactory
	{
		private static ObjectContentWriterFactory _instance;

		private ObjectContentWriterFactory()
		{
		}

		public static ObjectContentWriterFactory Instance
		{
			get { return _instance ?? (_instance = new ObjectContentWriterFactory()); }
		}

		public IObjectContentWriter CreateSystemConfigContentWriter()
		{
			return new SystemConfigXElementWriter();
		}

		public IObjectContentWriter CreateModuleStoreContentWriter()
		{
			return new ModuleStoreXElementWriter();
		}

		public IObjectContentWriter CreateSystemContextContentWriter()
		{
			return new SystemContextXElementWriter();
		}

		public IObjectContentWriter CreateProgramContentWriter()
		{
			return new ProgramXElementWriter();
		}

		public IObjectContentWriter CreateElementNodeTemplateContentWriter()
		{
			return new ElementNodeTemplateXElementWriter();
		}

		public IObjectContentWriter CreateSequenceContentWriter(string filePath)
		{
			return new SequenceXElementWriter(filePath);
		}

		public IObjectContentWriter CreateSequenceCacheContentWriter(string filePath)
		{
			return new SequenceCacheBinaryWriter();
		}

		public IObjectContentWriter CreateFixtureSpecificationContentWriter<T>(string filePath)
		{
			return new FixtureSpecificationXElementWriter<T>(filePath);
		}		
	}
}