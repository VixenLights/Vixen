using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.EffectEditor.EffectTypeEditors;
using VixenModules.EffectEditor.TypeConverters;
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
			InitPropertyDescriptors();
		}

		private void InitPropertyDescriptors()
		{
			UpdateAllAttributes();
			TypeDescriptor.Refresh(this);
		}

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken = null)
		{
			_elementData = new EffectIntents();
			
			var targetNodes = TargetNodes.AsParallel();
			
			if (cancellationToken != null)
				targetNodes = targetNodes.WithCancellation(cancellationToken.Token);
			
			targetNodes.ForAll(node => {
				if (node != null)
				RenderNode(node);
			});
	 
			 
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
			set
			{
				_data = value as AlternatingData;
				InitPropertyDescriptors();
			}
		}

		[Value]
		[Category(@"Effect Brightness")]
		[Editor(typeof(EffectLevelTypeEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(LevelTypeConverter))]
		[DisplayName(@"Color 1")]
		[Description(@"Controls the brightness of color 1.")]
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
		[Category(@"Effect Color")]
		[Editor(typeof(EffectColorTypeEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(ColorTypeConverter))]
		[DisplayName(@"Color 1")]
		[Description(@"Sets the first color.")]
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
		[Category(@"Effect Brightness")]
		[Editor(typeof(EffectLevelTypeEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(LevelTypeConverter))]
		[DisplayName(@"Color 2")]
		[Description(@"Controls the brightness of color 2.")]
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
		[Category(@"Effect Color")]
		[Editor(typeof(EffectColorTypeEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(ColorTypeConverter))]
		[DisplayName(@"Color 2")]
		[Description(@"Sets the second color.")]
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
		[Category(@"Effect Properties")]
		[Editor(typeof(EffectRangeTypeEditor), typeof(UITypeEditor))]
		//[TypeConverter(typeof(RangeTypeConverter))]
		[DisplayName(@"Alternate Interval")]
		[Description(@"Specifies how often the effect should switch in milliseconds.")]
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
		[Browsable(false)]
		public int DepthOfEffect //this property is not currently used
		{
			get { return _data.DepthOfEffect; }
			set
			{
				_data.DepthOfEffect = value;
				IsDirty = true;
			}
		}

		[Value]
		[Category(@"Effect Properties")]
		[DisplayName(@"Group Effect")]
		[Description(@"Indicates how many levels deep the effect should be grouped.")]
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
		[Category(@"Effect Properties")]
		[DisplayName(@"Make Display Static")]
		[Description(@"Indicates that the effect should be the same on all elements.")]
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
		[Category(@"Effect Color Type")]
		[DisplayName(@"Color 1")]
		[Description(@"Indicates the first color is a static color.")]
		[TypeConverter(typeof(ColorSelectionTypeConverter))]
		public bool StaticColor1
		{
			get { return _data.StaticColor1; }
			set
			{
				_data.StaticColor1 = value;
				IsDirty = true;
				UpdateColorOneAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		

		[Value]
		[Category(@"Effect Color Type")]
		[DisplayName(@"Color 2")]
		[Description(@"Indicates the second color is a static color.")]
		[TypeConverter(typeof(ColorSelectionTypeConverter))]
		public bool StaticColor2
		{
			get { return _data.StaticColor2; }
			set
			{
				_data.StaticColor2 = value;
				IsDirty = true;
				UpdateColorTwoAttributes();
				TypeDescriptor.Refresh(this);
			}
		}

		[Value]
		[Category(@"Effect Color")]
		[DisplayName(@"Color 1")]
		[Description(@"Sets the first color.")]
		public ColorGradient ColorGradient1
		{
			get
			{
				return _data.ColorGradient1;
			}
			set
			{
				_data.ColorGradient1 = value;
				IsDirty = true;
			}
		}

		[Value]
		[Category(@"Effect Color")]
		[DisplayName(@"Color 2")]
		[Description(@"Sets the second color.")]
		public ColorGradient ColorGradient2
		{
			get
			{
				return _data.ColorGradient2;
			}
			set
			{
				_data.ColorGradient2 = value;
				IsDirty = true;
			}
		}

		[Value]
		[Category(@"Effect Brightness")]
		[DisplayName(@"Color 1")]
		[Description(@"Controls the individual brightness curve of the first gradient.")]
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
		[Category(@"Effect Brightness")]
		[DisplayName(@"Color 2")]
		[Description(@"Controls the individual brightness curve of the second gradient.")]
		public Curve Curve2
		{
			get { return _data.Curve2; }
			set
			{
				_data.Curve2 = value;
				IsDirty = true;
			}
		}

		public override bool IsDirty
		{
			get
			{
				if (!Curve1.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				if (!Curve2.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				if (!ColorGradient1.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				if (!ColorGradient2.CheckLibraryReference())
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		#region Attributes

		private void UpdateAllAttributes()
		{
			UpdateColorOneAttributes();
			UpdateColorTwoAttributes();
		}


		private void UpdateColorOneAttributes()
		{
			SetBrowsable("Color1", StaticColor1);
			SetBrowsable("IntensityLevel1", StaticColor1);
			SetBrowsable("ColorGradient1", !StaticColor1);
			SetBrowsable("Curve1", !StaticColor1);
		}

		private void UpdateColorTwoAttributes()
		{
			SetBrowsable("Color2", StaticColor2);
			SetBrowsable("IntensityLevel2", StaticColor2);
			SetBrowsable("ColorGradient2", !StaticColor2);
			SetBrowsable("Curve2", !StaticColor2);
		}

		#endregion


		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			bool startingColor = false;
			double intervals = 1;
			 
			if (Enable) {
				//intervals = Math.DivRem((long)TimeSpan.TotalMilliseconds, (long)Interval, out rem);
				intervals = Math.Ceiling(TimeSpan.TotalMilliseconds / Interval);
			}

			TimeSpan startTime = TimeSpan.Zero;
		 
			for (int i = 0; i < intervals; i++) {
				bool altColor = startingColor;
				var intervalTime = intervals == 1
									? TimeSpan
									: TimeSpan.FromMilliseconds(Interval);

				int totalElements = node.Count();
				int currentNode = 0;

				var nodes = node.GetLeafEnumerator();

				while (currentNode < totalElements) {

					var elements = nodes.Skip(currentNode).Take(GroupEffect);

					currentNode += GroupEffect;

					foreach (var element in elements)
					{
						RenderElement(altColor, ref startTime, ref intervalTime, element);	
					}

					altColor = !altColor;
				}


				startTime += intervalTime;
				startingColor = !startingColor;
			}
		}

		private void RenderElement(bool altColor, ref TimeSpan startTime, ref TimeSpan intervalTime, ElementNode element)
		{
			EffectIntents result;

			if ((StaticColor1 && altColor) || StaticColor2 && !altColor) {

				var level = new SetLevel.SetLevel();
				level.TargetNodes = new[] { element };
				level.Color = altColor ? Color1 : Color2;
				level.TimeSpan = intervalTime;
				level.IntensityLevel = altColor ? IntensityLevel1 : IntensityLevel2;
				result = level.Render();

			} else {
				var pulse = new Pulse.Pulse();
				pulse.TargetNodes = new[] { element };
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