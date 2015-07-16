using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Common.ValueTypes;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Candle
{
	public class CandleModule : EffectModuleInstanceBase
	{
		private EffectIntents _effectIntents;
		private Random _r;
		private CandleData _data;

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (CandleData) value;
				IsDirty = true;
			}
		}

		protected override void TargetNodesChanged()
		{
			//Nothing to do
		}

		[Value]
		[ProviderCategory(@"Flicker")]
		[ProviderDisplayName(@"Frequency")]
		[ProviderDescription(@"FlickerFrequency")]
		public int FlickerFrequency
		{
			get { return _data.FlickerFrequency; }
			set
			{
				_data.FlickerFrequency = value;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Flicker")]
		[ProviderDisplayName(@"ChangePercent")]
		[ProviderDescription(@"ChangePercent")]
		public Percentage ChangePercentage
		{
			get { return _data.ChangePercentage; }
			set
			{
				_data.ChangePercentage = value;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness")]
		[ProviderDisplayName(@"MinBrightness")]
		[ProviderDescription(@"MinBrightness")]
		public Percentage MinLevel
		{
			get { return _data.MinLevel; }
			set
			{
				_data.MinLevel = value;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness")]
		[ProviderDisplayName(@"MaxBrightness")]
		[ProviderDescription(@"MaxBrightness")]
		public Percentage MaxLevel
		{
			get { return _data.MaxLevel; }
			set
			{
				_data.MaxLevel = value;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Flicker")]
		[ProviderDisplayName(@"FlickerPercent")]
		[ProviderDescription(@"FlickerPercent")]
		public Percentage FlickerFrequencyDeviationCap
		{
			get { return _data.FlickerFrequencyDeviationCap; }
			set
			{
				_data.FlickerFrequencyDeviationCap = value;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Flicker")]
		[ProviderDisplayName(@"DeviationPercent")]
		[ProviderDescription(@"DeviationPercent")]
		public Percentage ChangePercentageDeviationCap
		{
			get { return _data.ChangePercentageDeviationCap; }
			set
			{
				_data.ChangePercentageDeviationCap = value;
				OnPropertyChanged();
			}
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_effectIntents = new EffectIntents();
			_r = new Random();

			foreach (Element element in TargetNodes.SelectMany(x => x))
			{
				
				if (cancellationToken != null && cancellationToken.IsCancellationRequested)
					return;

				if (element != null)
					_RenderCandleOnElement(element);

			}
		}

		protected override EffectIntents _Render()
		{
			return _effectIntents;
		}

		private void _RenderCandleOnElement(Element element)
		{
			float startTime = 0;
			float endTime = (float) TimeSpan.TotalMilliseconds;

			float currentLevel = _GenerateStartingLevel();

			while (startTime < endTime) {
				// What will our new value be?
				float currentLevelChange = _GenerateLevelChange();
				float nextLevel = currentLevel + currentLevelChange;

				// Make sure we're still within our bounds.
				nextLevel = Math.Max(nextLevel, _data.MinLevel);
				nextLevel = Math.Min(nextLevel, _data.MaxLevel);

				// How long will this state last?
				float stateLength = _GenerateStateLength();

				// Make sure we don't exceed the end of the effect.
				stateLength = Math.Min(stateLength, endTime - startTime);

				// Add the intent.
				LightingValue startValue = new LightingValue(Color.White, currentLevel);
				LightingValue endValue = new LightingValue(Color.White, nextLevel);
				IIntent intent = new LightingIntent(startValue, endValue, TimeSpan.FromMilliseconds(stateLength));
				try {
					_effectIntents.AddIntentForElement(element.Id, intent, TimeSpan.FromMilliseconds(startTime));
				}
				catch (Exception e) {
					Console.WriteLine(e.ToString());
					throw;
				}
				startTime += stateLength;
				currentLevel = nextLevel;
			}
		}

		private float _GenerateStartingLevel()
		{
			return (float) _r.NextDouble();
		}

		private float _GenerateStateLength()
		{
			float stateLength = 1000f/_data.FlickerFrequency;

			int deviationCap = (int) (_data.FlickerFrequencyDeviationCap*100);
			float deviation = _r.Next(-deviationCap, deviationCap)*0.01f;

			return stateLength + stateLength*deviation;
		}

		private float _GenerateLevelChange()
		{
			float change = _data.ChangePercentage;

			int deviationCap = (int) (_data.ChangePercentageDeviationCap*100);
			float deviation = _r.Next(-deviationCap, deviationCap)*0.01f;

			int changeDirection = _r.Next(-1, 2);

			return (change + change*deviation)*changeDirection;
		}
	}
}