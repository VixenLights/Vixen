using System;
using Vixen.Commands;

namespace Vixen.Module.Intent {
	public interface IIntent {
		TimeSpan TimeSpan { get; set; }
		Command GetCurrentState(TimeSpan timeOffset);
		object[] Values { get; set; }
	}
}
