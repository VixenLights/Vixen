using System;
using System.Collections.Generic;
using Vixen.Data.Value;
using Vixen.Interpolator;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{
	/// <summary>
	/// This class was built to reduce the overhead of the segmented linear intent. Nothing uses that functionality at the 
	/// current time so this removes all that overhead until such time as it is needed.
	/// </summary>
	/// <typeparam name="TypeOfValue"></typeparam>
	public class NonSegmentedLinearIntent<TypeOfValue> : Dispatchable<NonSegmentedLinearIntent<TypeOfValue>>, IIntent<TypeOfValue>
		where TypeOfValue : IIntentDataType
	{
		private readonly StaticIntentState<TypeOfValue> _intentState;
		private readonly Interpolator<TypeOfValue> _interpolator;

		public NonSegmentedLinearIntent(TypeOfValue startValue, TypeOfValue endValue, TimeSpan timeSpan, Interpolator<TypeOfValue> interpolator = null)
		{
			_interpolator = interpolator ?? Interpolator.Interpolator.Create<TypeOfValue>();
			StartValue = startValue;
			EndValue = endValue;
			TimeSpan = timeSpan;
			_intentState = new StaticIntentState<TypeOfValue>(startValue);
		}

		public TypeOfValue StartValue { get; private set; }

		public TypeOfValue EndValue { get; private set; }

		public TimeSpan TimeSpan { get; private set; }
		public IIntentState CreateIntentState(TimeSpan intentRelativeTime, ILayer layer)
		{
			_intentState.Layer = layer;
			_intentState.SetValue(GetStateAt(intentRelativeTime));
			//_intentState.RelativeTime = intentRelativeTime;
			return _intentState;
		}

		public TypeOfValue GetStateAt(TimeSpan intentRelativeTime)
		{
			TypeOfValue value;
			if (_interpolator.Interpolate(intentRelativeTime, TimeSpan, StartValue, EndValue, out value))
			{
				return value;
			}
			return default(TypeOfValue);
		}

		object IIntent.GetStateAt(TimeSpan intentRelativeTime)
		{
			return GetStateAt(intentRelativeTime);
		}
	}
}
