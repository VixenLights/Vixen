using System;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSystemConfigFilePolicy : SystemConfigFilePolicy {
		private SystemConfig _systemConfig;
		private XElement _content;

		private const string ELEMENT_DATA_DIRECTORY = "DataDirectory";
		private const string ELEMENT_IDENTITY = "Identity";
		private const string ATTR_IS_CONTEXT = "isContext";

		public XmlSystemConfigFilePolicy() {
		}

		//private const string ELEMENT_ROOT = "SystemConfig";
		public XmlSystemConfigFilePolicy(SystemConfig systemConfig, XElement content) {
			_systemConfig = systemConfig;
			_content = content;
		}

		protected override void WriteContextFlag() {
			// If not a context, don't include the flag.
			if(_systemConfig.IsContext) {
				_content.Add(new XAttribute(ATTR_IS_CONTEXT, true));
			}
		}

		protected override void WriteIdentity() {
			_content.Add(new XElement(ELEMENT_IDENTITY, _systemConfig.Identity));
		}

		protected override void WriteAlternateDataDirectory() {
			if(!string.IsNullOrWhiteSpace(_systemConfig.AlternateDataPath)) {
				_content.Add(new XElement(ELEMENT_DATA_DIRECTORY, _systemConfig.AlternateDataPath));
			}
		}

		protected override void WriteChannels() {
			XmlChannelCollectionSerializer serializer = new XmlChannelCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.Channels);
			_content.Add(element);
		}

		protected override void WriteNodes() {
			XmlChannelNodeCollectionSerializer serializer = new XmlChannelNodeCollectionSerializer(_systemConfig.Channels);
			XElement element = serializer.WriteObject(_systemConfig.Nodes);
			_content.Add(element);
		}

		protected override void WriteControllers() {
			XmlControllerCollectionSerializer serializer = new XmlControllerCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.Controllers);
			_content.Add(element);
		}

		protected override void WriteControllerLinks() {
			XmlControllerLinkCollectionSerializer serializer = new XmlControllerLinkCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.ControllerLinking);
			_content.Add(element);
		}

		protected override void WriteChannelPatching() {
			XmlChannelPatchingSerializer serializer = new XmlChannelPatchingSerializer();
			XElement element = serializer.WriteObject(_systemConfig.ChannelPatching);
			_content.Add(element);
		}

		protected override void WriteDisabledControllers() {
			XmlDisabledControllerCollectionSerializer serializer = new XmlDisabledControllerCollectionSerializer();
			XElement element = serializer.WriteObject(_systemConfig.DisabledControllers.Select(x => x.Id));
			_content.Add(element);
		}

		protected override void ReadContextFlag() {
			// The presence of the flag is enough.  The value is immaterial.
			_systemConfig.IsContext = _content.Attribute(ATTR_IS_CONTEXT) != null;
		}

		protected override void ReadIdentity() {
			XElement identityElement = _content.Element(ELEMENT_IDENTITY);
			if(identityElement != null) {
				_systemConfig.Identity = Guid.Parse(identityElement.Value);
			} else {
				VixenSystem.Logging.Warning("System config does not have an identity value.");
			}
		}

		//protected override void ReadAlternateDataDirectory() {
		//    throw new NotImplementedException();
		//}

		protected override void ReadChannels() {
			XmlChannelCollectionSerializer serializer = new XmlChannelCollectionSerializer();
			_systemConfig.Channels = serializer.ReadObject(_content);
		}

		protected override void ReadNodes() {
			XmlChannelNodeCollectionSerializer serializer = new XmlChannelNodeCollectionSerializer(_systemConfig.Channels);
			_systemConfig.Nodes = serializer.ReadObject(_content);
		}

		protected override void ReadControllers() {
			XmlControllerCollectionSerializer serializer = new XmlControllerCollectionSerializer();
			_systemConfig.Controllers = serializer.ReadObject(_content);
		}

		protected override void ReadControllerLinks() {
			XmlControllerLinkCollectionSerializer serializer = new XmlControllerLinkCollectionSerializer();
			_systemConfig.ControllerLinking = serializer.ReadObject(_content);
		}

		protected override void ReadChannelPatching() {
			XmlChannelPatchingSerializer serializer = new XmlChannelPatchingSerializer();
			_systemConfig.ChannelPatching = serializer.ReadObject(_content);
		}

		protected override void ReadDisabledControllers() {
			XmlDisabledControllerCollectionSerializer serializer = new XmlDisabledControllerCollectionSerializer();
			_systemConfig.DisabledControllers = serializer.ReadObject(_content).Select(VixenSystem.Controllers.Get);
		}
	}
}
