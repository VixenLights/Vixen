using System;
using System.Xml.Linq;
using Vixen.Factory;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.IO.Xml.Serializer
{
	internal class XmlPreviewSerializer : IXmlSerializer<IOutputDevice>
	{
		private static NLog.Logger logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_PREVIEW = "Preview";
		private const string ATTR_NAME = "name";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";

		public XElement WriteObject(IOutputDevice value)
		{
			OutputPreview preview = (OutputPreview) value;

			XElement element = new XElement(ELEMENT_PREVIEW,
			                                new XAttribute(ATTR_NAME, preview.Name),
			                                new XAttribute(ATTR_TYPE_ID, preview.ModuleId),
			                                new XAttribute(ATTR_INSTANCE_ID, preview.ModuleInstanceId));

			return element;
		}

		public IOutputDevice ReadObject(XElement element)
		{
			try {
				string name = XmlHelper.GetAttribute(element, ATTR_NAME);
				if (name == null)
					return null;

				Guid? typeId = XmlHelper.GetGuidAttribute(element, ATTR_TYPE_ID);
				if (typeId == null)
					return null;

				Guid? instanceId = XmlHelper.GetGuidAttribute(element, ATTR_INSTANCE_ID);
				if (instanceId == null)
					return null;

				PreviewFactory previewFactory = new PreviewFactory();
				OutputPreview preview = (OutputPreview) previewFactory.CreateDevice(typeId.Value, instanceId.Value, name);

				_Populate(preview, element);

				return preview;
			} catch (Exception e) {
				logging.Error(e, "Error loading Preview from XML");
				return null;
			}
		}

		private void _Populate(OutputPreview preview, XElement element)
		{
			//XmlModuleLocalDataSetSerializer dataSetSerializer = new XmlModuleLocalDataSetSerializer();
			//preview.ModuleDataSet = dataSetSerializer.ReadObject(element);
		}
	}
}