using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO {
	class ChannelNodeDefinitionWriter : IWriter {
		public void Write(string filePath, object value) {
			ChannelNodeDefinition definition = (ChannelNodeDefinition)value;
			XElement doc = ChannelNodeDefinition.WriteXml(definition);
			doc.Save(filePath);
		}
	}
}
