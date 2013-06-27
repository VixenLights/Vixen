using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution
{
	internal interface ITimingProvider
	{
		string TimingProviderTypeName { get; }
		string[] GetAvailableTimingSources(ISequence sequence);
		ITiming GetTimingSource(ISequence sequence, string sourceName);
	}
}