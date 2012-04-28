using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Execution {
	class SequenceDataSource : IDataSource {
		private EffectNodeQueue _data;

		public SequenceDataSource(IEnumerable<IEffectNode> sequenceData) {
			_data = new EffectNodeQueue(sequenceData.OrderBy(x => x.StartTime));
		}

		public IEnumerable<IEffectNode> GetDataAt(TimeSpan time) {
			return _data.Get(time);
		}
	}
}
