using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Strobe
{
	public class Strobe : BaseEffect
	{
		private EffectIntents _elementData;
		private StrobeData _data;
		private IEnumerable<IMark> _marks = null;
		private List<StrobeClass> _strobeClass;

		public Strobe()
		{
			_data = new StrobeData();
			InitAllAttributes();
		}

		protected override void TargetNodesChanged()
		{
			if (TargetNodes.Any())
			{
				CheckForInvalidColorData();
			}
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_elementData = new EffectIntents();

			var renderNodes = GetNodesToRenderOn().ToList();

			_elementData.Add(RenderNode(renderNodes));

		}

		private IEnumerable<ElementNode> GetNodesToRenderOn()
		{
			IEnumerable<ElementNode> renderNodes = TargetNodes;
			
			renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());

			return renderNodes;
		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		//we only need to check against 1 color variable,
		//it should be checked at a later time than what this is doing currently
		private void CheckForInvalidColorData()
		{
			// check for sane default colors when first rendering it
			var validColors = GetValidColors();
			if (validColors.Any())
			{
				bool changed = false;
				if (!Colors.GetColorsInGradient().IsSubsetOf(validColors))
				{
					Colors = new ColorGradient(validColors.First());
					changed = true;
				}
				if (changed)
				{
					OnPropertyChanged("Colors");
				}
			}

		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as StrobeData;
				CheckForInvalidColorData();
				InitAllAttributes();
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		#region Config

		[Value]
		[ProviderCategory("Config", 1)]
		[ProviderDisplayName(@"TimingSource")]
		[ProviderDescription(@"TimingSource")]
		[PropertyOrder(0)]
		public StrobeSource StrobeSource
		{
			get
			{
				return _data.StrobeSource;
			}
			set
			{
				if (_data.StrobeSource != value)
				{
					_data.StrobeSource = value;
					UpdateStrobeModeAttributes();
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"StrobeMarkCollection")]
		[TypeConverter(typeof(IMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(1)]
		public string MarkCollectionId
		{
			get
			{
				return MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId)?.Name;
			}
			set
			{
				var newMarkCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(value));
				var id = newMarkCollection?.Id ?? Guid.Empty;
				if (!id.Equals(_data.MarkCollectionId))
				{
					var oldMarkCollection = MarkCollections.FirstOrDefault(x => x.Id.Equals(_data.MarkCollectionId));
					RemoveMarkCollectionListeners(oldMarkCollection);
					_data.MarkCollectionId = id;
					AddMarkCollectionListeners(newMarkCollection);
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CycleTime")]
		[ProviderDescription(@"CycleTime")]
		[NumberRange(0, 10000, 1, 0)]
		[PropertyOrder(2)]
		public int CycleTime
		{
			get { return _data.CycleTime; }
			set
			{
				_data.CycleTime = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"StrobeMode")]
		[ProviderDescription(@"StrobeMode")]
		[PropertyOrder(3)]
		public StrobeMode StrobeMode
		{
			get { return _data.StrobeMode; }
			set
			{
				_data.StrobeMode = value;
				UpdateStrobeModeAttributes();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CycleVariation")]
		[ProviderDescription(@"CycleVariation")]
		[PropertyOrder(4)]
		public Curve CycleVariationCurve
		{
			get { return _data.CycleVariationCurve; }
			set
			{
				_data.CycleVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"OnTime")]
		[ProviderDescription(@"OnTime")]
		[PropertyOrder(5)]
		public Curve OnTimeCurve
		{
			get { return _data.OnTimeCurve; }
			set
			{
				_data.OnTimeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		#endregion

		#region Pulse

		[Value]
		[ProviderCategory(@"Pulse", 2)]
		[ProviderDisplayName(@"ColorGradient")]
		[ProviderDescription(@"Gradient")]
		[PropertyOrder(1)]
		public ColorGradient Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Pulse", 2)]
		[ProviderDisplayName(@"Intensity")]
		[ProviderDescription(@"Intensity")]
		[PropertyOrder(2)]
		public Curve IntensityCurve
		{
			get { return _data.IntensityCurve; }
			set
			{
				_data.IntensityCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Due to inherent limitations within lighting protocols, fast strobing with short cycle times below 100ms may not be possible. \n\n\r Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/strobe/"; }
		}

		#endregion

		#region Attributes

		private void InitAllAttributes()
		{
			UpdateStrobeModeAttributes(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateStrobeModeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
			{
				{"MarkCollectionId", StrobeSource != StrobeSource.TimeInterval},
				{"OnTimeCurve", StrobeMode == StrobeMode.Advanced},
				{"CycleVariationCurve", StrobeMode == StrobeMode.Advanced & StrobeSource == StrobeSource.TimeInterval},
				{"CycleTime", StrobeSource == StrobeSource.TimeInterval}
			};
			SetBrowsable(propertyStates);
			
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		#endregion

		public override bool IsDirty
		{
			get
			{
				if (!Colors.CheckLibraryReference() || !IntensityCurve.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private EffectIntents RenderNode(List<ElementNode> elements)
		{
			_strobeClass = new List<StrobeClass>();
			EffectIntents effectIntents = new EffectIntents();
			var startTime = TimeSpan.Zero;
			double intervalPos = GetEffectTimeIntervalPosition(startTime) * 100;
			double interval;
			int i = 0;

			if (StrobeSource != StrobeSource.TimeInterval)
			{
				SetupMarks();
				if (_marks == null || _strobeClass.Count == 0) return effectIntents;
				interval = _strobeClass[i].CycleTime;
			}
			else
			{
				interval = StrobeMode == StrobeMode.Simple ? CycleTime : CalculateInterval(intervalPos);
			}

			TimeSpan intervalTime = TimeSpan.FromMilliseconds(interval);
			TimeSpan onDuration = TimeSpan.FromMilliseconds(interval * (StrobeMode == StrobeMode.Simple ? 0.5 : OnTimeCurve.GetValue(intervalPos) / 100));
			
			// Continue until the intent start time is >= effect duration.
			do
			{
				// Adjust the onDuration if intent end time is greater then the next marks start time.
				if (StrobeSource != StrobeSource.TimeInterval && (onDuration + startTime).TotalMilliseconds >
				    _strobeClass[i + 1].StartTime.TotalMilliseconds) onDuration = TimeSpan.FromMilliseconds((_strobeClass[i + 1].StartTime.TotalMilliseconds - 1 - startTime.TotalMilliseconds)/2);

				foreach (ElementNode element in elements)
				{
					var glp = new GradientLevelPair(Colors, IntensityCurve);
					RenderElement(glp, startTime, onDuration.Subtract(TimeSpan.FromMilliseconds(1)), element,
						effectIntents);
				}

				startTime = startTime + intervalTime;
				intervalPos = GetEffectTimeIntervalPosition(startTime) * 100;

				// Get next Interval
				if (StrobeSource != StrobeSource.TimeInterval)
				{
					if (_strobeClass[i+1].StartTime.TotalMilliseconds < startTime.TotalMilliseconds)
					{
						i++;
						startTime = _strobeClass[i].StartTime;
						if (i > _strobeClass.Count - 1) break;
					}
					interval = _strobeClass[i].CycleTime;
				}
				else
				{
					interval = StrobeMode == StrobeMode.Simple ? CycleTime : CalculateInterval(intervalPos);
				}
				
				onDuration = TimeSpan.FromMilliseconds(interval * (StrobeMode == StrobeMode.Simple ? 0.5 : OnTimeCurve.GetValue(intervalPos) / 100));
				intervalTime = TimeSpan.FromMilliseconds(interval);
			} while (startTime.TotalMilliseconds < TimeSpan.TotalMilliseconds);

			_strobeClass = null;
			return effectIntents;
		}

		private void RenderElement(GradientLevelPair gradientLevelPair, TimeSpan startTime, TimeSpan interval,
			ElementNode element, EffectIntents effectIntents)
		{
			if (interval <= TimeSpan.Zero) return;
			var result = PulseRenderer.RenderNode(element, gradientLevelPair.Curve, gradientLevelPair.ColorGradient, interval, HasDiscreteColors);
			result.OffsetAllCommandsByTime(startTime);
			effectIntents.Add(result);
		}

		private double CalculateInterval(double intervalPosFactor)
		{
			double value = (int)ScaleCurveToValue(CycleVariationCurve.GetValue(intervalPosFactor), CycleTime, 1);
			if (value < 1) value = 1;
			return value;
		}

		// Strobe Class
		public class StrobeClass
		{
			public int CycleTime;
			public TimeSpan StartTime;
		}

		private void SetupMarks()
		{
			IMarkCollection mc = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			_marks = mc?.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);
			if (_marks == null) return;

			bool firstMark = true;
			StrobeClass t;

			foreach (var mark in _marks)
			{
				if (firstMark && mark.StartTime > StartTime)
				{
					t = new StrobeClass {StartTime = TimeSpan.Zero, CycleTime = (int) (StrobeSource == StrobeSource.MarkCollectionLabelDuration ? mark.Duration.TotalMilliseconds : GetMarkLabelValue(mark.Text))};
					_strobeClass.Add(t);
					firstMark = false;
				}

				if (mark.StartTime < StartTime) continue;
				t = new StrobeClass {CycleTime = (int)(StrobeSource == StrobeSource.MarkCollectionLabelDuration ? mark.Duration.TotalMilliseconds : GetMarkLabelValue(mark.Text)), StartTime = mark.StartTime - StartTime};
				_strobeClass.Add(t);
				firstMark = false;
			}

			if (_strobeClass.Count > 0)
			{
				t = new StrobeClass {CycleTime = _strobeClass.Last().CycleTime, StartTime = TimeSpan};
				_strobeClass.Add(t);
			}

		}

		private int GetMarkLabelValue(string markLabel)
		{
			bool parsed = Int32.TryParse(markLabel, out var cycleTime);
			if (!parsed) cycleTime = 150;
			return cycleTime;
		}

		/// <inheritdoc />
		protected override void MarkCollectionsChanged()
		{
			if (StrobeSource != StrobeSource.TimeInterval)
			{
				var markCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(MarkCollectionId));
				InitializeMarkCollectionListeners(markCollection);
			}
		}

		/// <inheritdoc />
		protected override void MarkCollectionsRemoved(IList<IMarkCollection> addedCollections)
		{
			var mc = addedCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			if (mc != null)
			{
				//Our collection is gone!!!!
				RemoveMarkCollectionListeners(mc);
				MarkCollectionId = String.Empty;
			}
		}
	}

}