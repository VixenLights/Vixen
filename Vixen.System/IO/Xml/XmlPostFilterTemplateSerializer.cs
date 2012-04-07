using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlPostFilterTemplateSerializer : FileSerializer<PostFilterTemplate> {
		private XmlTemplatedSerializer<PostFilterTemplate> _templatedSerializer;
		private XmlPostFilterTemplateSerializerTemplate _serializerTemplate;

		public XmlPostFilterTemplateSerializer() {
			_templatedSerializer = new XmlTemplatedSerializer<PostFilterTemplate>();
			_serializerTemplate = new XmlPostFilterTemplateSerializerTemplate();
		}

		protected override PostFilterTemplate _Read(string filePath) {
			PostFilterTemplate template = _templatedSerializer.Read(ref filePath, _serializerTemplate);
			if(template != null) {
				template.FilePath = filePath;
			}
			return template;
			//if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(PostFilterTemplate.Directory, filePath);
			//filePath = Path.ChangeExtension(filePath, PostFilterTemplate.Extension);

			//PostFilterTemplate template = new PostFilterTemplate();
			//XElement content = _LoadFile(filePath);
			//XmlPostFilterTemplatePolicy filePolicy = new XmlPostFilterTemplatePolicy(template, content);
			//filePolicy.Read();

			//return template;
		}

		protected override void _Write(PostFilterTemplate value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			value.FilePath = filePath;
			//XmlVersionedContent content = new XmlVersionedContent("PostFilterTemplate");
			//IFilePolicy filePolicy = new XmlPostFilterTemplatePolicy(value, content);
			//content.Version = filePolicy.GetVersion();
			//filePolicy.Write();

			//filePath = Path.Combine(PostFilterTemplate.Directory, Path.GetFileName(filePath));
			//filePath = Path.ChangeExtension(filePath, PostFilterTemplate.Extension);
			//content.Save(filePath);
		}

		//private XElement _LoadFile(string filePath) {
		//    XmlFileLoader fileLoader = new XmlFileLoader();
		//    XElement content = Helper.Load(filePath, fileLoader);
		//    content = _EnsureContentIsUpToDate(content, filePath);
		//    return content;
		//}

		//private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
		//    IMigrator sequenceMigrator = new XmlPostFilterTemplateMigrator(content);
		//    IFilePolicy filePolicy = new XmlPostFilterTemplatePolicy();
		//    XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
		//    _AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

		//    return content;
		//}
	}
}
