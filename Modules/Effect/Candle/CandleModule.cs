using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Common.ValueTypes;
using NLog;
using Vixen.Attributes;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Candle
{
	public class CandleModule : BaseEffect
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private EffectIntents _effectIntents;
		private Random _r;
		private CandleData _data;

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = (CandleData) value;
				CheckForInvalidColorData();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"GroupLevel")]
		[ProviderDescription(@"GroupLevel")]
		[NumberRange(1, 5000, 1)]
		[PropertyOrder(1)]
		public int GroupLevel
		{
			get { return _data.GroupLevel; }
			set
			{
				_data.GroupLevel = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		public Color Color
		{
			get
			{
				return _data.Color;
			}
			set
			{
				_data.Color = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Flicker",4)]
		[ProviderDisplayName(@"Frequency")]
		[ProviderDescription(@"FlickerFrequency")]
		public int FlickerFrequency
		{
			get { return _data.FlickerFrequency; }
			set
			{
				_data.FlickerFrequency = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Flicker", 4)]
		[ProviderDisplayName(@"ChangePercent")]
		[ProviderDescription(@"ChangePercent")]
		public Percentage ChangePercentage
		{
			get { return _data.ChangePercentage; }
			set
			{
				_data.ChangePercentage = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness",3)]
		[ProviderDisplayName(@"MinBrightness")]
		[ProviderDescription(@"MinBrightness")]
		public Percentage MinLevel
		{
			get { return _data.MinLevel; }
			set
			{
				_data.MinLevel = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness",3)]
		[ProviderDisplayName(@"MaxBrightness")]
		[ProviderDescription(@"MaxBrightness")]
		public Percentage MaxLevel
		{
			get { return _data.MaxLevel; }
			set
			{
				_data.MaxLevel = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Flicker", 4)]
		[ProviderDisplayName(@"FlickerPercent")]
		[ProviderDescription(@"FlickerPercent")]
		public Percentage FlickerFrequencyDeviationCap
		{
			get { return _data.FlickerFrequencyDeviationCap; }
			set
			{
				_data.FlickerFrequencyDeviationCap = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Flicker", 4)]
		[ProviderDisplayName(@"DeviationPercent")]
		[ProviderDescription(@"DeviationPercent")]
		public Percentage ChangePercentageDeviationCap
		{
			get { return _data.ChangePercentageDeviationCap; }
			set
			{
				_data.ChangePercentageDeviationCap = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/candle-flicker/"; }
		}

		#endregion

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			var validColors = GetValidColors();
			if (validColors.Any())
			{
				if (!validColors.Contains(_data.Color))
				{
					//Our color is not valid for any elements we have.
					//Set a default color 
					Color = validColors.First();
				}
			}
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_effectIntents = new EffectIntents();
			_r = new Random();
		
			var nodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());

			var elementGroup = nodes.Select((x, index) => new { x, index })
					.GroupBy(x => x.index / GroupLevel, y => y.x);

			foreach (IGrouping<int, IElementNode> block in elementGroup)
			{
				_RenderCandleOnElements(block.ToList());
			}

			//_effectIntents = IntentBuilder.ConvertToStaticArrayIntents(_effectIntents, TimeSpan, IsDiscrete());

		}

		protected override EffectIntents _Render()
		{
			return _effectIntents;
		}

		private void _RenderCandleOnElements(List<IElementNode> elements)
		{
			TimeSpan startTime = TimeSpan.Zero;
			double currentLevel = _GenerateStartingLevel();

			while (startTime < TimeSpan)
			{
				// What will our new value be?
				double currentLevelChange = _GenerateLevelChange();
				double nextLevel = currentLevel + currentLevelChange;

				// Make sure we're still within our bounds.
				nextLevel = Math.Max(nextLevel, _data.MinLevel);
				nextLevel = Math.Min(nextLevel, _data.MaxLevel);

				// How long will this state last?
				double stateLength = _GenerateStateLength();
				
				// Make sure we don't exceed the end of the effect.
				stateLength = Math.Min(stateLength, (TimeSpan - startTime).TotalMilliseconds);
				var length = TimeSpan.FromMilliseconds(stateLength);
				if (length == TimeSpan.Zero)
				{
					length = TimeSpan.FromMilliseconds(1);
				}
				else
				{
					IIntent discreteIntent = null;
					IIntent fullColorintent = null;

					// Add the intent.
					try
					{
						foreach (var element in elements)
						{
							if (element != null)
							{
								if (HasDiscreteColors && IsElementDiscrete(element))
								{
									if (discreteIntent == null)
									{
										discreteIntent = CreateDiscreteIntent(Color, currentLevel, nextLevel, length);
									}
									_effectIntents.AddIntentForElement(element.Element.Id, discreteIntent, startTime);
								}
								else
								{
									if (fullColorintent == null)
									{
										fullColorintent = CreateIntent(Color, Color, currentLevel, nextLevel, length);
									}
									_effectIntents.AddIntentForElement(element.Element.Id, fullColorintent, startTime);
								}
							}
						}
					}
					catch (Exception e)
					{
						Logging.Error(e, "Error generating Candle intents");
						throw;
					}
				}

				startTime += length;	
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