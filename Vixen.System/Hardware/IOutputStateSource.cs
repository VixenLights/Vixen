using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;

namespace Vixen.Hardware {
	public interface IOutputStateSource {
		Command SourceState { get; }
	}
}
