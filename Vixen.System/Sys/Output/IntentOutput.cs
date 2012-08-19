using System;

namespace Vixen.Sys.Output {
	public class IntentOutput : Output {
		internal IntentOutput(Guid id, string name)
			: base(id, name) {
		}

		public IntentChangeCollection IntentChangeCollection { get; set; }
	}
}
