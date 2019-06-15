using System;
using System.Collections.Generic;
using System.Windows;
using NLog;
using VixenModules.App.ColorGradients;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class InlineGradientEditor : BaseInlineGradientEditor
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private static readonly Type ThisType = typeof(InlineGradientEditor);
		
		static InlineGradientEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#region Dependency Property Fields

		/// <summary>
		/// Identifies the <see cref="Value"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ColorGradient),
			ThisType, new PropertyMetadata(new ColorGradient(), ValueChanged));

		#endregion Dependency Property Fields

		#region Properties

		/// <summary>
		/// Gets or sets the value. This is a dependency property.
		/// </summary>
		/// <value>The value.</value>
		public ColorGradient Value
		{
			get { return (ColorGradient)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		#endregion Properties

		#region Property Changed Callbacks

		private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlineGradientEditor = (InlineGradientEditor)d;
			if (!inlineGradientEditor.IsInitialized)
				return;
			if (e.NewValue == null)
			{
				Logging.Warn("Null gradient presented!");
				return;
			}
			//if (e.NewValue.Equals(e.OldValue))
			//{
			//	Logging.Warn("Same gradient in value changed.");
			//}
			inlineGradientEditor.OnGradientValueChanged();
		}

		#endregion Property Changed Callbacks
		
		protected override ColorGradient GetColorGradientValue()
		{
			return Value;
		}

		protected override void SetColorGradientValue(ColorGradient cg)
		{
			if (cg != null)
			{
				Value = cg;
			}
		}
	}
}
