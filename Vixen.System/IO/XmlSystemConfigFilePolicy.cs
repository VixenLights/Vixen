using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;

namespace Vixen.IO {
	class XmlSystemConfigFilePolicy : IFilePolicy {
		private SystemConfig _systemConfig;
		private XElement _content;

		public XmlSystemConfigFilePolicy() {
		}

		public XmlSystemConfigFilePolicy(SystemConfig systemConfig, XElement content) {
			_systemConfig = systemConfig;
			_content = content;
		}

		public void Write() {
			throw new NotImplementedException();
		}

		public void Read() {
			throw new NotImplementedException();
		}

		public int GetVersion() {
			throw new NotImplementedException();
		}
	}
}
