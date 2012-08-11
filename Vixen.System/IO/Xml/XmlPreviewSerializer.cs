using System;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml {
	class XmlPreviewSerializer : IXmlSerializer<IOutputDevice> {
		private const string ELEMENT_PREVIEW = "Preview";
		private const string ATTR_NAME = "name";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IOutputDevice value) {
			OutputPreview preview = (OutputPreview)value;

			//XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			//XElement dataSetElement = null;
			//if(preview.ModuleDataSet != null) {
			//    dataSetElement = dataSetSerializer.WriteObject(preview.ModuleDataSet);
			//}

			XElement element = new XElement(ELEMENT_PREVIEW,
				new XAttribute(ATTR_NAME, preview.Name),
				new XAttribute(ATTR_HARDWARE_ID, preview.ModuleId),
				new XAttribute(ATTR_ID, preview.Id));
				//dataSetElement);

			return element;
		}

		public IOutputDevice ReadObject(XElement element) {
			string name = XmlHelper.GetAttribute(element, ATTR_NAME);
			if(name == null) return null;

			Guid? moduleId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_ID);
			if(moduleId == null) return null;

			Guid? id = XmlHelper.GetGuidAttribute(element, ATTR_ID);
			if(id == null) return null;

			OutputPreview preview = new OutputPreview(id.Value, name, moduleId.Value);

			_Populate(preview, element);

			return preview;
		}

		private void _Populate(OutputPreview preview, XElement element) {
			//XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			//preview.ModuleDataSet = dataSetSerializer.ReadObject(element);
		}
	}
}
