using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlProgramSerializer : FileSerializer<Program> {
		private XmlTemplatedSerializer<Program> _templatedSerializer;
		private XmlProgramSerializerTemplate _serializerTemplate;

		public XmlProgramSerializer() {
			_templatedSerializer = new XmlTemplatedSerializer<Program>();
			_serializerTemplate = new XmlProgramSerializerTemplate();
		}

		protected override Program _Read(string filePath) {
			Program program = _templatedSerializer.Read(ref filePath, _serializerTemplate);
			if(program != null) {
				program.FilePath = filePath;
			}
			return program;
			//if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(Program.Directory, filePath);
			//filePath = Path.ChangeExtension(filePath, Program.Extension);

			//Program program = new Program(Path.GetFileNameWithoutExtension(filePath));
			//XElement content = _LoadFile(filePath);
			//XmlProgramFilePolicy filePolicy = new XmlProgramFilePolicy(program, content);
			//filePolicy.Read();

			//return program;
		}

		protected override void _Write(Program value, string filePath) {
			_templatedSerializer.Write(value, ref filePath, _serializerTemplate);
			value.FilePath = filePath;
			//XmlVersionedContent content = new XmlVersionedContent("Program");
			//IFilePolicy filePolicy = new XmlProgramFilePolicy(value, content);
			//content.Version = filePolicy.GetVersion();
			//filePolicy.Write();

			//filePath = Path.Combine(Program.Directory, Path.GetFileName(filePath));
			//filePath = Path.ChangeExtension(filePath, Program.Extension);
			//content.Save(filePath);
		}

		//private XElement _LoadFile(string filePath) {
		//    XmlFileLoader fileLoader = new XmlFileLoader();
		//    XElement content = Helper.Load(filePath, fileLoader);
		//    content = _EnsureContentIsUpToDate(content, filePath);
		//    return content;
		//}

		//private XElement _EnsureContentIsUpToDate(XElement content, string originalFilePath) {
		//    IMigrator sequenceMigrator = new XmlProgramMigrator(content);
		//    IFilePolicy filePolicy = new XmlProgramFilePolicy();
		//    XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
		//    _AddResults(serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, sequenceMigrator));

		//    return content;
		//}
	}
}
