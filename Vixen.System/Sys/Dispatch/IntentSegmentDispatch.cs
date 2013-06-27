using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch
{
	public abstract class IntentSegmentDispatch : IAnyIntentSegmentHandler
	{
		public virtual void Handle(IIntentSegment<LightingValue> obj)
		{
		}

		public virtual void Handle(IIntentSegment<PositionValue> obj)
		{
		}

		public virtual void Handle(IIntentSegment<ColorValue> obj)
		{
		}

		public virtual void Handle(IIntentSegment<CommandValue> obj)
		{
		}
	}
}