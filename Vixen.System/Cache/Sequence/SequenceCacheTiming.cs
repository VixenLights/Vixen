using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Cache.Sequence
{
	internal class SequenceCacheTiming: ITiming
	{
		private TimeSpan _currentPosition = TimeSpan.Zero;

		public SequenceCacheTiming()
		{
			Speed = VixenSystem.DefaultUpdateInterval;
		}

		public void Start()
		{
			_currentPosition = TimeSpan.Zero;
		}

		public void Stop()
		{
			
		}

		public void Pause()
		{
			
		}

		public void Resume()
		{
			
		}

		public void Increment()
		{
			_currentPosition += TimeSpan.FromMilliseconds(Speed);
		}

		public TimeSpan Position { get; set; }
		public bool SupportsVariableSpeeds { get{return false;} }
		public float Speed { get; set; }
	}
}
