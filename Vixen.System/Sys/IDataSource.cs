using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IDataSource {
		IEnumerable<EffectNode> GetDataAt(TimeSpan time);
	}
}
