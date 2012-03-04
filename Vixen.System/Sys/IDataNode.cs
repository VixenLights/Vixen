using System;

namespace Vixen.Sys {
	public interface IDataNode {
		TimeSpan StartTime { get; }
		TimeSpan EndTime { get; }

		TimeSpan TimeSpan { get; }
	}
}
