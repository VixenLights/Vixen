using System;
using System.Drawing;
using Vixen.Sys;

namespace Vixen.Module.PreFilter {
	public interface IPreFilter : ISetup {
		TimeSpan TimeSpan { get; set; }
		ChannelNode[] TargetNodes { get; set; }
		float Affect(float value, float percentIntoFilter);
		DateTime Affect(DateTime value, float percentIntoFilter);
		Color Affect(Color value, float percentIntoFilter);
		long Affect(long value, float percentIntoFilter);
		double Affect(double value, float percentIntoFilter);
		IFilterState CreateFilterState(TimeSpan filterRelativeTime);
	}
}
