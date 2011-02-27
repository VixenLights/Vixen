using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Sys;
using Vixen.Common;
using System.IO;

namespace Vixen.IO {
    class ChannelWriter {
        public void Write(XmlWriter writer) {
            if(Channel != null) {
                writer.WriteStartElement("Channel");

				writer.WriteAttributeString("id", Channel.Id.ToString());
				writer.WriteAttributeString("allowFrameSkip", Channel.AllowFrameSkip.ToString());
				writer.WriteAttributeString("name", Channel.Name);

                writer.WriteStartElement("Patch");

                foreach(ControllerReference controllerReference in Channel.Patch.ControllerReferences) {
                    writer.WriteStartElement("ControllerReference");
                    writer.WriteAttributeString("controllerId", controllerReference.ControllerId.ToString());
                    writer.WriteAttributeString("outputIndex", controllerReference.OutputIndex.ToString());
                    writer.WriteEndElement(); // ControllerReference
                }

                writer.WriteEndElement(); // Patch

                writer.WriteEndElement();
            }
        }

        public Channel Channel { get; set; }
    }
}
