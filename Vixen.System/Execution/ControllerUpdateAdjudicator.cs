using System.Diagnostics;
using Vixen.Sys;

namespace Vixen.Execution
{
	internal class ControllerUpdateAdjudicator
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private int _threshold;
		private Stopwatch _stopwatch;
		private bool _firstTime;
		

		public ControllerUpdateAdjudicator(int thresholdInMilliseconds)
		{
			_threshold = thresholdInMilliseconds;
			_firstTime = true;
			_stopwatch = Stopwatch.StartNew();
		}

		/// <summary>
		/// Determines if an update should be allowed.
		/// </summary>
		/// <returns>If an update should be done.</returns>
		public bool PetitionForUpdate()
		{
			bool result = false;

			// no sense in allowing faster updates than the fastest controller...
			//int _minUpdateInterval = 200;   // slowly in case of no controllers..
			//foreach (var oc in Vixen.Sys.VixenSystem.OutputControllers)
			//	if (oc.UpdateInterval < _minUpdateInterval)
			//		_minUpdateInterval = oc.UpdateInterval;

			

			// 3 to allow if 3ms early (this is not an exact science)
			long ms = _stopwatch.ElapsedMilliseconds;
			if (_firstTime || ms+3 >= System.Math.Max(_threshold,VixenSystem.DefaultUpdateInterval)) {
				_firstTime = false;
				_stopwatch.Restart();
				result = true;
				if (ms > VixenSystem.DefaultUpdateInterval + 10)
					Logging.Debug("late petition: {0} ms", ms - VixenSystem.DefaultUpdateInterval);
			}
			//Logging.Debug("petition: {0} ms, {1}", ms, result ? "allowed" : "denied");
			return result;
		}
	}
}