using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
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

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();
			foreach (ElementNode node in TargetNodes) {
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
		public ColorGradient ColorGradient
		{
			get { return _data.ColorGradient; }
			set
			{
				_data.ColorGradient = value;
				IsDirty = true;
			}
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			foreach (ElementNode elementNode in node.GetLeafEnumerator()) {
				// this is probably always going to be a single element for the given node, as
				// we have iterated down to leaf nodes in RenderNode() above. May as well do
				// it this way, though, in case something changes in future.
				if (elementNode == null)
					continue;

				ElementColorType colorType = ColorModule.getColorTypeForElementNode(elementNode);

				if (colorType == ElementColorType.FullColor) {
					addIntentsToElement(elementNode.Element);
				}
				else {
					IEnumerable<Color> colors = ColorModule.getValidColorsForElementNode(elementNode);
					foreach (Color color in colors) {
						addIntentsToElement(elementNode.Element, color);
					}
				}
			}
		}

		private void addIntentsToElement(Element element, Color? color = null)
		{
			double[] allPointsTimeOrdered = _GetAllSignificantDataPoints().ToArray();
			Debug.Assert(allPointsTimeOrdered.Length > 1);

			double lastPosition = allPointsTimeOrdered[0];
			for (int i = 1; i < allPointsTimeOrdered.Length; i++) {
				double position = allPointsTimeOrdered[i];

				LightingValue startValue;
				LightingValue endValue;
				if (color == null) {
					startValue = new LightingValue(ColorGradient.GetColorAt(lastPosition),
					                               (float) LevelCurve.GetValue(lastPosition*100)/100);
					endValue = new LightingValue(ColorGradient.GetColorAt(position), (float) LevelCurve.GetValue(position*100)/100);
				}
				else {
					startValue = new LightingValue((Color) color,
					                               (float)
					                               (ColorGradient.GetProportionOfColorAt(lastPosition, (Color) color)*
					                                LevelCurve.GetValue(lastPosition*100)/100));
					endValue = new LightingValue((Color) color,
					                             (float)
					                             (ColorGradient.GetProportionOfColorAt(position, (Color) color)*
					                              LevelCurve.GetValue(position*100)/100));
				}

				TimeSpan startTime = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds*lastPosition);
				TimeSpan timeSpan = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds*(position - lastPosition));

				IIntent intent = new LightingIntent(startValue, endValue, timeSpan);

				_elementData.AddIntentForElement(element.Id, intent, startTime);

				lastPosition = position;
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
					if (lastPointPos != point.Position) {
						lastDistinctPos = lastPointPos;
					}

					if (addNextPointAsFadeOut && point.Position != lastPointPos) {
						points.Add(point.Position);
						addNextPointAsFadeOut = false;
					}

					// if this current point is the same color, it is significant; add it, as well as
					// the points before & after (to get the color fade in and out)
					if (point.Color.ToRGB().ToArgb().ToArgb() == ((Color) color).ToArgb()) {
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