namespace Vixen.Data.Flow
{
	public interface IDataFlowComponentReference
	{
		IDataFlowComponent Component { get; }
		int OutputIndex { get; }
		IDataFlowData GetOutputState();
	}

	public interface IDataFlowComponentReference<T> : IDataFlowComponentReference
		where T : IDataFlowData
	{
		new IDataFlowComponent<T> Component { get; }
	}
}