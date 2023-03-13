using Vixen.Data.Value;
using Vixen.Sys.LayerMixing;

namespace Vixen.Sys
{
	public interface IIntentState : IDispatchable
	{
		IIntentState Clone();
		object GetValue();
		ILayer Layer { get; }
	}

	public interface IIntentState<out T> : IIntentState
		where T : IIntentDataType
	{
		new T GetValue();
	}
}