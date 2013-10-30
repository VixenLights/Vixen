using System;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class LightingIntent : LinearIntent<LightingValue>
	{
		private static object lockObject = new object();
		private static long genericIDValue = 0;
		public LightingIntent(LightingValue startValue, LightingValue endValue, TimeSpan timeSpan)
			: base(startValue, endValue, timeSpan)
		{
			//Generate a RNG Cryptographically random ID code for reference... 
			//GenericID = System.IO.Path.GetRandomFileName().Replace(".", "");
			lock (lockObject) {
				GenericID = genericIDValue++;
			}
		}
		public long GenericID { get; set; }
	}
}