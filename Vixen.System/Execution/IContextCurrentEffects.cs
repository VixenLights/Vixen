using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Execution {
	interface IContextCurrentEffects : IEnumerable<IEffectNode> {
		Guid[] UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime);
	}
}
