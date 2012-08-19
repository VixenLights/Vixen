using System;
using Vixen.Data.Flow;

namespace Vixen.Sys.Output {
	abstract public class Output {
		internal protected Output(Guid id, string name) {
			Id = id;
			Name = name;
		}

		// Completely independent; nothing is current dependent upon this value.
		public string Name { get; set; }

		public Guid Id { get; private set; }

		public void Update() {
			if(Source != null) {
				State = Source.GetOutputState();
			}
		}

		public IDataFlowComponentReference Source { get; set; }

		public IDataFlowData State { get; private set; }
	}
}
