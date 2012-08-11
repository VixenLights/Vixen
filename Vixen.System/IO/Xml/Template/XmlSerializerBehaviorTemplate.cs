using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Result;
using Vixen.Sys;

// The idea is this:
// FileSerializer XML subclass uses two pieces:
// 1. XmlSerializerBehaviorTemplate (this): Serialization behavior for the internal XML serializers.
// 2. A contract fulfillment class (because I really don't know what to call it): Provides object-specific
//    functionality that the behavior template needs to serialize that object type.
namespace Vixen.IO.Xml.Template {
	class XmlSerializerBehaviorTemplate<T>
		where T : class {
		private List<ISerializationResult> _results;

		public XmlSerializerBehaviorTemplate() {
			_results = new List<ISerializationResult>();
		}

		public T Read(ref string filePath, IXmlStandardFileReadTemplate<T> readTemplate) {
			filePath = readTemplate.GetAbsoluteFilePath(filePath);
			if(!File.Exists(filePath)) {
				_results.Add(new SerializationResult(false, "File does not exist.", null));
				return default(T);
			}

			XElement content = _LoadFile(filePath);

			T obj = readTemplate.CreateNewObjectFor(filePath);
			IFilePolicy filePolicy = readTemplate.GetFilePolicy(obj, content);
			filePolicy.Read();

			return obj;
		}

		public void Write(T obj, ref string filePath, IXmlStandardFileWriteTemplate<T> writeTemplate) {
			XmlVersionedContent content = writeTemplate.GetContentNode();
			IFilePolicy filePolicy = writeTemplate.GetFilePolicy(obj, content);
			content.Version = filePolicy.Version;
			filePolicy.Write();

			filePath = writeTemplate.GetAbsoluteFilePath(filePath);
			content.Save(filePath);
		}

		public int FileVersion { get; private set; }

		private XElement _LoadFile(string filePath) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);

			XmlVersionedContent versionedContent = new XmlVersionedContent(content);
			FileVersion = versionedContent.Version;

			return content;
		}
	}
}
