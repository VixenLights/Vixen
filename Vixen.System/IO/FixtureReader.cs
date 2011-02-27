using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.IO {
    class FixtureReader {
        public bool Read(XmlReader reader) {
            if(reader.IsStartElement() && reader.Name == "Fixture") {
                Fixture fixture = new Fixture();

                //...Any attributes go here...
                fixture.AllowFrameSkip = bool.Parse(reader.GetAttribute("allowFrameSkip"));
				fixture.Name = reader.GetAttribute("name");
				// This needs to hold off until after channels are loaded.
				string fixtureDefinitionName = reader.GetAttribute("fixtureDefinitionName");

                if(reader.ElementsExistWithin("Fixture")) { // Entity element
					// Channels
                    if(reader.ElementsExistWithin("Channels")) { // Container element for child entity
						ChannelReader<OutputChannel> channelReader = new ChannelReader<OutputChannel>();
                        while(channelReader.Read(reader)) {
                            fixture.InsertChannel(channelReader.Channel);
                        }
                        reader.ReadEndElement(); // Channels
                    }

                    // With channels loaded, the fixture template reference can be set.
					fixture.FixtureDefinitionName = fixtureDefinitionName;

                    reader.ReadEndElement(); // Fixture

                    this.Fixture = fixture;
                }
                return true;
            }
            return false;
        }

        public Fixture Fixture { get; private set; }
    }
}
