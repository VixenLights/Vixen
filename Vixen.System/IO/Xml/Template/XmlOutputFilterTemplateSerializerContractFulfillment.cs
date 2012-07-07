using System.IO;
using System.Xml.Linq;
using Vixen.IO.Policy;
//using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlOutputFilterTemplateSerializerContractFulfillment : IXmlStandardFileWriteTemplate<OutputFilterTemplate>, IXmlStandardFileReadTemplate<OutputFilterTemplate> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("OutputFilterTemplate");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlOutputFilterTemplatePolicy();
		}

		public IFilePolicy GetFilePolicy(OutputFilterTemplate obj, XElement content) {
			return new XmlOutputFilterTemplatePolicy(obj, content);
		}

		public string GetAbsoluteFilePath(string filePath) {
			if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(OutputFilterTemplate.Directory, filePath);
			filePath = Path.ChangeExtension(filePath, OutputFilterTemplate.Extension);
			return filePath;
		}


		public OutputFilterTemplate CreateNewObjectFor(string filePath) {
			return new OutputFilterTemplate();
		}

		//public IMigrator GetMigrator(XElement content) {
		//    return new XmlOutputFilterTemplateMigrator(content);
		//}
	}
}
