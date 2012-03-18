using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlModuleStoreSerializer : FileSerializer<ModuleStore> {
		private XmlTemplatedSerializer<ModuleStore> _templatedSerializer;
		private XmlModuleStoreSerializerTemplate _serializerTemplate;

		public XmlModuleStoreSerializer() {
			_templatedSerializer = new XmlTemplatedSerializer<ModuleStore>();
			_serializerTemplate = new XmlModuleStoreSerializerTemplate();
		}

		protected override ModuleStore _Read(string filePath) {
			return _templatedSerializer.Read(ref filePath, _serializerTemplate);

			//ModuleStore moduleStore = new ModuleStore();
			//XElement content = _LoadFile(filePath);
			//XmlModuleStoreFilePolicy filePolicy = new XmlModuleStoreFilePolicy(moduleStore, content);
			//filePolicy.Read();

			//moduleStore.LoadedFilePath = filePath;

			//return moduleStore;
		}

		protected override void _Write(ModuleStore value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			//XmlVersionedContent content = new XmlVersionedContent("ModuleStore");
			//IFilePolicy filePolicy = new XmlModuleStoreFilePolicy(value, content);
			//content.Version = filePolicy.GetVersion();
			//filePolicy.Write();
			//content.Save(filePath);

			//value.LoadedFilePath = filePath;
		}

		//private XElement _LoadFile(string filePath) {
		//    XmlFileLoader fileLoader = new XmlFileLoader();
		//    XElement content = Helper.Load(filePath, fileLoader);
		//    content = _EnsureContentIsUpToDate(content, filePath);
		//    return content;
		//}

		//private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
		//    IMigrator sequenceMigrator = new XmlModuleStoreMigrator(content);
		//    IFilePolicy filePolicy = new XmlModuleStoreFilePolicy();
		//    XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
		//    _AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

		//    return content;
		//}
	}
}
