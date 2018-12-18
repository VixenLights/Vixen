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
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;

namespace VixenModules.Effect.Strobe
{
	public class Strobe : BaseEffect
	{
		private EffectIntents _elementData;
		private StrobeData _data;
		private IEnumerable<IMark> _marks = null;
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

		#region Config

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Max Interval")]
		[ProviderDescription(@"MaxInterval")]
		[NumberRange(0, 10000, 1, 0)]
		[PropertyOrder(1)]
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
		[ProviderDisplayName(@"Cycle Interval")]
		[ProviderDescription(@"CycleInterval")]
		[PropertyOrder(2)]
		public Curve IntervalCurve
		{
			get { return _data.IntervalCurve; }
			set
			{
				_data.IntervalCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"On Percentage")]
		[ProviderDescription(@"On Percentage")]
		[PropertyOrder(3)]
		public Curve CycleRatioCurve
		{
			get { return _data.CycleRatioCurve; }
			set
			{
				_data.CycleRatioCurve = value;
				IsDirty = true;
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/strobe/"; }
		}

		#endregion

		#region Attributes

		private void InitAllAttributes()
		{
			TypeDescriptor.Refresh(this);
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
			EffectIntents effectIntents = new EffectIntents();
			var startTime = TimeSpan.Zero;
			double intervalPos = GetEffectTimeIntervalPosition(startTime) * 100;
			double interval = CalculateInterval(intervalPos);
			double cycleRatio = CycleRatioCurve.GetValue(intervalPos) / 100;
			TimeSpan intervalTime = TimeSpan.FromMilliseconds(interval);
			TimeSpan onTime = TimeSpan.FromMilliseconds(interval * cycleRatio);

			do
			{
				foreach (ElementNode element in elements)
				{
					var glp = new GradientLevelPair(Colors, IntensityCurve);
					RenderElement(glp, startTime, onTime.Subtract(TimeSpan.FromMilliseconds(1)), element,
						effectIntents);
				}

				startTime = startTime + intervalTime;
				intervalPos = GetEffectTimeIntervalPosition(startTime) * 100;
				interval = CalculateInterval(intervalPos);
				cycleRatio = CycleRatioCurve.GetValue(intervalPos) / 100;
				onTime = TimeSpan.FromMilliseconds(interval * cycleRatio);
				intervalTime = TimeSpan.FromMilliseconds(interval);
			} while (startTime.TotalMilliseconds < TimeSpan.TotalMilliseconds);

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
			double value = (int)ScaleCurveToValue(IntervalCurve.GetValue(intervalPosFactor), Interval, 1);
			if (value < 1) value = 1;
			return value;
		}
	}

}