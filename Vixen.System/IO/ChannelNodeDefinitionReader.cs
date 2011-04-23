using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.IO {
	class ChannelNodeDefinitionReader : IReader {
		public object Read(string filePath) {
			XElement element = Helper.LoadXml(filePath);
			return ChannelNodeDefinition.ReadXml(element);
		}
	}
}
