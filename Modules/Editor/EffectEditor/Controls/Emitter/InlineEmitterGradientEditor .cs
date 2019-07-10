using System;
using System.Windows;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class InlineEmitterGradientEditor:BaseInlineGradientEditor
	{
		#region Static Fields

		private static readonly Type ThisType = typeof(InlineEmitterGradientEditor);

		#endregion

		#region Static Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		static InlineEmitterGradientEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion

		#region Dependency Property Fields

		/// <summary>
		/// Identifies the <see cref="Value"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(IEmitter),
			ThisType, new PropertyMetadata(null, ValueChanged));

		#endregion Dependency Property Fields

		#region Properties

		/// <summary>
		/// Gets or sets the value. This is a dependency property.
		/// </summary>
		/// <value>The value.</value>
		public IEmitter Value
		{
			get { return (IEmitter)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		#endregion Properties

		#region Property Changed Callbacks

		private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
		{
			var inlineGradientEditor = (InlineEmitterGradientEditor)d;
			if (!inlineGradientEditor.IsInitialized)
				return;
			inlineGradientEditor.Value = (IEmitter)e.NewValue;
			inlineGradientEditor.OnGradientValueChanged();
		}

		#endregion Property Changed Callbacks

		#region Protected Methods

		protected override ColorGradient GetColorGradientValue()
		{
			return Value.Color;
		}

		protected override void SetColorGradientValue(ColorGradient cg)
		{
			if (cg != null)
			{
				IEmitter newEmitter = Value.CreateInstanceForClone();
				newEmitter.Color = cg;
				Value = newEmitter;
			}
		}

		#endregion
	}
}
