//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Xml.Linq;
//using Vixen.IO.Policy;
using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelNodeTemplateSerializer : FileSerializer<ChannelNodeTemplate> {
		private XmlTemplatedSerializer<ChannelNodeTemplate> _templatedSerializer;
		private XmlChannelNodeTemplateSerializerTemplate _serializerTemplate;

		public XmlChannelNodeTemplateSerializer() {
			_templatedSerializer = new XmlTemplatedSerializer<ChannelNodeTemplate>();
			_serializerTemplate = new XmlChannelNodeTemplateSerializerTemplate();
		}

		protected override ChannelNodeTemplate _Read(string filePath) {
			return _templatedSerializer.Read(ref filePath, _serializerTemplate);

			//if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(ChannelNodeTemplate.Directory, filePath);
			//filePath = Path.ChangeExtension(filePath, ChannelNodeTemplate.Extension);

			//ChannelNodeTemplate template = new ChannelNodeTemplate();
			//XElement content = _LoadFile(filePath);
			//XmlChannelNodeTemplatePolicy filePolicy = new XmlChannelNodeTemplatePolicy(template, content);
			//filePolicy.Read();

			//return template;
		}

		protected override void _Write(ChannelNodeTemplate value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			//XmlVersionedContent content = new XmlVersionedContent("ChannelNodeTemplate");
			//IFilePolicy filePolicy = new XmlChannelNodeTemplatePolicy(value, content);
			//content.Version = filePolicy.GetVersion();
			//filePolicy.Write();

			//filePath = Path.Combine(ChannelNodeTemplate.Directory, Path.GetFileName(filePath));
			//filePath = Path.ChangeExtension(filePath, ChannelNodeTemplate.Extension);
			//content.Save(filePath);
		}

		//private XElement _LoadFile(string filePath) {
		//    XmlFileLoader fileLoader = new XmlFileLoader();
		//    XElement content = Helper.Load(filePath, fileLoader);
		//    content = _EnsureContentIsUpToDate(content, filePath);
		//    return content;
		//}

		//private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
		//    IMigrator sequenceMigrator = new XmlChannelNodeTemplateMigrator(content);
		//    IFilePolicy filePolicy = new XmlChannelNodeTemplatePolicy();
		//    XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
		//    _AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

		//    return content;
		//}
	}
}
