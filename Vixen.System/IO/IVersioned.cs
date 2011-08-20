using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO {
	interface IVersioned {
		int Version { get; }
	}
}
