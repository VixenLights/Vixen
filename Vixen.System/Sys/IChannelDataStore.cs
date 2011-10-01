using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Sys {
	interface IChannelDataStore : IEnumerable<CommandNode> {
		void Add(CommandNode command);
		void Clear();
	}
}
