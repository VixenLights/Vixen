using System.Collections.Generic;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Result;
using Vixen.Sys;

namespace Vixen.IO.Xml.Template {
	class XmlTemplatedSerializer<T>
		where T : class {
		private List<IFileOperationResult> _results;

		public XmlTemplatedSerializer() {
			_results = new List<IFileOperationResult>();
		}

		public T Read(ref string filePath, IXmlStandardFileReadTemplate<T> readTemplate) {
			filePath = readTemplate.GetAbsoluteFilePath(filePath);
			XElement content = _LoadFile(filePath, readTemplate);

			T obj = readTemplate.CreateNewObjectFor(filePath);
			IFilePolicy filePolicy = readTemplate.GetFilePolicy(obj, content);
			filePolicy.Read();

			return obj;
		}

		public void Write(T obj, ref string filePath, IXmlStandardFileWriteTemplate<T> writeTemplate) {
			XmlVersionedContent content = writeTemplate.GetContentNode();
			IFilePolicy filePolicy = writeTemplate.GetFilePolicy(obj, content);
			content.Version = filePolicy.GetVersion();
			filePolicy.Write();

			filePath = writeTemplate.GetAbsoluteFilePath(filePath);
			content.Save(filePath);
		}

		public IEnumerable<IFileOperationResult> GetResults() {
			return _results;
		}

		private XElement _LoadFile(string filePath, IXmlStandardFileReadTemplate<T> readTemplate) {
			XmlFileLoader fileLoader = new XmlFileLoader();
			XElement content = Helper.Load(filePath, fileLoader);
			IEnumerable<IFileOperationResult> results = _EnsureContentIsUpToDate(content, filePath, readTemplate);
			_AddFileOperationResults(results);

			return content;
		}

		private IEnumerable<IFileOperationResult> _EnsureContentIsUpToDate(XElement content, string originalFilePath, IXmlStandardFileReadTemplate<T> readTemplate) {
			IMigrator migrator = readTemplate.GetMigrator(content);
			IFilePolicy filePolicy = readTemplate.GetEmptyFilePolicy();
			XmlFileSerializationHelper serializationHelper = new XmlFileSerializationHelper();
			IEnumerable<IFileOperationResult> results = serializationHelper.EnsureContentIsUpToDate(content, originalFilePath, filePolicy, migrator);

			return results;
		}

		private void _AddFileOperationResults(IEnumerable<IFileOperationResult> results) {
			_results.AddRange(results);
		}
	}
}
