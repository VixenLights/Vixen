using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	abstract class TransitionIntentState<T, ResultType, InterpolatorType> : IntentState<T, ResultType>
		//where T : TransitionIntentState<T, ResultType, InterpolatorType>
		where T : IntentState<T, ResultType>
		where InterpolatorType : Interpolator<ResultType>, new()
		where ResultType : struct {
		private TransitionIntent<ResultType> _intent;
		private Interpolator<ResultType> _interpolator;

		protected TransitionIntentState(TransitionIntent<ResultType> intent, TimeSpan intentRelativeTime)
			: base(intentRelativeTime) {
			_intent = intent;
			_interpolator = new InterpolatorType();
		}

		override public ResultType GetValue() {
			ResultType value;
			_interpolator.Interpolate(RelativeTime, _intent.TimeSpan, _intent.StartValue, _intent.EndValue, out value);
			return SubordinateIntentState.Aggregate(value, SubordinateIntentStates);
		}

		abstract override protected IntentState<T, ResultType> _Clone();
	}
	//abstract class TransitionIntentState<T, ResultType, InterpolatorType> : Dispatchable<T>, IIntentState<ResultType>
	//    where T : TransitionIntentState<T, ResultType, InterpolatorType>
	//    where InterpolatorType : Interpolator<ResultType>, new()
	//    where ResultType : struct {
	//    private TransitionIntent<ResultType> _intent;
	//    private Interpolator<ResultType> _interpolator;

	//    protected TransitionIntentState(TransitionIntent<ResultType> intent, TimeSpan intentRelativeTime) {
	//        RelativeTime = intentRelativeTime;
	//        _intent = intent;
	//        _interpolator = new InterpolatorType();
	//        FilterStates = new List<IFilterState>();
	//        SubordinateIntentStates = new List<SubordinateIntentState>();
	//    }

	//    public TimeSpan RelativeTime { get; private set; }
	//    public List<IFilterState> FilterStates { get; private set; }
	//    public List<SubordinateIntentState> SubordinateIntentStates { get; private set; }

	//    public ResultType GetValue() {
	//        ResultType value;
	//        _interpolator.Interpolate(RelativeTime, _intent.TimeSpan, _intent.StartValue, _intent.EndValue, out value);
	//        return SubordinateIntentState.Aggregate(value, SubordinateIntentStates);
	//    }

	//    public IIntentState Clone() {
	//        TransitionIntentState<T, ResultType, InterpolatorType> newIntentState = _Clone();
	
	//        if(VixenSystem.EvaluateFilters) {
	//            newIntentState.FilterStates.AddRange(FilterStates);
	//        } else {
	//            newIntentState.FilterStates.Clear();
	//        }

	//        if(VixenSystem.AllowSubordinateEffects) {
	//            newIntentState.SubordinateIntentStates.AddRange(SubordinateIntentStates);
	//        } else {
	//            newIntentState.SubordinateIntentStates.Clear();
	//        }

	//        return newIntentState;
	//    }
	//    abstract protected TransitionIntentState<T, ResultType, InterpolatorType> _Clone();
	//}
}
