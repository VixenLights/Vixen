using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlModuleStoreWriter : XmlWriterBase<ModuleStore> {
		private const string ELEMENT_ROOT = "ModuleStore";
		private const string ELEMENT_MODULE_DATA = "ModuleData";

		protected override System.Xml.Linq.XElement _CreateContent(ModuleStore obj) {
			return new XElement(ELEMENT_ROOT,
				_WriteModuleData(obj));
		}

		private XElement _WriteModuleData(ModuleStore obj) {
			return new XElement(ELEMENT_MODULE_DATA, obj.Data.ToXElement());
		}
	}
}
