using System;
using System.Xml.Linq;
using Vixen.Factory;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml.Serializer {
	class XmlPreviewSerializer : IXmlSerializer<IOutputDevice> {
		private const string ELEMENT_PREVIEW = "Preview";
		private const string ATTR_NAME = "name";
		private const string ATTR_HARDWARE_ID = "hardwareId";
		private const string ATTR_HARDWARE_INSTANCE_ID = "hardwareInstanceId";
		private const string ATTR_ID = "id";

		public XElement WriteObject(IOutputDevice value) {
			OutputPreview preview = (OutputPreview)value;

			XElement element = new XElement(ELEMENT_PREVIEW,
				new XAttribute(ATTR_NAME, preview.Name),
				new XAttribute(ATTR_HARDWARE_ID, preview.ModuleId),
				new XAttribute(ATTR_ID, preview.Id));

			return element;
		}

		public IOutputDevice ReadObject(XElement element) {
			string name = XmlHelper.GetAttribute(element, ATTR_NAME);
			if(name == null) return null;

			Guid? moduleId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_ID);
			if(moduleId == null) return null;

			Guid? moduleInstanceId = XmlHelper.GetGuidAttribute(element, ATTR_HARDWARE_INSTANCE_ID);
			if(moduleInstanceId == null) return null;

			Guid? deviceId = XmlHelper.GetGuidAttribute(element, ATTR_ID);
			if(deviceId == null) return null;

			PreviewFactory previewFactory = new PreviewFactory();
			OutputPreview preview = (OutputPreview)previewFactory.CreateDevice(deviceId.Value, moduleId.Value, moduleInstanceId.Value, name);

			_Populate(preview, element);

			return preview;
		}

		private void _Populate(OutputPreview preview, XElement element) {
			//XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			//preview.ModuleDataSet = dataSetSerializer.ReadObject(element);
		}
	}
}
