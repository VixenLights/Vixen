using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.App.ColorGradients;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;

namespace VixenModules.Effect.Alternating
{

	public class Alternating : EffectModuleInstanceBase
	{
		private AlternatingData _data;
		private EffectIntents _elementData;

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
			EffectIntents data = new EffectIntents();

			foreach (ElementNode node in TargetNodes)
			{
				if (node != null)
					data.Add(RenderNode(node));
			}

			_elementData = data;
		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		//we only need to check against 1 color variable,
		//it should be checked at a later time than what this is doing currently
		private void CheckForInvalidColorData()
		{
			// check for sane default colors when first rendering it
			var validColors = new HashSet<Color>();
			validColors.AddRange(TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));

			//We need to beable to modify the list in the loop, since a collection used in foreach is immuatable we need to use a for loop
			for (int i = 0; i < Colors.Count; i++)
			{
				if (validColors.Any() && !Colors[i].ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
				{
					Colors[i].ColorGradient = new ColorGradient(validColors.First());
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
		[ProviderCategory(@"Config", 10)]
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
			var pulse = new Pulse.Pulse(); 
			
			if (!EnableStatic)
			{
				intervals = Convert.ToInt32(Math.Ceiling(TimeSpan.TotalMilliseconds/Interval));
			}

			var startTime = TimeSpan.Zero;
			var nodes = node.GetLeafEnumerator();

			var intervalTime = intervals == 1
					? TimeSpan
					: TimeSpan.FromMilliseconds(Interval);

			pulse.TimeSpan = intervalTime;
			
			for (int i = 0; i < intervals; i++)
			{
				var elements = nodes.Select((x, index) => new { x, index })
					.GroupBy(x => x.index / group, y => y.x);

				foreach (IGrouping<int, ElementNode> elementGroup in elements)
				{
					var glp = Colors[gradientLevelItem];
					foreach (var element in elementGroup)
					{
						RenderElement(pulse, glp, startTime, element, effectIntents);
					}
					gradientLevelItem = ++gradientLevelItem % colorCount;

				}

				startIndexOffset = (skip+startIndexOffset) % colorCount;
				gradientLevelItem = startIndexOffset;
				
				startTime += intervalTime;
			}

			return effectIntents;
		}

		private void RenderElement(Pulse.Pulse pulse, GradientLevelPair gradientLevelPair, TimeSpan startTime,
			ElementNode element, EffectIntents effectIntents)
		{
			pulse.ColorGradient = gradientLevelPair.ColorGradient;
			pulse.LevelCurve = gradientLevelPair.Curve;
			pulse.TargetNodes = new[] { element };
			
			var result = pulse.Render();

			result.OffsetAllCommandsByTime(startTime);
			effectIntents.Add(result);
		}
	}

}