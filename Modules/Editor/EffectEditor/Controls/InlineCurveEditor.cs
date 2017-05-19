using System;
using System.Windows;
using NLog;
using VixenModules.App.Curves;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class InlineCurveEditor : BaseInlineCurveEditor
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private static readonly Type ThisType = typeof(InlineCurveEditor);
		
		static InlineCurveEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#region Dependency Property Fields

		/// <summary>
		/// Identifies the <see cref="Value"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Curve),
			ThisType, new PropertyMetadata(new Curve(CurveType.Flat100), ValueChanged));

		
		#endregion Dependency Property Fields

		#region Properties

		/// <summary>
		/// Gets or sets the value. This is a dependency property.
		/// </summary>
		/// <value>The value.</value>
		public Curve Value
		{
			get { return (Curve)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		#endregion Properties

		#region Property Changed Callbacks

		private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlineCurveEditor = (InlineCurveEditor)d;
			if (!inlineCurveEditor.IsInitialized)
				return;
			if (e.NewValue == null)
			{
				Logging.Warn("Null Curve presented!");
				return;
			}
			
			inlineCurveEditor.OnCurveValueChanged();

		}

		protected override void SetCurveValue(Curve c)
		{
			if (c != null)
			{
				Value = c;
			}
		}

		protected override Curve GetCurveValue()
		{
			return Value;
		}

		#endregion Property Changed Callbacks

	}
}
