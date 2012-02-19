using System.Xml.Linq;

namespace Vixen.IO.Xml {
	interface ISerializer<T>
		where T : class {
		XElement WriteObject(T value);
		T ReadObject(XElement element);
	}
}
