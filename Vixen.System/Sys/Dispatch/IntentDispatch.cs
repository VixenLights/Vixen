using Vixen.Data.Value;

namespace Vixen.Sys.Dispatch
{
	public abstract class IntentDispatch : IAnyIntentHandler
	{
		public virtual void Handle(IIntent<LightingValue> obj)
		{
		}

		public virtual void Handle(IIntent<PositionValue> obj)
		{
		}

		public virtual void Handle(IIntent<CommandValue> obj)
		{
		}

		public virtual void Handle(IIntent<RGBValue> obj)
		{
		}
		public virtual void Handle(IIntent<DiscreteValue> obj)
		{
		}
		public virtual void Handle(IIntent<IntensityValue> obj)
		{
		}
	}
}