using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch
{
	public abstract class IntentSegmentDispatch : IAnyIntentSegmentHandler
	{
		public virtual void Handle(IIntentSegment<LightingValue> obj)
		{
		}
		
		public virtual void Handle(IIntentSegment<RangeValue<FunctionIdentity>> obj)
		{
		}

		public virtual void Handle(IIntentSegment<RGBValue> obj)
		{
		}

		public virtual void Handle(IIntentSegment<CommandValue> obj)
		{
		}
	}
}