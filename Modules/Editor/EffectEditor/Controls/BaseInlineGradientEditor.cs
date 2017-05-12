using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common.Controls.ColorManagement.ColorPicker;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.Editor.EffectEditor.Converters;
using VixenModules.Effect.Effect;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public abstract class BaseInlineGradientEditor: Control
	{

		private static readonly Type ThisType = typeof(BaseInlineGradientEditor);
		private static readonly ColorGradientToImageConverter Converter = new ColorGradientToImageConverter();
		private bool _isDiscrete;
		protected static ColorPicker.Mode Mode = ColorPicker.Mode.HSV_RGB;
		protected static ColorPicker.Fader Fader = ColorPicker.Fader.HSV_H;
		protected Canvas _canvas;
		protected readonly List<SliderPoint> _points;

		static BaseInlineGradientEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		protected BaseInlineGradientEditor()
		{
			_points = new List<SliderPoint>();
			Loaded += InlineGradientEditor_Loaded;
			InitializeStyle();
		}

		private void InlineGradientEditor_Loaded(object sender, RoutedEventArgs e)
		{
			_canvas = (Canvas)Template.FindName("FaderCanvas", this);
			_canvas.MouseMove += _canvas_MouseMove; ;
			//_canvas.MouseDown += CanvasOnMouseDown;
			//_canvas.MouseLeftButtonDown += CanvasOnMouseLeftButtonDown;
			OnComponentChanged();
			//ReloadPoints();
			//UpdateImage(Value);
		}

		private void _canvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsMouseOverAnyHandle())
			{
				Cursor = Cursors.SizeWE;
			}
			else
			{
				Cursor = Cursors.Arrow;
			}

		}

		#region Dependency Property Fields

		/// <summary>
		/// Identifies the <see cref="Component"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ComponentProperty = DependencyProperty.Register("Component", typeof(Object),
			ThisType, new PropertyMetadata(null, OnComponentChanged));

		public static readonly DependencyProperty SliderStyleProperty = DependencyProperty.Register("SliderStyle", typeof(Style), ThisType);

		#endregion Dependency Property Fields

		#region Properties

		public Style SliderStyle
		{
			get { return (Style)GetValue(SliderStyleProperty); }
			set { SetValue(SliderStyleProperty, value); }
		}

		public Object Component
		{
			get { return (Object)GetValue(ComponentProperty); }
			set { SetValue(ComponentProperty, value); }
		}

		protected bool IsDiscrete
		{
			get { return _isDiscrete; }
			set
			{
				_isDiscrete = value;
				UpdateImage(GetColorGradientValue());
			}
		}

		public HashSet<Color> ValidColors { get; set; }


		#endregion

		#region Property Changed Callbacks

		private static void OnComponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlineGradientEditor = (BaseInlineGradientEditor)d;
			if (!inlineGradientEditor.IsInitialized)
				return;
			inlineGradientEditor.OnComponentChanged();
		}

		protected virtual void OnComponentChanged()
		{
			if (Component is BaseEffect)
			{
				var effect = (BaseEffect)Component;
				IsDiscrete = effect.HasDiscreteColors;
				if (IsDiscrete)
				{
					ValidColors = effect.GetValidColors();
				}
			}
			else if (Component is Array)
			{
				var effects = (BaseEffect[])Component;
				IsDiscrete = effects.Any(e => e.HasDiscreteColors);
				if (IsDiscrete)
				{
					ValidColors.Clear();
					foreach (var baseEffect in effects)
					{
						ValidColors.AddRange(baseEffect.GetValidColors());
					}
				}
			}
		}

		#endregion

		private void InitializeStyle()
		{
			SliderStyle = new Style();
			var brush = new LinearGradientBrush(Colors.LightGray, System.Windows.Media.Color.FromRgb((byte)(Colors.LightGray.R * 0.5), (byte)(Colors.LightGray.G * 0.5), (byte)(Colors.LightGray.B * 0.5)), 0);
			SliderStyle.Setters.Add(new Setter(Shape.FillProperty, brush));
			brush = new LinearGradientBrush(Colors.Gray, System.Windows.Media.Color.FromRgb((byte)(Colors.Gray.R * 0.5), (byte)(Colors.Gray.G * 0.5), (byte)(Colors.Gray.B * 0.5)), 0);
			var trigger = new Trigger { Setters = { new Setter(Shape.FillProperty, brush) }, Value = true, Property = IsFocusedProperty };
			SliderStyle.Triggers.Add(trigger);
		}

		protected bool IsMouseOverAnyHandle()
		{
			return _points.Any(q => q.IsMouseOver);
		}

		protected abstract ColorGradient GetColorGradientValue();

		protected void UpdateImage(ColorGradient colorGradient)
		{
			if (IsInitialized)
			{
				var template = Template;
				var imageControl = (Image)template.FindName("GradientImage", this);
				if (imageControl != null)
				{
					imageControl.Source = (BitmapImage)Converter.Convert(colorGradient, null, IsDiscrete, null);
				}
			}
		}

	}
}
