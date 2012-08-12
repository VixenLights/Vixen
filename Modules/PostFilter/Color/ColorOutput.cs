using System.Linq;
using Vixen.Data.Flow;

namespace VixenModules.OutputFilter.Color {
	class ColorOutput : IDataFlowOutput<IntentsDataFlowData> {
		private ColorComponentFilter _filter;

		public ColorOutput(ColorComponentFilter filter) {
			_filter = filter;
		}

		public void ProcessInputData(IntentsDataFlowData data) {
			Data = new IntentsDataFlowData(data.Value.Select(_filter.Filter));
		}

		public IntentsDataFlowData Data { get; private set; }

		IDataFlowData IDataFlowOutput.Data {
			get { return Data; }
		}

		public string Name {
			get { return _filter.FilterName; }
		}
	}
}
