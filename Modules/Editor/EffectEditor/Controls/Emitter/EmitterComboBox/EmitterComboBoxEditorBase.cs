using System;
using System.Windows;
using System.Windows.Controls;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterComboBoxEditorBase : ComboBox		
	{		
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterComboBoxEditorBase);

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(IEmitter),
			ThisType, new PropertyMetadata(null, ValueDepChanged));

		public static readonly DependencyProperty EmitterValueProperty = DependencyProperty.Register("EmitterValue", typeof(IEmitter),
			ThisType, new PropertyMetadata(null));

		#endregion

		#region Constructor

		public EmitterComboBoxEditorBase()
		{
			SelectionChanged += EmitterDropDownClosed;
		}

		#endregion

		#region Private Static Methods
	
		private static void ValueDepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			EmitterComboBoxEditorBase comboBoxEditor = (EmitterComboBoxEditorBase)d;
			
			if (e.NewValue == null)
			{
				return;
			}

			// Get the emitter that is part of the binding
			IEmitter originalEmitter = (IEmitter)e.NewValue;

			// Make a copy of the emitter
			comboBoxEditor.EmitterValue = originalEmitter.CreateInstanceForClone();

			// Give the original emitter a reference to the emitter that the ComboBox is bound to
			originalEmitter.InEdit = comboBoxEditor.EmitterValue;
		}
		
		#endregion

		#region Protected Methods

		protected virtual void EmitterDropDownClosed(object sender, EventArgs e)
		{
			if (Value != null)
			{
				SelectionChangedEventArgs selectionChanged = (SelectionChangedEventArgs)e;
				
				if (selectionChanged.AddedItems.Count == 1 && selectionChanged.RemovedItems.Count == 1)
				{
					// Set the emitter back on the main effect
					// This triggers the dirty state and the undo/redo logic
					Value = EmitterValue.CreateInstanceForClone();
				}
			}
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
