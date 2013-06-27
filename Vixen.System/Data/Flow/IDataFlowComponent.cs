using System;

namespace Vixen.Data.Flow
{
	public interface IDataFlowComponent
	{
		Guid DataFlowComponentId { get; }
		DataFlowType InputDataType { get; }
		DataFlowType OutputDataType { get; }
		IDataFlowOutput[] Outputs { get; }
		IDataFlowComponentReference Source { get; set; }
		string Name { get; }
	}

	public interface IDataFlowComponent<T> : IDataFlowComponent
		where T : IDataFlowData
	{
		IDataFlowOutput<T>[] Outputs { get; }
		IDataFlowComponentReference<T> Source { get; set; }
	}
}