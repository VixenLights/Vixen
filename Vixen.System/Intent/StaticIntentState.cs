using System;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{
	public class StaticIntentState<ResultType> : Dispatchable<StaticIntentState<ResultType>>, IIntentState<ResultType>
		where ResultType : IIntentDataType
	{
		private ResultType _value;
		
		public StaticIntentState(ResultType value) : this(value, SequenceLayers.GetDefaultLayer())
		{
		}

		public StaticIntentState(ResultType value, ILayer layer)
		{
			_value = value;
			Layer = layer;
		}

		public IIntent<ResultType> Intent
		{
			//This is a little shaky, but it will satisfy the interface for now.
			//As this is a static state, the original inent has been modified in some way that makes it 
			//somewhat irrelevant at this point. This will attempt to make a intent that represents this 
			//state in case anything wants to use it.
			get { return new StaticIntent<ResultType>(_value, TimeSpan.FromMilliseconds(1)); }
		}

		IIntent IIntentState.Intent
		{
			get { return Intent; }
		}

		public void SetValue(ResultType value)
		{
			_value = value;
		}

		public ResultType GetValue()
		{
			return _value;
		}

		public ILayer Layer { get; private set; }

		object IIntentState.GetValue()
		{
			return GetValue();
		}

		public TimeSpan RelativeTime
		{
			get { return TimeSpan.Zero; }
		}

		public IIntentState Clone()
		{
			return new StaticIntentState<ResultType>(_value);
		}
	}
}