using Vixen.Sys;

namespace Vixen.Data.Flow
{
	public interface IDataFlowData : IDispatchable
	{
		object Value { get; }
	}

	public interface IDataFlowData<out T> : IDataFlowData
	{
		new T Value { get; }
	}
}