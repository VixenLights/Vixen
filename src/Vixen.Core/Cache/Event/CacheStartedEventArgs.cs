using Vixen.Module.Timing;

namespace Vixen.Cache.Event
{
	public class CacheStartedEventArgs : EventArgs
	{

		public CacheStartedEventArgs(ITiming timingSource, TimeSpan length, TimeSpan start)
		{
			Start = start;
			Length = length;
			TimingSource = timingSource;
		}

		public TimeSpan Start { get; private set; }
		public TimeSpan Length { get; private set; }
		public ITiming TimingSource { get; private set; }
	}
}
