using System;using System.Linq;
using System.Xml.Linq;
using Vixen.Module.Media;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlMediaCollectionSerializer : IXmlSerializer<MediaCollection> {
		private const string ELEMENT_MEDIA = "Media";
		private const string ELEMENT_MEDIA_SOURCE = "MediaSource";
		private const string ATTR_TYPE_ID = "typeId";
		private const string ATTR_INSTANCE_ID = "instanceId";

		public XElement WriteObject(MediaCollection value) {
			return new XElement(ELEMENT_MEDIA,
				value.Select(x => new XElement(ELEMENT_MEDIA_SOURCE,
					new XAttribute(ATTR_TYPE_ID, x.Descriptor.TypeId),
					new XAttribute(ATTR_INSTANCE_ID, x.InstanceId),
					x.MediaFilePath)));
		}

		public MediaCollection ReadObject(XElement element) {
			MediaCollection mediaModules = new MediaCollection();

			XElement mediaElement = element.Element(ELEMENT_MEDIA);
			if(mediaElement != null) {
				foreach(XElement mediaSourceElement in mediaElement.Elements(ELEMENT_MEDIA_SOURCE)) {
					Guid? typeId = XmlHelper.GetGuidAttribute(mediaSourceElement, ATTR_TYPE_ID);
					if(typeId == null) continue;

					Guid? instanceId = XmlHelper.GetGuidAttribute(mediaSourceElement, ATTR_INSTANCE_ID);
					if(instanceId == null) continue;

					string filePath = mediaSourceElement.Value;
					if(string.IsNullOrWhiteSpace(filePath)) continue;

					IMediaModuleInstance module = Modules.ModuleManagement.GetMedia(typeId.Value);
					if(module != null) {
						module.InstanceId = instanceId.Value;
						module.MediaFilePath = filePath;
						mediaModules.Add(module);
					}
				}
			}

			return mediaModules;
		}
	}
}
