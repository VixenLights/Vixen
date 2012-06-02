using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSequenceSerializer : FileSerializer<Sequence> {
		private XmlTemplatedSerializer<Sequence> _templatedSerializer;
		private XmlSequenceSerializerTemplate _serializerTemplate;

		public XmlSequenceSerializer() {
			_templatedSerializer = new XmlTemplatedSerializer<Sequence>();
			_serializerTemplate = new XmlSequenceSerializerTemplate();
		}

		protected override Sequence _Read(string filePath) {
			Sequence sequence = _templatedSerializer.Read(ref filePath, _serializerTemplate);
			if(sequence != null) {
				sequence.FilePath = filePath;
			}
			return sequence;
			//if(!Path.IsPathRooted(filePath)) {
			//    filePath = Path.Combine(Sequence.DefaultDirectory, filePath);
			//}

			//Sequence sequence = _CreateSequenceFor(filePath);
			//XElement content = _LoadFile(filePath);
			//XmlSequenceFilePolicy filePolicy = new XmlSequenceFilePolicy(sequence, content);
			//filePolicy.Read();

			//sequence.FilePath = filePath;

			//return sequence;
		}

		protected override void _Write(Sequence value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			value.FilePath = filePath;
			//XmlVersionedContent content = new XmlVersionedContent("Sequence");
			//IFilePolicy filePolicy = new XmlSequenceFilePolicy(value, content);
			//content.Version = filePolicy.GetVersion();
			//filePolicy.Write();

			//filePath = Path.Combine(Sequence.DefaultDirectory, Path.GetFileName(filePath));
			//content.Save(filePath);

			//value.FilePath = filePath;
		}

		//private XElement _LoadFile(string filePath) {
		//    XmlFileLoader fileLoader = new XmlFileLoader();
		//    XElement content = Helper.Load(filePath, fileLoader);
		//    content = _EnsureContentIsUpToDate(content, filePath);
		//    return content;
		//}

		//private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
		//    IMigrator sequenceMigrator = new XmlSequenceMigrator(content);
		//    IFilePolicy filePolicy = new XmlSequenceFilePolicy();
		//    XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
		//    _AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

		//    return content;
		//}

		//private Sequence _CreateSequenceFor(string filePath) {
		//    // Get the specific sequence module manager.
		//    SequenceModuleManagement manager = Modules.GetManager<ISequenceModuleInstance, SequenceModuleManagement>();

		//    // Get an instance of the appropriate sequence module.
		//    Sequence sequence = (Sequence)manager.Get(filePath);
		//    if(sequence == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

		//    return sequence;
		//}
	}
}
