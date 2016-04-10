using Vixen.Data.Flow;

namespace Vixen.Sys.Dispatch
{
	public abstract class DataFlowDataDispatch : IAnyDataFlowDataHandler
	{
		public virtual void Handle(CommandDataFlowData obj)
		{
		}

		public virtual void Handle(CommandsDataFlowData obj)
		{
		}

		public virtual void Handle(IntentsDataFlowData obj)
		{
		}

		public virtual void Handle(IntentDataFlowData obj)
		{
		}
	}
}