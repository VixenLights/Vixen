using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using System.Threading.Tasks;
using VixenModules.App.ColorGradients;

namespace VixenModules.Effect.Alternating
{
	public class Alternating : EffectModuleInstanceBase
	{
		private AlternatingData _data;
		private EffectIntents _elementData = null;

		public Alternating()
		{
			_data = new AlternatingData();
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
			set { _data = value as AlternatingData; }
		}

		[Value]
		public double IntensityLevel1
		{
			get { return _data.Level1; }
			set
			{
				_data.Level1 = value;
				IsDirty = true;
			}
		}

		[Value]
		public Color Color1
		{
			get { return _data.Color1; }
			set
			{
				_data.Color1 = value;
				IsDirty = true;
			}
		}

		[Value]
		public double IntensityLevel2
		{
			get { return _data.Level2; }
			set
			{
				_data.Level2 = value;
				IsDirty = true;
			}
		}

		[Value]
		public Color Color2
		{
			get { return _data.Color2; }
			set
			{
				_data.Color2 = value;
				IsDirty = true;
			}
		}

		[Value]
		public int Interval
		{
			get { return _data.Interval; }
			set
			{
				_data.Interval = value;
				IsDirty = true;
			}
		}

		[Value]
		public int DepthOfEffect
		{
			get { return _data.DepthOfEffect; }
			set
			{
				_data.DepthOfEffect = value;
				IsDirty = true;
			}
		}

		[Value]
		public int GroupEffect
		{
			get { return _data.GroupEffect; }
			set
			{
				_data.GroupEffect = value;
				IsDirty = true;
			}
		}

		[Value]
		public bool Enable
		{
			get { return _data.Enable; }
			set
			{
				_data.Enable = value;
				IsDirty = true;
			}
		}

		[Value]
		public bool StaticColor1
		{
			get { return _data.StaticColor1; }
			set
			{
				_data.StaticColor1 = value;
				IsDirty = true;
			}
		}

		[Value]
		public bool StaticColor2
		{
			get { return _data.StaticColor2; }
			set
			{
				_data.StaticColor2 = value;
				IsDirty = true;
			}
		}

		[Value]
		public ColorGradient ColorGradient1
		{
			get { return _data.ColorGradient1; }
			set
			{
				_data.ColorGradient1 = value;
				IsDirty = true;
			}
		}

		[Value]
		public ColorGradient ColorGradient2
		{
			get { return _data.ColorGradient2; }
			set
			{
				_data.ColorGradient2 = value;
				IsDirty = true;
			}
		}


		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			bool altColor = false;
			bool startingColor = false;
			double intervals = 1;
			long rem = 0;

			if (Enable) {
				//intervals = Math.DivRem((long)TimeSpan.TotalMilliseconds, (long)Interval, out rem);
				intervals = Math.Ceiling(TimeSpan.TotalMilliseconds/(double) Interval);
			}

			TimeSpan startTime = TimeSpan.Zero;

			for (int i = 0; i < intervals; i++) {
				altColor = startingColor;
				var intervalTime = intervals == 1
				                   	? TimeSpan
				                   	: TimeSpan.FromMilliseconds(i == intervals - 1 ? Interval + rem : Interval);

				LightingValue? lightingValue = null;
				double[] gradient1Points = GetDataPoints(ColorGradient1).ToArray();
				double[] gradient2Points = GetDataPoints(ColorGradient2).ToArray();
				double groupLastPosition = 0;
				double groupPosition = 0;
				int totalElements = node.Count();
				int currentNode = 0;
				while (currentNode < totalElements) {
					var elements = node.Skip(currentNode).Take(GroupEffect);
					currentNode += GroupEffect;
					int cNode = 0;
					elements.ToList().ForEach(element =>
					                          	{
					                          		double lastPosition = groupLastPosition;
					                          		double position = groupPosition;

					                          		RenderElement(altColor, ref startTime, ref intervalTime, ref lightingValue,
					                          		              gradient1Points, gradient2Points, element, ref lastPosition, ref position);
					                          		if (cNode == 0) groupLastPosition = lastPosition;
					                          		if (cNode == 0) groupPosition = position;
					                          		cNode++;
					                          	});
					altColor = !altColor;
				}


				startTime += intervalTime;
				startingColor = !startingColor;
			}
		}

		private void RenderElement(bool altColor, ref TimeSpan startTime, ref System.TimeSpan intervalTime,
		                           ref LightingValue? lightingValue, double[] gradient1Points, double[] gradient2Points,
		                           Element element, ref double lastPosition, ref double position)
		{
			if ((StaticColor1 && altColor) || StaticColor2 && !altColor) {
				lightingValue = new LightingValue(altColor ? Color1 : Color2,
				                                  altColor ? (float) IntensityLevel1 : (float) IntensityLevel2);
				IIntent intent = new LightingIntent(lightingValue.Value, lightingValue.Value, intervalTime);
				_elementData.AddIntentForElement(element.Id, intent, startTime);
			}
			else {
				var x =
					TimeSpan.FromMilliseconds(
						Math.Ceiling(intervalTime.TotalMilliseconds/(altColor ? gradient1Points.Length : gradient2Points.Length)));
				TimeSpan startTime2 = startTime;
				lastPosition = altColor ? gradient1Points[0] : gradient2Points[0];
				for (int j = 0; j < (altColor ? gradient1Points.Length : gradient2Points.Length); j++) {
					position = altColor ? gradient1Points[j] : gradient2Points[j];

					LightingValue startValue =
						new LightingValue(altColor ? ColorGradient1.GetColorAt(lastPosition) : ColorGradient2.GetColorAt(lastPosition),
						                  altColor ? (float) IntensityLevel1 : (float) IntensityLevel2);
					LightingValue endValue =
						new LightingValue(altColor ? ColorGradient1.GetColorAt(position) : ColorGradient2.GetColorAt(position),
						                  altColor ? (float) IntensityLevel1 : (float) IntensityLevel2);

					IIntent intent = new LightingIntent(startValue, endValue, x);

					_elementData.AddIntentForElement(element.Id, intent, startTime2);

					lastPosition = position;
					startTime2 += x;
				}
			}
		}

		private IEnumerable<double> GetDataPoints(ColorGradient gradient)
		{
			HashSet<double> allPoints = new HashSet<double>();

			allPoints.Add(0.0);


			foreach (ColorPoint point in gradient.Colors) {
				allPoints.Add(point.Position);
			}

			allPoints.Add(1.0);

			return allPoints.OrderBy(x => x);
		}
	}
}