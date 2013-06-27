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

			if (_firstTime || _stopwatch.ElapsedMilliseconds > _threshold) {
				_firstTime = false;
				_stopwatch.Restart();
				result = true;
			}

			return result;
		}
	}
}