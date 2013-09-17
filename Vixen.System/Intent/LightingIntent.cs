using System;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class LightingIntent : LinearIntent<LightingValue>
	{
		public LightingIntent(LightingValue startValue, LightingValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
			//Generate a RNG Cryptographically random ID code for reference... 
			GenericID = System.IO.Path.GetRandomFileName().Replace(".", "");
		}
		public string GenericID { get; set; }
	}
}