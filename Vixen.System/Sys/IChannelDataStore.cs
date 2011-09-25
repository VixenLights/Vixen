using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	interface IChannelDataStore : IEnumerable<Command> {
		void Add(Command command);
		void Clear();
	}
}
