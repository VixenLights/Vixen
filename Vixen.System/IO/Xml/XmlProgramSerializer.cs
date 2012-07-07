using Vixen.IO.Xml.Template;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlProgramSerializer : IVersionedFileSerializer {
		private XmlSerializerBehaviorTemplate<Program> _serializerBehaviorTemplate;
		private XmlProgramSerializerContractFulfillment _serializerContractFulfillment;

		public XmlProgramSerializer() {
			_serializerBehaviorTemplate = new XmlSerializerBehaviorTemplate<Program>();
			_serializerContractFulfillment = new XmlProgramSerializerContractFulfillment();
		}

		public int FileVersion {
			get { return _serializerBehaviorTemplate.FileVersion; }
			//set { _serializerBehaviorTemplate.FileVersion = value; }
		}

		public int ClassVersion {
			get { return _serializerContractFulfillment.GetEmptyFilePolicy().Version; }
		}

		public object Read(string filePath) {
			Program program = _serializerBehaviorTemplate.Read(ref filePath, _serializerContractFulfillment);
			if(program != null) {
				program.FilePath = filePath;
			}
			return program;
		}

		public void Write(object value, string filePath) {
			Program program = (Program)value;
			_serializerBehaviorTemplate.Write(program, ref filePath, _serializerContractFulfillment);
			program.FilePath = filePath;
		}
	}
}
