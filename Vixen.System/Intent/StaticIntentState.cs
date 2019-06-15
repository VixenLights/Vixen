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

		public void SetValue(ResultType value)
		{
			_value = value;
		}

		public ref ResultType GetValueRef()
		{
			return ref _value;
		}

		public ResultType GetValue()
		{
			return _value;
		}

		public ILayer Layer { get; set; }

		object IIntentState.GetValue()
		{
			return GetValue();
		}

		public IIntentState Clone()
		{
			return new StaticIntentState<ResultType>(_value);
		}
	}
}