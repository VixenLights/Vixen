using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	interface ITimed {
		TimeSpan StartTime { get; }
		TimeSpan EndTime { get; }
		bool IsEmpty { get; }
	}
}
