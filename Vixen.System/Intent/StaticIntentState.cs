using System;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class StaticIntentState<ResultType> : Dispatchable<StaticIntentState<ResultType>>, IIntentState<ResultType>
		where ResultType : IIntentDataType
	{
		private IIntentState<ResultType> _originalIntentState;
		private ResultType _value;

		public StaticIntentState(IIntentState<ResultType> originalIntentState, ResultType value)
		{
			_originalIntentState = originalIntentState;
			_value = value;
		}

		public IIntent<ResultType> Intent
		{
			get { return _originalIntentState.Intent; }
		}

		IIntent IIntentState.Intent
		{
			get { return Intent; }
		}

		public ResultType GetValue()
		{
			return _value;
		}

		object IIntentState.GetValue()
		{
			return GetValue();
		}

		public TimeSpan RelativeTime
		{
			get { return _originalIntentState.RelativeTime; }
		}

		public IIntentState Clone()
		{
			return new StaticIntentState<ResultType>(_originalIntentState, _value);
		}
	}
}