using System;

namespace Vixen.Sys {
	public interface IDataNode : IComparable<IDataNode> {
		TimeSpan StartTime { get; }
		TimeSpan EndTime { get; }
		TimeSpan TimeSpan { get; }
		TimeNode TimeNode { get; }
	}
}
