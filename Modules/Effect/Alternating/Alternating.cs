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
using VixenModules.Property.Color;
using VixenModules.App.Curves;
using ZedGraph;

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

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();

			foreach (ElementNode node in TargetNodes) {
				if (node != null)
					RenderNode(node);
			}
		}

		//Validate that the we are using valid colors and set appropriate defaults if not.
		private void CheckForInvalidColorData()
		{
			// check for sane default colors when first rendering it
			HashSet<Color> validColors = new HashSet<Color>();
			validColors.AddRange(TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
	
			//Validate Color 1
			if (validColors.Any() && 
				(!validColors.Contains(_data.Color1.ToArgb()) || !_data.ColorGradient1.GetColorsInGradient().IsSubsetOf(validColors)))
			{
				Color1 = validColors.First();
				ColorGradient1 = new ColorGradient(validColors.First());
			}

			//Validate color 2
			if (validColors.Any() &&
				(!validColors.Contains(_data.Color2.ToArgb()) || !_data.ColorGradient2.GetColorsInGradient().IsSubsetOf(validColors)))
			{
				if (validColors.Count > 1)
				{
					Color2 = validColors.ElementAt(1);
					ColorGradient2 = new ColorGradient(validColors.ElementAt(1));
				} else
				{
					Color2 = validColors.First();
					ColorGradient2 = new ColorGradient(validColors.First());
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
			get
			{
				return _data.Color1;
			}
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
			get
			{
				return _data.Color2;
			}
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
			get
			{
				CheckForInvalidColorData();
				return _data.ColorGradient1;
			}
			set
			{
				_data.ColorGradient1 = value;
				IsDirty = true;
			}
		}

		[Value]
		public ColorGradient ColorGradient2
		{
			get
			{
				CheckForInvalidColorData();
				return _data.ColorGradient2;
			}
			set
			{
				_data.ColorGradient2 = value;
				IsDirty = true;
			}
		}

		[Value]
		public Curve Curve1
		{
			get { return _data.Curve1; }
			set
			{
				_data.Curve1 = value;
				IsDirty = true;
			}
		}

		[Value]
		public Curve Curve2
		{
			get { return _data.Curve2; }
			set
			{
				_data.Curve2 = value;
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
			 
			if (Enable) {
				//intervals = Math.DivRem((long)TimeSpan.TotalMilliseconds, (long)Interval, out rem);
				intervals = Math.Ceiling(TimeSpan.TotalMilliseconds / (double)Interval);
			}

			TimeSpan startTime = TimeSpan.Zero;
		 
			for (int i = 0; i < intervals; i++) {
				altColor = startingColor;
				var intervalTime = intervals == 1
									? TimeSpan
									: TimeSpan.FromMilliseconds(Interval);

				LightingValue? lightingValue = null;

				int totalElements = node.Count();
				int currentNode = 0;

				var nodes = node.GetLeafEnumerator();

				while (currentNode < totalElements) {

					var elements = nodes.Skip(currentNode).Take(GroupEffect);

					currentNode += GroupEffect;

					int cNode = 0;
					elements.ToList().ForEach(element => {
						RenderElement(altColor, ref startTime, ref intervalTime, ref lightingValue, element);
						cNode++;
					});
					altColor = !altColor;
				}


				startTime += intervalTime;
				startingColor = !startingColor;
			}
		}

		private void RenderElement(bool altColor, ref TimeSpan startTime, ref System.TimeSpan intervalTime,
								   ref LightingValue? lightingValue, ElementNode element)
		{
			EffectIntents result;

			if ((StaticColor1 && altColor) || StaticColor2 && !altColor) {

				var level = new SetLevel.SetLevel();
				level.TargetNodes = new ElementNode[] { element };
				level.Color = altColor ? Color1 : Color2;
				level.TimeSpan = intervalTime;
				
				result = level.Render();

			} else {
				var pulse = new Pulse.Pulse();
				pulse.TargetNodes = new ElementNode[] { element };
				pulse.TimeSpan = intervalTime;
				pulse.ColorGradient = altColor ? ColorGradient1 : ColorGradient2;
				pulse.LevelCurve = altColor ? Curve1 : Curve2;
				result = pulse.Render();
			}

			result.OffsetAllCommandsByTime(startTime);
			_elementData.Add(result);
		}
	}
}