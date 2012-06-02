using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlChannelPatchingSerializer : IXmlSerializer<IEnumerable<ChannelOutputPatch>> {
		private const string ELEMENT_PATCHING = "Patching";
		private const string ELEMENT_PATCH = "Patch";
		private const string ATTR_CHANNEL_ID = "channelId";

		public XElement WriteObject(IEnumerable<ChannelOutputPatch> value) {
			IEnumerable<XElement> elements = value.Select(_WriteChannelOutputPatch);
			return new XElement(ELEMENT_PATCHING, elements);
		}

		public IEnumerable<ChannelOutputPatch> ReadObject(XElement element) {
			List<ChannelOutputPatch> patches = new List<ChannelOutputPatch>();

			XElement parentNode = element.Element(ELEMENT_PATCHING);
			if(parentNode != null) {
				patches.AddRange(parentNode.Elements().Select(_ReadChannelOutputPatch).NotNull());
			}

			return patches;
		}

		private XElement _WriteChannelOutputPatch(ChannelOutputPatch channelOutputPatch) {
			XmlControllerReferenceSerializer controllerReferenceSerializer = new XmlControllerReferenceSerializer();
			IEnumerable<XElement> elements = channelOutputPatch.Select(controllerReferenceSerializer.WriteObject);
			
			return new XElement(ELEMENT_PATCH,
				new XAttribute(ATTR_CHANNEL_ID, channelOutputPatch.ChannelId),
				elements);
		}

		private ChannelOutputPatch _ReadChannelOutputPatch(XElement element) {
			Guid? channelId = XmlHelper.GetGuidAttribute(element, ATTR_CHANNEL_ID);
			if(channelId == null) return null;

			XmlControllerReferenceSerializer controllerReferenceSerializer = new XmlControllerReferenceSerializer();
			IEnumerable<ControllerReference> controllerReferences = element.Elements().Select(controllerReferenceSerializer.ReadObject);
			
			return new ChannelOutputPatch(channelId.Value, controllerReferences);
		}
	}
}
