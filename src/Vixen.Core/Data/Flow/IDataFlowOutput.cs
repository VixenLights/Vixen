namespace Vixen.Data.Flow
{
	public interface IDataFlowOutput
	{
		string Name { get; }
		IDataFlowData Data { get; }
	}

	public interface IDataFlowOutput<out T> : IDataFlowOutput
		where T : IDataFlowData
	{
		new T Data { get; }
	}
}