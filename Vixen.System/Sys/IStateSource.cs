using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public interface IStateSource<out V> {
		V Value { get; }
	}
}
