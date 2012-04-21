using System;
using Vixen.Sys;

namespace Vixen.Module.PreFilter {
	public interface IPreFilter : ISetup {
		TimeSpan TimeSpan { get; set; }
		ChannelNode[] TargetNodes { get; set; }
		//*** the pre-filter is expected to affect the intent segment in-place
		void AffectIntent(IIntentSegment intentSegment, TimeSpan filterRelativeStartTime, TimeSpan filterRelativeEndTime);
	}
}
