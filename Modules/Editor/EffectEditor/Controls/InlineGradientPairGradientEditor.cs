using System;
using System.Windows;
using VixenModules.App.ColorGradients;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class InlineGradientPairGradientEditor:BaseInlineGradientEditor
	{

		private static readonly Type ThisType = typeof(InlineGradientPairGradientEditor);

		static InlineGradientPairGradientEditor()
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
			var inlineGradientEditor = (InlineGradientPairGradientEditor)d;
			if (!inlineGradientEditor.IsInitialized)
				return;
			//inlineGradientEditor.Value = (GradientLevelPair)e.NewValue;
			inlineGradientEditor.OnGradientValueChanged();
		}

		#endregion Property Changed Callbacks


		protected override ColorGradient GetColorGradientValue()
		{
			return Value.ColorGradient;
		}

		protected override void SetColorGradientValue(ColorGradient cg)
		{
			if (cg != null)
			{
				Value = new GradientLevelPair(cg, Value.Curve);
			}
		}
	}
}
