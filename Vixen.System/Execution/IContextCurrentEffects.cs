using System;
using System.Collections.Generic;
using Vixen.Execution.DataSource;
using Vixen.Sys;

namespace Vixen.Execution
{
	internal interface IContextCurrentEffects : IEnumerable<IEffectNode>
	{
		bool UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime);
		void RemoveEffects(IEnumerable<IEffectNode> nodes);
		void Reset(bool now);
		bool Resetting();
	}
}