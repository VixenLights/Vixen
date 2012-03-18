using System.Xml.Linq;
using Vixen.IO.Policy;

namespace Vixen.IO.Xml.Template {
	interface IXmlStandardFileReadTemplate<T> {
		string GetAbsoluteFilePath(string filePath);
		T CreateNewObjectFor(string filePath);
		IMigrator GetMigrator(XElement content);
		IFilePolicy GetEmptyFilePolicy();
		IFilePolicy GetFilePolicy(T obj, XElement content);
	}
}
