using NLog;
using System;
using System.Windows;
using VixenModules.App.Curves;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class InlineEmitterLifetimeCurveEditor : BaseInlineCurveEditor
	{
		#region Private Static Fields

		private static readonly Type ThisType = typeof(InlineEmitterLifetimeCurveEditor);

		#endregion

		#region Constructor

		static InlineEmitterLifetimeCurveEditor()
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
			var inlineCurveEditor = (InlineEmitterLifetimeCurveEditor)d;
			if (!inlineCurveEditor.IsInitialized)
				return;
			if (e.NewValue == null)
			{				
				return;
			}
			inlineCurveEditor.OnCurveValueChanged();
			
		}

		protected override void SetCurveValue(Curve c)
		{
			if (c != null)
			{
				IEmitter newEmitter = Value.CreateInstanceForClone();
				newEmitter.Lifetime = c;

				Value = newEmitter;
			}
		}

		protected override Curve GetCurveValue()
		{
			if (Value != null)
			{
				return Value.Lifetime;
			}
			else
			{
				return null;
			}
		}

		#endregion Property Changed Callbacks				
	}	
}
