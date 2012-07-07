using System;
using System.Collections.Generic;
using Vixen.Execution.DataSource;
using Vixen.Sys;

namespace Vixen.Execution {
	interface IContextCurrentEffects : IEnumerable<IEffectNode> {
		Guid[] UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime);
	}
}
