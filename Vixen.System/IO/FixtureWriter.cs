using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Sys;

namespace Vixen.IO {
    class FixtureWriter {
        public void Write(XmlWriter writer) {
            if(Fixture != null) {
                writer.WriteStartElement("Fixture");

                // Attributes go here...
				writer.WriteAttributeString("allowFrameSkip", Fixture.AllowFrameSkip.ToString());
				writer.WriteAttributeString("name", Fixture.Name);
				writer.WriteAttributeString("fixtureDefinitionName", Fixture.FixtureDefinitionName);

                // Channels
                ChannelWriter channelWriter = new ChannelWriter();
                writer.WriteStartElement("Channels");
                foreach(Channel channel in Fixture.Channels) {
                    channelWriter.Channel = channel;
                    channelWriter.Write(writer);
                }
                writer.WriteEndElement(); // Channels

                writer.WriteEndElement(); // Fixture
            }
        }

        public Fixture Fixture { get; set; }
    }
}
