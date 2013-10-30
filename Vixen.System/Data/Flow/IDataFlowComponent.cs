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
		new IDataFlowOutput<T>[] Outputs { get; }
		new IDataFlowComponentReference<T> Source { get; set; }
	}
}