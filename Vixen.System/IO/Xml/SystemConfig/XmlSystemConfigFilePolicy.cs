using System;
using System.Xml.Linq;
using Vixen.IO.Policy;
using Vixen.IO.Xml.Serializer;

namespace Vixen.IO.Xml.SystemConfig
{
	using Vixen.Sys;

	internal class XmlSystemConfigFilePolicy : SystemConfigFilePolicy
	{
		private SystemConfig _systemConfig;
		private XElement _content;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private const string ELEMENT_IDENTITY = "Identity";
		private const string ELEMENT_EVAL_FILTERS = "AllowFilterEvaluation";
		private const string ATTR_IS_CONTEXT = "isContext";
		private const string ELEMENT_DEFAULT_UPDATE_INTERVAL = "DefaultUpdateInterval";

		public XmlSystemConfigFilePolicy(SystemConfig systemConfig, XElement content)
		{
			_systemConfig = systemConfig;
			_content = content;
		}

		protected override void WriteContextFlag()
		{
			// If not a context, don't include the flag.
			if (_systemConfig.IsContext) {
				_content.Add(new XAttribute(ATTR_IS_CONTEXT, true));
			}
		}

		protected override void WriteIdentity()
		{
			_content.Add(new XElement(ELEMENT_IDENTITY, _systemConfig.Identity));
		}

		protected override void WriteFilterEvaluationAllowance()
		{
			_content.Add(new XElement(ELEMENT_EVAL_FILTERS, _systemConfig.AllowFilterEvaluation));
		}

		protected override void WriteDefaultUpdateInterval()
		{
			_content.Add(new XElement(ELEMENT_DEFAULT_UPDATE_INTERVAL, _systemConfig.DefaultUpdateInterval));
		}

		protected override void WriteElements()
		{
			XmlElementCollectionSerializer serializer = new XmlElementCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.Elements);
			_content.Add(element);
		}

		protected override void WriteNodes()
		{
			XmlElementNodeCollectionSerializer serializer = new XmlElementNodeCollectionSerializer(_systemConfig.Elements);
			XElement element = serializer.WriteObject(_systemConfig.Nodes);
			_content.Add(element);
		}

		protected override void WriteControllers()
		{
			XmlControllerCollectionSerializer serializer = new XmlControllerCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.OutputControllers);
			_content.Add(element);
		}

		//protected override void WriteSmartControllers()
		//{
		//	XmlSmartControllerCollectionSerializer serializer = new XmlSmartControllerCollectionSerializer();
		//	XElement element = serializer.WriteObject(_systemConfig.SmartOutputControllers);
		//	_content.Add(element);
		//}

		protected override void WriteDisabledDevices()
		{
			XmlDisabledControllerCollectionSerializer serializer = new XmlDisabledControllerCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.DisabledDeviceIds);
			_content.Add(element);
		}

		protected override void WritePreviews()
		{
			XmlPreviewCollectionSerializer serializer = new XmlPreviewCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.Previews);
			_content.Add(element);
		}

		protected override void WriteFilters()
		{
			XmlOutputFilterCollectionSerializer serializer = new XmlOutputFilterCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.Filters);
			_content.Add(element);
		}

		protected override void WriteDataFlowPatching()
		{
			XmlDataFlowPatchCollectionSerializer serializer = new XmlDataFlowPatchCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.DataFlow);
			_content.Add(element);
		}

		protected override void ReadContextFlag()
		{
			// The presence of the flag is enough.  The value is immaterial.
			_systemConfig.IsContext = _content.Attribute(ATTR_IS_CONTEXT) != null;
		}

		protected override void ReadIdentity()
		{
			XElement identityElement = _content.Element(ELEMENT_IDENTITY);
			if (identityElement != null) {
				_systemConfig.Identity = Guid.Parse(identityElement.Value);
			}
			else {
				Logging.Warn("System config does not have an identity value.");
			}
		}

		protected override void ReadFilterEvaluationAllowance()
		{
			// If it can't be determined, default to true.
			_systemConfig.AllowFilterEvaluation = XmlHelper.GetElementValue(_content, ELEMENT_EVAL_FILTERS, true);
		}

		protected override void ReadDefaultUpdateInterval()
		{
			XElement identityElement = _content.Element(ELEMENT_DEFAULT_UPDATE_INTERVAL);
			// ctor has default if we don't find value here..
			if (identityElement != null)
			{
				_systemConfig.DefaultUpdateInterval = Int32.Parse(identityElement.Value);
			}
		}

		protected override void ReadElements()
		{
			XmlElementCollectionSerializer serializer = new XmlElementCollectionSerializer();
			_systemConfig.Elements = serializer.ReadObject(_content);
		}

		protected override void ReadNodes()
		{
			XmlElementNodeCollectionSerializer serializer = new XmlElementNodeCollectionSerializer(_systemConfig.Elements);
			_systemConfig.Nodes = serializer.ReadObject(_content);
		}

		protected override void ReadControllers()
		{
			XmlControllerCollectionSerializer serializer = new XmlControllerCollectionSerializer();
			_systemConfig.OutputControllers = serializer.ReadObject(_content);
		}

		//protected override void ReadSmartControllers()
		//{
		//	XmlSmartControllerCollectionSerializer serializer = new XmlSmartControllerCollectionSerializer();
		//	_systemConfig.SmartOutputControllers = serializer.ReadObject(_content);
		//}

		protected override void ReadDisabledDevices()
		{
			XmlDisabledControllerCollectionSerializer serializer = new XmlDisabledControllerCollectionSerializer();
			_systemConfig.DisabledDeviceIds = serializer.ReadObject(_content);
		}

		protected override void ReadPreviews()
		{
			XmlPreviewCollectionSerializer serializer = new XmlPreviewCollectionSerializer();
			_systemConfig.Previews = serializer.ReadObject(_content);
		}

		protected override void ReadFilters()
		{
			XmlOutputFilterCollectionSerializer serializer = new XmlOutputFilterCollectionSerializer();
			_systemConfig.Filters = serializer.ReadObject(_content);
		}

		protected override void ReadDataFlowPatching()
		{
			XmlDataFlowPatchCollectionSerializer serializer = new XmlDataFlowPatchCollectionSerializer();
			_systemConfig.DataFlow = serializer.ReadObject(_content);
		}
	}
}