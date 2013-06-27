using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Execution.DataSource
{
	public interface IDataSource
	{
		IEnumerable<IEffectNode> GetDataAt(TimeSpan time);
	}
}