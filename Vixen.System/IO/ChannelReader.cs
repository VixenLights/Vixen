using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Common;
using Vixen.Sys;
using System.IO;

namespace Vixen.IO {
	// Must be generic because it's creating instances.
    class ChannelReader<T>
		where T : Channel, new() {
        public bool Read(XmlReader reader) {
			if(reader.IsStartElement("Channel")) {
				//This cannot be converted to an OutputChannel
				//Channel<T> channel = new Channel<T>(new Guid(reader.GetAttribute("id")));
				T channel = new T();
				channel.Id = new Guid(reader.GetAttribute("id"));

				//...Any attributes go here...
				channel.AllowFrameSkip = bool.Parse(reader.GetAttribute("allowFrameSkip"));
				channel.Name = reader.GetAttribute("name");

				if(reader.ElementsExistWithin("Channel")) { // Entity element
					if(reader.ElementsExistWithin("Patch")) {
						// Get the controller references.
						while(reader.IsStartElement("ControllerReference")) {
							channel.Patch.Add(new Guid(reader.GetAttribute("controllerId")), int.Parse(reader.GetAttribute("outputIndex")));
							reader.Skip();
						}
						reader.ReadEndElement(); // Patch
					}

					reader.ReadEndElement(); // Channel
					this.Channel = channel;
				}
				return true;
			}
			return false;
        }

		public T Channel { get; private set; }
    }
}
