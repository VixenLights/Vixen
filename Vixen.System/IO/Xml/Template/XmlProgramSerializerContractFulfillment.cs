using System.IO;
using System.Xml.Linq;
using Vixen.IO.Policy;
//using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlProgramSerializerContractFulfillment : IXmlStandardFileWriteTemplate<Program>, IXmlStandardFileReadTemplate<Program> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("Program");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlProgramFilePolicy();
		}

		public IFilePolicy GetFilePolicy(Program obj, XElement content) {
			return new XmlProgramFilePolicy(obj, content);
		}

		public string GetAbsoluteFilePath(string filePath) {
			if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(Program.ProgramDirectory, filePath);
			filePath = Path.ChangeExtension(filePath, Program.Extension);
			return filePath;
		}


		public Program CreateNewObjectFor(string filePath) {
			return new Program();
		}

		//public IMigrator GetMigrator(XElement content) {
		//    return new XmlProgramMigrator(content);
		//}
	}
}
