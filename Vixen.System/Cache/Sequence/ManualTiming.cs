using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Cache.Sequence
{
	internal class ManualTiming: ITiming
	{
		public ManualTiming()
		{
			Speed = VixenSystem.DefaultUpdateInterval;
		}

		public void Start()
		{
			Position = TimeSpan.Zero;
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
			Position += TimeSpan.FromMilliseconds(Speed);
		}

		public TimeSpan Position { get; set; }
		public bool SupportsVariableSpeeds { get{return false;} }
		public float Speed { get; set; }
	}
}
