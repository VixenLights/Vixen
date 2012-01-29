using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public interface IDataNode {
		TimeSpan StartTime { get; }
		TimeSpan EndTime { get; }
	}
}
