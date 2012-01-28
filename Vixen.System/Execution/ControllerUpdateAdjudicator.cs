using System;

namespace Vixen.Execution {
	class ControllerUpdateAdjudicator {
		private int _threshold;
		private int _lastUpdate;

		public ControllerUpdateAdjudicator(int thresholdInMilliseconds) {
			_threshold = thresholdInMilliseconds;
		}

		/// <summary>
		/// Determines if an update should be allowed.
		/// </summary>
		/// <returns>If an update should be done.</returns>
		public bool PetitionForUpdate() {
			if(Environment.TickCount - _lastUpdate > _threshold) {
				_lastUpdate = Environment.TickCount;
				return true;
			}
			return false;
		}
	}
}
