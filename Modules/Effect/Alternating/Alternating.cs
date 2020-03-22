using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Alternating
{
	public class Alternating : BaseEffect
	{
		private EffectIntents _elementData;
		private AlternatingData _data;
		private IEnumerable<IMark> _marks = null;
		public Alternating()
		{
			_data = new AlternatingData();
			InitAllAttributes();
		}

		protected override void TargetNodesChanged()
		{
			if (TargetNodes.Any())
			{
				CheckForInvalidColorData();
				var firstNode = TargetNodes.FirstOrDefault();
				if (firstNode != null && DepthOfEffect > firstNode.GetMaxChildDepth() - 1)
				{
					DepthOfEffect = 0;
				}
			}
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_elementData = new EffectIntents();

			var renderNodes = GetNodesToRenderOn();
			
			_elementData.Add(RenderNode(renderNodes));

		}

		private IEnumerable<IElementNode> GetNodesToRenderOn()
		{
			IEnumerable<IElementNode> renderNodes = TargetNodes;

			if (!EnableDepth)
			{
				renderNodes = TargetNodes.SelectMany(x => x.GetLeafEnumerator());
			}
			else
			{
				for (int i = 0; i < DepthOfEffect; i++)
				{
					renderNodes = renderNodes.SelectMany(x => x.Children);
				}
			}
			
			// If the given DepthOfEffect results in no nodes (because it goes "too deep" and misses all nodes), 
			// then we'll default to the LeafElements, which will at least return 1 element (the TargetNode)
			if (!renderNodes.Any())
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
				foreach (GradientLevelPair t in Colors)
				{
					if (!t.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
					{
						t.ColorGradient = new ColorGradient(validColors.First());
						changed = true;
					}
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
				_data = value as AlternatingData;
				CheckForInvalidColorData();
				InitAllAttributes();
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		#region Color

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradients")]
		[ProviderDescription(@"GradientLevelPair")]
		public List<GradientLevelPair> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config

		[Value]
		[ProviderCategory("Config", 1)]
		[DisplayName(@"Timing Source")]
		[Description(@"Selects what source is used to determine change.")]
		[PropertyOrder(0)]
		public AlternatingMode AlternatingMode
		{
			get
			{
				return _data.AlternatingMode;
			}
			set
			{
				if (_data.AlternatingMode != value)
				{
					_data.AlternatingMode = value;
					UpdateAlternatingModeAttributes();
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Mark Collection")]
		[ProviderDescription(@"Mark Collection that has the phonemes to align to.")]
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
		[ProviderDisplayName(@"StaticEffect")]
		[ProviderDescription(@"StaticEffect")]
		[TypeConverter(typeof(BooleanStringTypeConverter))]
		[BoolDescription("Yes", "No")]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public bool EnableStatic
		{
			get { return _data.EnableStatic; }
			set
			{
				_data.EnableStatic = value;
				IsDirty = true;
				UpdateIntervalAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"GroupLevel")]
		[ProviderDescription(@"GroupLevel")]
		[NumberRange(1, 5000, 1)]
		[PropertyOrder(3)]
		public int GroupLevel
		{
			get { return _data.GroupLevel; }
			set
			{
				_data.GroupLevel = value>0?value:1;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Interval")]
		[ProviderDescription(@"Interval")]
		[NumberRange(0, 10000, 1, 0)]
		[PropertyOrder(4)]
		public int Interval
		{
			get { return _data.Interval; }
			set
			{
				_data.Interval = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"IntervalSkip")]
		[ProviderDescription(@"IntervalSkip")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(5)]
		public int IntervalSkipCount
		{
			get { return _data.IntervalSkipCount; }
			set
			{
				_data.IntervalSkipCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}


		#endregion

		#region Depth

		[Value]
		[ProviderCategory(@"Depth", 4)]
		[ProviderDisplayName(@"Depth")]
		[ProviderDescription(@"Depth")]
		[TypeConverter(typeof(TargetElementDepthConverter))]
		[PropertyEditor("SelectionEditor")]
		public int DepthOfEffect
		{
			get { return _data.DepthOfEffect; }
			set
			{
				_data.DepthOfEffect = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Depth", 10)]
		[ProviderDisplayName(@"IndividualElements")]
		[ProviderDescription(@"AlternatingDepth")]
		public bool EnableDepth
		{
			get { return (bool)_data.EnableDepth; }
			set
			{
				_data.EnableDepth = value;
				IsDirty = true;
				UpdateDepthAttributes();
				TypeDescriptor.Refresh(this);
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/alternating/"; }
		}

		#endregion

		#region Attributes

		private void InitAllAttributes()
		{
			UpdateIntervalAttribute(false);
			UpdateAlternatingModeAttributes(false);
			UpdateDepthAttributes();
			TypeDescriptor.Refresh(this);
		}
		
		private void UpdateAlternatingModeAttributes(bool refresh=true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1); 
			propertyStates.Add("MarkCollectionId", AlternatingMode == AlternatingMode.MarkCollection);
			propertyStates.Add("Interval", AlternatingMode == AlternatingMode.TimeInterval);
			propertyStates.Add("EnableStatic", AlternatingMode == AlternatingMode.TimeInterval);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateIntervalAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("IntervalSkipCount", !EnableStatic);
			propertyStates.Add("Interval", !EnableStatic);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateDepthAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"DepthOfEffect", EnableDepth}
			};
			SetBrowsable(propertyStates);
		}

		#endregion

		public override bool IsDirty
		{
			get
			{
				if (Colors.Any(x => !x.ColorGradient.CheckLibraryReference() || !x.Curve.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		//(Numbers represent color/curve pairs, rows are elements columns are intervals)
		//12341234
		//23412341
		//34123412
		//41234123

		//An offset of 2
		//12341234
		//34123412
		//12341234
		//34123412

		//Offset 3
		//12341234
		//41234123
		//23412341
		//12341234

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private EffectIntents RenderNode(IEnumerable<IElementNode> nodes)
		{
			EffectIntents effectIntents = new EffectIntents();
			int intervals = 1;
			var gradientLevelItem = 0;
			var startIndexOffset = 0;
			//make local hold variables to prevent changes in the middle of rendering.
			int group = GroupLevel;
			var skip = IntervalSkipCount;
			int colorCount = Colors.Count();
			TimeSpan intervalTime = TimeSpan;
			List<TimeSpan> markInterval = new List<TimeSpan>();
			//Use a single pulse to do our work, we don't need to keep creating it and then thowing it away making the GC work
			//hard for no reason.
			
			if (AlternatingMode == AlternatingMode.MarkCollection)
			{
				SetupMarks();
				if (_marks != null) markInterval.AddRange(_marks.Select(mark => mark.StartTime - StartTime));
				markInterval.Add(TimeSpan);
				intervals = markInterval.Count;
			}
			else
			{
				if (!EnableStatic)
				{
					intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds / Interval));
					if (intervals >= 1) intervalTime = TimeSpan.FromMilliseconds(Interval);
				}
			}
			
			var elements = nodes.Select((x, index) => new { x, index })
				.GroupBy(x => x.index / group, y => y.x);

			var startTime = TimeSpan.Zero;
			for (var i = 0; i < intervals; i++)
			{
				if (AlternatingMode == AlternatingMode.MarkCollection) intervalTime = markInterval[i] - startTime;

				foreach (IGrouping<int, IElementNode> elementGroup in elements)
				{
					var glp = Colors[gradientLevelItem];
					foreach (var element in elementGroup)
					{
						RenderElement(glp, startTime, intervalTime.Subtract(TimeSpan.FromMilliseconds(1)), element, effectIntents);
					}
					gradientLevelItem = ++gradientLevelItem % colorCount;
				}

				startIndexOffset = (skip + startIndexOffset) % colorCount;
				gradientLevelItem = startIndexOffset;

				startTime = AlternatingMode == AlternatingMode.TimeInterval ? startTime + intervalTime : markInterval[i];
			}

			return effectIntents;
		}

		private void RenderElement(GradientLevelPair gradientLevelPair, TimeSpan startTime, TimeSpan interval,
			IElementNode element, EffectIntents effectIntents)
		{
			if (interval <= TimeSpan.Zero) return;
			var result = PulseRenderer.RenderNode(element, gradientLevelPair.Curve, gradientLevelPair.ColorGradient, interval, HasDiscreteColors);
			result.OffsetAllCommandsByTime(startTime);
			effectIntents.Add(result);
		}

		private void SetupMarks()
		{
			IMarkCollection mc = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			_marks = mc?.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);

		}

		/// <inheritdoc />
		protected override void MarkCollectionsChanged()
		{
			if (AlternatingMode == AlternatingMode.MarkCollection)
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