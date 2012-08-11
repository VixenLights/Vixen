using System;
using Vixen.Sys.Output;

namespace Vixen.Data.Flow {
	/// <summary>
	/// Facilitates allowing intent outputs to participate in the data flow system.
	/// </summary>
	class IntentOutputDataFlowAdapter : IDataFlowComponent {
		private IntentOutput _output;

		public IntentOutputDataFlowAdapter(IntentOutput output) {
			_output = output;
		}

		public IDataFlowOutput[] Outputs {
			get { return null; }
		}

		public DataFlowType InputDataType {
			get { return DataFlowType.SingleCommand; }
		}

		public DataFlowType OutputDataType {
			get { return DataFlowType.None; }
		}

		public Guid DataFlowComponentId {
			get { return _output.Id; }
		}


		public IDataFlowComponentReference Source {
			get { return _output.Source; }
			set { _output.Source = value; }
		}
	}
}
