using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.Execution {
	class EnumeratedCommandNodeList {
		public EnumeratedCommandNodeList() {
			CommandNodes = new List<CommandNode>();
			Enumerator = new LiveListEnumerator<CommandNode>(CommandNodes);
		}

		public List<CommandNode> CommandNodes { get; private set; }
		public IEnumerator<CommandNode> Enumerator { get; private set; }
	}
}
