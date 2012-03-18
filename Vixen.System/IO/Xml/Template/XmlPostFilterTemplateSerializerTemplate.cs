using System.IO;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Migrator;
using Vixen.IO.Xml.Policy;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlPostFilterTemplateSerializerTemplate : IXmlStandardFileWriteTemplate<PostFilterTemplate>, IXmlStandardFileReadTemplate<PostFilterTemplate> {
		public XmlVersionedContent GetContentNode() {
			return new XmlVersionedContent("PostFilterTemplate");
		}

		public IFilePolicy GetEmptyFilePolicy() {
			return new XmlPostFilterTemplatePolicy();
		}

		public IFilePolicy GetFilePolicy(PostFilterTemplate obj, XElement content) {
			return new XmlPostFilterTemplatePolicy(obj, content);
		}

		public string GetAbsoluteFilePath(string filePath) {
			if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(PostFilterTemplate.Directory, filePath);
			filePath = Path.ChangeExtension(filePath, PostFilterTemplate.Extension);
			return filePath;
		}


		public PostFilterTemplate CreateNewObjectFor(string filePath) {
			return new PostFilterTemplate();
		}

		public IMigrator GetMigrator(XElement content) {
			return new XmlPostFilterTemplateMigrator(content);
		}
	}
}
