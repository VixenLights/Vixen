using System.Xml.Linq;

namespace Vixen.IO.Xml {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of object being serialized.</typeparam>
	interface IXmlSerializer<T> : ISerializer<T, XElement>
		where T : class {
	}
}
