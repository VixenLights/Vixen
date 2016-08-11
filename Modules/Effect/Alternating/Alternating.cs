using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;

namespace VixenModules.Effect.Alternating
{
	public class Alternating : BaseEffect
	{
		private EffectIntents _elementData;
		private AlternatingData _data;

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
			}
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_elementData = new EffectIntents();

			foreach (ElementNode node in TargetNodes)
			{
				if (node != null)
					_elementData.Add(RenderNode(node));
			}

			//_elementData = IntentBuilder.ConvertToStaticArrayIntents(_elementData, TimeSpan, IsDiscrete());
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
		[ProviderDisplayName(@"GradientLevelPair")]
		[ProviderDescription(@"GradientLevelPair")]
		[MergableProperty(false)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"StaticEffect")]
		[ProviderDescription(@"StaticEffect")]
		[TypeConverter(typeof(BooleanStringTypeConverter))]
		[BoolDescription("Yes", "No")]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(0)]
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
		[PropertyOrder(1)]
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
		[PropertyOrder(2)]
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
		[PropertyOrder(3)]
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


		#region Attributes

		private void InitAllAttributes()
		{
			UpdateIntervalAttribute(false);
			TypeDescriptor.Refresh(this);
		}


		private void UpdateIntervalAttribute(bool refresh=true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2);
			propertyStates.Add("IntervalSkipCount", !EnableStatic);
			propertyStates.Add("Interval", !EnableStatic);
			SetBrowsable(propertyStates);
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
		private EffectIntents RenderNode(ElementNode node)
		{
			EffectIntents effectIntents = new EffectIntents();
			int intervals = 1;
			var gradientLevelItem = 0;
			var startIndexOffset = 0;
			//make local hold variables to prevent changes in the middle of rendering.
			int group = GroupLevel;
			var skip = IntervalSkipCount;
			int colorCount = Colors.Count();
			//Use a single pulse to do our work, we don't need to keep creating it and then thowing it away making the GC work
			//hard for no reason.
			
			if (!EnableStatic)
			{
				intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds/Interval));
			}

			var startTime = TimeSpan.Zero;
			var nodes = node.GetLeafEnumerator();

			var intervalTime = intervals == 1
					? TimeSpan
					: TimeSpan.FromMilliseconds(Interval);

			
			for (int i = 0; i < intervals; i++)
			{
				var elements = nodes.Select((x, index) => new { x, index })
					.GroupBy(x => x.index / group, y => y.x);

				foreach (IGrouping<int, ElementNode> elementGroup in elements)
				{
					var glp = Colors[gradientLevelItem];
					foreach (var element in elementGroup)
					{
						RenderElement(glp, startTime, intervalTime, element, effectIntents);
					}
					gradientLevelItem = ++gradientLevelItem % colorCount;

				}

				startIndexOffset = (skip+startIndexOffset) % colorCount;
				gradientLevelItem = startIndexOffset;
				
				startTime += intervalTime;
			}

			return effectIntents;
		}

		private void RenderElement(GradientLevelPair gradientLevelPair, TimeSpan startTime, TimeSpan interval,
			ElementNode element, EffectIntents effectIntents)
		{
			var result = PulseRenderer.RenderNode(element, gradientLevelPair.Curve, gradientLevelPair.ColorGradient, interval, HasDiscreteColors);
			result.OffsetAllCommandsByTime(startTime);
			effectIntents.Add(result);
		}
	}

}