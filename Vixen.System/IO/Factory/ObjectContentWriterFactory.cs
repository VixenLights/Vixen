using Vixen.IO.Xml.ChannelNodeTemplate;
using Vixen.IO.Xml.ModuleStore;
using Vixen.IO.Xml.Program;
using Vixen.IO.Xml.Sequence;
using Vixen.IO.Xml.SystemConfig;
using Vixen.IO.Xml.SystemContext;

namespace Vixen.IO.Factory {
	class ObjectContentWriterFactory : IObjectContentWriterFactory {
		static private ObjectContentWriterFactory _instance;

		private ObjectContentWriterFactory() {
		}

		public static ObjectContentWriterFactory Instance {
			get { return _instance ?? (_instance = new ObjectContentWriterFactory()); }
		}

		public IObjectContentWriter CreateSystemConfigContentWriter() {
			return new SystemConfigXElementWriter();
		}

		public IObjectContentWriter CreateModuleStoreContentWriter() {
			return new ModuleStoreXElementWriter();
		}

		public IObjectContentWriter CreateSystemContextContentWriter() {
			return new SystemContextXElementWriter();
		}

		public IObjectContentWriter CreateProgramContentWriter() {
			return new ProgramXElementWriter();
		}

		public IObjectContentWriter CreateChannelNodeTemplateContentWriter() {
			return new ChannelNodeTemplateXElementWriter();
		}

		public IObjectContentWriter CreateSequenceContentWriter(string filePath) {
			return new SequenceXElementWriter(filePath);
		}
	}
}
