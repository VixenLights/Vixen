using System.Diagnostics;

namespace Vixen.Execution
{
	internal class ControllerUpdateAdjudicator
	{
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
			int minUpdateInterval = 200;   // slowly in case of no controllers..
			foreach (var oc in Vixen.Sys.VixenSystem.OutputControllers)
				if (oc.UpdateInterval < minUpdateInterval)
					minUpdateInterval = oc.UpdateInterval;

			if (_firstTime || _stopwatch.ElapsedMilliseconds > System.Math.Max(_threshold,minUpdateInterval)) {
				_firstTime = false;
				_stopwatch.Restart();
				result = true;
			}

			return result;
		}
	}
}