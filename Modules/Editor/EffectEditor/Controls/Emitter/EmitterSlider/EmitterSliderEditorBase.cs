using System;
using System.Windows;
using System.Windows.Controls;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterSliderEditorBase : Slider
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterSliderEditorBase);

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(IEmitter),
			ThisType, new PropertyMetadata(null, ValueDepChanged));

		public static readonly DependencyProperty EmitterValueProperty = DependencyProperty.Register("EmitterValue", typeof(IEmitter),
			ThisType, new PropertyMetadata(null));

		#endregion
		
		#region Private Methods

		private static void ValueDepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			EmitterSliderEditorBase comboBoxEditor = (EmitterSliderEditorBase)d;

			if (e.NewValue == null)
			{
				return;
			}

			IEmitter originalEmitter = (IEmitter)e.NewValue;
			comboBoxEditor.EmitterValue = originalEmitter.CreateInstanceForClone();			
		}

		#endregion

		#region Protected Methods

		protected override void OnThumbDragCompleted(System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			Value = EmitterValue.CreateInstanceForClone();						
		}

		#endregion

		#region Public Properties

		public IEmitter Value
		{
			get { return (IEmitter)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public IEmitter EmitterValue
		{
			get { return (IEmitter)GetValue(EmitterValueProperty); }
			set
			{
				SetValue(EmitterValueProperty, value);
			}
		}

		#endregion Properties
	}
}
