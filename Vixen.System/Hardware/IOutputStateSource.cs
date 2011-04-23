using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Hardware {
	public interface IOutputStateSource {
		CommandData SourceState { get; }
	}
}
