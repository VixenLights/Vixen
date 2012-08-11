using System;

namespace Vixen.Data.Flow {
	public interface IDataFlowComponent {
		Guid DataFlowComponentId { get; }
		DataFlowType InputDataType { get; }
		DataFlowType OutputDataType { get; }
		IDataFlowOutput[] Outputs { get; }
		IDataFlowComponentReference Source { get; set; }
		//This is done as a channel (channel layer update), filter (filter layer update), or output (output layer update)
		//void Update(); // Update outputs by getting data from the source and doing whatever with it.
	}

	public interface IDataFlowComponent<T> : IDataFlowComponent
		where T : IDataFlowData {
		IDataFlowOutput<T>[] Outputs { get; }
		IDataFlowComponentReference<T> Source { get; set; }
	}
}
