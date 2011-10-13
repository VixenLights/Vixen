using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Module;

namespace Vixen.IO.Xml {
	class XmlModuleStoreReader : XmlReaderBase<ModuleStore> {
		private const string ELEMENT_ROOT = "ModuleStore";
		private const string ELEMENT_MODULE_DATA = "ModuleData";

		protected override ModuleStore _CreateObject(XElement element, string filePath) {
			ModuleStore obj = new ModuleStore() { LoadedFilePath = filePath };

			return obj;
		}

		protected override void _PopulateObject(ModuleStore obj, XElement element) {
			// Alternate data path handled by VixenSystem.
			ModuleStaticDataSet moduleData = _ReadModuleData(element);
			obj.Data = moduleData;
		}

		private ModuleStaticDataSet _ReadModuleData(XElement element) {
			XElement moduleDataElement = element.Element(ELEMENT_MODULE_DATA);
			ModuleStaticDataSet moduleData = new ModuleStaticDataSet();

			if(moduleDataElement != null) {
				moduleData.Deserialize(moduleDataElement.ToString());
			}

			return moduleData;
		}
	}
}
