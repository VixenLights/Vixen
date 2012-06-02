using System.Xml.Linq;
using Vixen.IO.Policy;

namespace Vixen.IO.Xml.Template {
	interface IXmlStandardFileWriteTemplate<in T> {
		XmlVersionedContent GetContentNode();
		IFilePolicy GetEmptyFilePolicy();
		IFilePolicy GetFilePolicy(T obj, XElement content);
		string GetAbsoluteFilePath(string filePath);
	}
}
