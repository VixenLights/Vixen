using System.Diagnostics;

namespace Vixen.Execution {
	class ControllerUpdateAdjudicator {
		private int _threshold;
		private Stopwatch _stopwatch;

		public ControllerUpdateAdjudicator(int thresholdInMilliseconds) {
			_threshold = thresholdInMilliseconds;
			_stopwatch = Stopwatch.StartNew();
		}

		/// <summary>
		/// Determines if an update should be allowed.
		/// </summary>
		/// <returns>If an update should be done.</returns>
		public bool PetitionForUpdate() {
			bool result = false;

			if(_stopwatch.ElapsedMilliseconds > _threshold) {
				_stopwatch.Restart();
				result = true;
			}

			return result;
		}
	}
}
