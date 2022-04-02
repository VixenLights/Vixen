using System.Collections.Generic;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.StateCombinator
{
	public class StateCombinator<T> : Dispatchable<T>, IStateCombinator, IAnyIntentStateHandler
		where T : StateCombinator<T>
	{
		public StateCombinator()
		{
			StateCombinatorValue = new List<IIntentState>(4);
		}

		public virtual List<IIntentState> Combine(List<IIntentState> states)
		{
			StateCombinatorValue.Clear();
			//TODO consider layers and may not need to iterate all of them. Need them in a sorted order by layer
			foreach (var intentState in states)
			{
				intentState.Dispatch(this);
			}
			
			return StateCombinatorValue;
		}

		// Opt-in, not opt-out.  Default handlers will not be called
		// from the base class.

		public virtual void Handle(IIntentState<RGBValue> obj)
		{
		}

		public virtual void Handle(IIntentState<LightingValue> obj)
		{
		}

		public virtual void Handle(IIntentState<RangeValue<FunctionIdentity>> obj)
		{
			StateCombinatorValue.Add(obj);
		}

		public virtual void Handle(IIntentState<CommandValue> obj)
		{
			StateCombinatorValue.Add(obj);
		}

		public virtual void Handle(IIntentState<DiscreteValue> obj)
		{
		}
		public virtual void Handle(IIntentState<IntensityValue> obj)
		{
		}

		protected List<IIntentState> StateCombinatorValue { get; set; }
	}
}
