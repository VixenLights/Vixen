using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.EffectEditor.EffectTypeEditors;
using VixenModules.Property.Color;
using ZedGraph;

namespace VixenModules.Effect.Pulse
{
	public class Pulse : EffectModuleInstanceBase
	{
		private PulseData _data;
		private EffectIntents _elementData = null;

		public Pulse()
		{
			_data = new PulseData();
		}

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();
			
			foreach (ElementNode node in TargetNodes) {
				if (tokenSource != null && tokenSource.IsCancellationRequested)
					return;

				if (node != null)
					RenderNode(node);
			}
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as PulseData; }
		}

		[Value]
		[Category(@"Effect Brightness")]
		[EditorAttribute(typeof(EffectCurveTypeEditor), typeof(UITypeEditor))]
		[DisplayName(@"Brightness Curve")]
		[Description(@"Controls the brightness of the effect.")]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
			}
		}

		[Value]
		[Category(@"Effect Color")]
		[EditorAttribute(typeof(EffectColorGradientTypeEditor), typeof(UITypeEditor))]
		[DisplayName(@"Color")]
		[Description(@"Controls the color gradient of the effect.")]
		public ColorGradient ColorGradient
		{
			get
			{
				return _data.ColorGradient;
			}
			set
			{
				_data.ColorGradient = value;
				IsDirty = true;
			}
		}

		public override bool IsDirty
		{
			get
			{
				if (!LevelCurve.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				if (!ColorGradient.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		private void CheckForInvalidColorData()
		{
			HashSet<Color> validColors = new HashSet<Color>();
			validColors.AddRange(TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
			if (validColors.Any() && !_data.ColorGradient.GetColorsInGradient().IsSubsetOf(validColors))
			{
				//Our color is not valid for any elements we have.
				//Try to set a default color gradient from our available colors
				_data.ColorGradient = new ColorGradient(validColors.First());
			}
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			//Collect all the points first.
			double[] allPointsTimeOrdered = _GetAllSignificantDataPoints().ToArray();
			foreach (ElementNode elementNode in node.GetLeafEnumerator()) {
				// this is probably always going to be a single element for the given node, as
				// we have iterated down to leaf nodes in RenderNode() above. May as well do
				// it this way, though, in case something changes in future.
				if (elementNode == null || elementNode.Element == null)
					continue;

				ElementColorType colorType = ColorModule.getColorTypeForElementNode(elementNode);

				if (colorType == ElementColorType.FullColor) {
					addIntentsToElement(elementNode.Element, allPointsTimeOrdered);
				}
				else {
					IEnumerable<Color> colors = ColorModule.getValidColorsForElementNode(elementNode, false)
						 .Intersect(ColorGradient.GetColorsInGradient());
					foreach (Color color in colors) {
						addIntentsToElement(elementNode.Element, allPointsTimeOrdered, color);	
					}
				}
			}
		}

		private void addIntentsToElement(Element element, double[] allPointsTimeOrdered, Color? color = null)
		{
			if (element != null)
			{
				double lastPosition = allPointsTimeOrdered[0];
				TimeSpan lastEnd = TimeSpan.Zero;
				for (var i = 1; i < allPointsTimeOrdered.Length; i++)
				{
					double position = allPointsTimeOrdered[i];

					LightingValue startValue;
					LightingValue endValue;
					if (color == null)
					{
						startValue = new LightingValue(ColorGradient.GetColorAt(lastPosition),
													   LevelCurve.GetValue(lastPosition * 100) / 100);
						endValue = new LightingValue(ColorGradient.GetColorAt(position), LevelCurve.GetValue(position * 100) / 100);
					}
					else
					{
						startValue = new LightingValue((Color)color,
													   (ColorGradient.GetProportionOfColorAt(lastPosition, (Color)color) *
														LevelCurve.GetValue(lastPosition * 100) / 100));
						endValue = new LightingValue((Color)color,
													 (ColorGradient.GetProportionOfColorAt(position, (Color)color) *
													  LevelCurve.GetValue(position * 100) / 100));
					}

					TimeSpan startTime = lastEnd;
					TimeSpan timeSpan = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds * (position - lastPosition));
					if (startValue.Intensity.Equals(0f) && endValue.Intensity.Equals(0f))
					{
						lastPosition = position;
						lastEnd = startTime + timeSpan;
						continue;
					}

					IIntent intent = new LightingIntent(startValue, endValue, timeSpan);

					_elementData.AddIntentForElement(element.Id, intent, startTime);

					lastPosition = position;
					lastEnd = startTime + timeSpan;
				}
			}
		}

		private IEnumerable<double> _GetAllSignificantDataPoints(Color? color = null)
		{
			HashSet<double> points = new HashSet<double>();

			points.Add(0.0);

			foreach (PointPair point in LevelCurve.Points) {
				points.Add(point.X/100);
			}
			double lastPointPos = 0.0;
			double lastDistinctPos = 0.0;
			bool addNextPointAsFadeOut = false;
			foreach (ColorPoint point in ColorGradient.Colors.SortedArray()) {
				if (color != null) {
					if (!lastPointPos.Equals( point.Position)) {
						lastDistinctPos = lastPointPos;
					}

					if (addNextPointAsFadeOut && !point.Position.Equals( lastPointPos)) {
						points.Add(point.Position);
						addNextPointAsFadeOut = false;
					}

					// if this current point is the same color, it is significant; add it, as well as
					// the points before & after (to get the color fade in and out)
					if (point.Color.ToRGB().ToArgb().ToArgb() == ((Color)color).ToArgb()) {
						points.Add(point.Position);
						points.Add(lastDistinctPos);
						addNextPointAsFadeOut = true;
					}

					lastPointPos = point.Position;
				}
				else {
					points.Add(point.Position);
				}
			}

			points.Add(1.0);

			return points.OrderBy(x => x);
		}
	}
}