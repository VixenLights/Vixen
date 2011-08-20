using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Vixen.IO.Xml {
	abstract class XmlWriterBase<T> : IWriter 
		where T : class, IVersioned {
		private const string ATTR_VERSION = "version";

		void IWriter.Write(string filePath, object value) {
			Write(filePath, value as T);
		}

		virtual public void Write(string filePath, T value) {
			XElement doc = _CreateContent(value);
			doc.SetAttributeValue(ATTR_VERSION, value.Version);
			doc.Save(filePath);
		}

		abstract protected XElement _CreateContent(T obj);
	}
}
