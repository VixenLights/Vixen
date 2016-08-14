using System;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{
	internal class IntentState<ResultType> : Dispatchable<IntentState<ResultType>>, IIntentState<ResultType>
		where ResultType : IIntentDataType
	{
		public IntentState(IIntent<ResultType> intent, TimeSpan intentRelativeTime) : this(intent, intentRelativeTime, SequenceLayers.GetDefaultLayer())
		{

		}

		public IntentState(IIntent<ResultType> intent, TimeSpan intentRelativeTime, ILayer layer)
		{
			if (intent == null) throw new ArgumentNullException("intent");

			Intent = intent;
			RelativeTime = intentRelativeTime;
			Layer = layer;
		}

		public IIntent<ResultType> Intent { get; private set; }

		IIntent IIntentState.Intent
		{
			get { return Intent; }
		}

		public TimeSpan RelativeTime { get; set; }

		public ResultType GetValue()
		{
			return Intent.GetStateAt(RelativeTime);
		}

		public ILayer Layer { get; set; }

		object IIntentState.GetValue()
		{
			return GetValue();
		}

		public IIntentState Clone()
		{
			IntentState<ResultType> newIntentState = new IntentState<ResultType>(Intent, RelativeTime, Layer);

			return newIntentState;
		}
	}
}