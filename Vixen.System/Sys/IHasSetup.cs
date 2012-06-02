using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public interface IHasSetup {
		bool HasSetup { get; }
		bool Setup();
	}
}
