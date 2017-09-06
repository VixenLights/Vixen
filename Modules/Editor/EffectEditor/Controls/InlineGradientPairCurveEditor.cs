using System;
using System.Windows;
using System.Windows.Input;
using NLog;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class InlineGradientPairCurveEditor : BaseInlineCurveEditor
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private static readonly Type ThisType = typeof(InlineGradientPairCurveEditor);
		
		static InlineGradientPairCurveEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}
		
		
		#region Dependency Property Fields

		/// <summary>
		/// Identifies the <see cref="Value"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(GradientLevelPair),
			ThisType, new PropertyMetadata(new GradientLevelPair(), ValueChanged));

		
		#endregion Dependency Property Fields

		#region Properties

		/// <summary>
		/// Gets or sets the value. This is a dependency property.
		/// </summary>
		/// <value>The value.</value>
		public GradientLevelPair Value
		{
			get { return (GradientLevelPair)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		
		#endregion Properties

		#region Property Changed Callbacks

		private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlineCurveEditor = (InlineGradientPairCurveEditor)d;
			if (!inlineCurveEditor.IsInitialized)
				return;
			if (e.NewValue == null)
			{
				Logging.Warn("Null Gradient Pair presented!");
				return;
			}
			inlineCurveEditor.OnCurveValueChanged();
			
		}

		protected override void SetCurveValue(Curve c)
		{
			if (c != null)
			{
				Value = new GradientLevelPair(Value.ColorGradient, c);
			}
		}

		protected override Curve GetCurveValue()
		{
			if (Value != null)
			{
				return Value.Curve;
			}
			else
			{
				return null;
			}
		}

		#endregion Property Changed Callbacks

		
		
	}
}
