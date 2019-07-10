using System;
using System.Windows;
using System.Windows.Controls;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public abstract class EmitterCheckBoxEditorBase : CheckBox		
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterCheckBoxEditorBase);

		#endregion

		#region Dependency Property 

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(IEmitter),
			ThisType, new PropertyMetadata(null, ValueDepChanged));

		public static readonly DependencyProperty EmitterValueProperty = DependencyProperty.Register("EmitterValue", typeof(IEmitter),
			ThisType, new PropertyMetadata(null, null));

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
			set { SetValue(EmitterValueProperty, value); }
		}

		#endregion

		#region Fields

		bool _updatingBusinessObject = false;

		#endregion

		#region Constructor

		public EmitterCheckBoxEditorBase()
		{
			Checked += CheckBoxEditor_Checked;
			Unchecked += CheckBoxEditor_Unchecked;
		}

		#endregion

		#region Private Methods

		private void CheckBoxEditor_Unchecked(object sender, RoutedEventArgs e)
		{
			UpdateValue();
			
			_updatingBusinessObject = false;			
		}

		private void CheckBoxEditor_Checked(object sender, RoutedEventArgs e)
		{
			if (!_updatingBusinessObject)
			{
				UpdateValue();
			}
			else
			{
				_updatingBusinessObject = false;
			}
		}

		private void UpdateValue()
		{
			Value = EmitterValue.CreateInstanceForClone();
		}

		private void UpdateEmitterValue()
		{
			EmitterValue = Value.CreateInstanceForClone();
		}

		private static void ValueDepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var checkBoxEditor = (EmitterCheckBoxEditorBase)d;
			
			if (e.NewValue == null)
			{
				return;
			}

			IEmitter original = (IEmitter)e.NewValue;
			if (checkBoxEditor.GetIEmitterProperty(original))
			{
				checkBoxEditor._updatingBusinessObject = true;
			}

			checkBoxEditor.UpdateEmitterValue();
		}

		#endregion

		#region Protected Methods

		protected abstract bool GetIEmitterProperty(IEmitter emitter);

		#endregion
	}
}
