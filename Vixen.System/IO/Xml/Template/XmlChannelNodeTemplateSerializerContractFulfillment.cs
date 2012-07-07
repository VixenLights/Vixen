using System.IO;
using System.Xml.Linq;
using Vixen.IO.Policy;
//using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlChannelNodeTemplateSerializerContractFulfillment : IXmlStandardFileWriteTemplate<ChannelNodeTemplate>, IXmlStandardFileReadTemplate<ChannelNodeTemplate> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("ChannelNodeTemplate");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlChannelNodeTemplatePolicy();
		}

		public string GetAbsoluteFilePath(string filePath) {
			if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(ChannelNodeTemplate.Directory, filePath);
			filePath = Path.ChangeExtension(filePath, ChannelNodeTemplate.Extension);
			return filePath;
		}

		public ChannelNodeTemplate CreateNewObjectFor(string filePath) {
			return new ChannelNodeTemplate();
		}

		//public IMigrator GetMigrator(XElement content) {
		//    return new XmlChannelNodeTemplateMigrator(content);
		//}

		public IFilePolicy GetFilePolicy(ChannelNodeTemplate obj, XElement content) {
			return new XmlChannelNodeTemplatePolicy(obj, content);
		}
	}
}
