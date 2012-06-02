using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemContextSerializer : FileSerializer<SystemContext> {
		private XmlTemplatedSerializer<SystemContext> _templatedSerializer;
		private XmlSystemContextSerializerTemplate _serializerTemplate;

		public XmlSystemContextSerializer() {
			_templatedSerializer = new XmlTemplatedSerializer<SystemContext>();
			_serializerTemplate = new XmlSystemContextSerializerTemplate();
		}

		protected override SystemContext _Read(string filePath) {
			return _templatedSerializer.Read(ref filePath, _serializerTemplate);
			//SystemContext systemContext = new SystemContext();
			//XElement content = _LoadFile(filePath);
			//XmlSystemContextFilePolicy filePolicy = new XmlSystemContextFilePolicy(systemContext, content);
			//filePolicy.Read();

			//return systemContext;
		}

		protected override void _Write(SystemContext value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			//XmlVersionedContent content = new XmlVersionedContent("SystemContext");
			//IFilePolicy filePolicy = new XmlSystemContextFilePolicy(value, content);
			//content.Version = filePolicy.GetVersion();
			//filePolicy.Write();
			//content.Save(filePath);
		}

		//private XElement _LoadFile(string filePath) {
		//    XmlFileLoader fileLoader = new XmlFileLoader();
		//    XElement content = Helper.Load(filePath, fileLoader);
		//    content = _EnsureContentIsUpToDate(content, filePath);
		//    return content;
		//}

		//private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
		//    IMigrator sequenceMigrator = new XmlSystemContextMigrator(content);
		//    IFilePolicy filePolicy = new XmlSystemContextFilePolicy();
		//    XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
		//    _AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

		//    return content;
		//}
	}
}
