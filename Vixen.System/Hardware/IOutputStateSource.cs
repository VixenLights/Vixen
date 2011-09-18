using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Hardware {
	public interface IOutputStateSource {
		Command[] SourceState { get; }
	}
}
