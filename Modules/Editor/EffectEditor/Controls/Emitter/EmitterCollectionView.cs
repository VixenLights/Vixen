using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VixenModules.Editor.EffectEditor.Input;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterCollectionView: ListView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EmitterCollectionView"/> class.
		/// </summary>
		public EmitterCollectionView()
		{
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.RemoveCollectionItem, OnRemoveCollectionItemCommand, CanExecuteDelete));
			LostFocus += CollectionView_LostFocus;			
		}

		private void CanExecuteDelete(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
		{
			if (canExecuteRoutedEventArgs.Parameter != null)
			{
				if (canExecuteRoutedEventArgs.Parameter.Equals(PropertyValue))
				{
					// Don't allow the user to delete the last emitter
					canExecuteRoutedEventArgs.CanExecute = Items.Count > 1 && SelectedItems.Count > 0;
					canExecuteRoutedEventArgs.Handled = true;
				}
			}
			else
			{
				canExecuteRoutedEventArgs.CanExecute = false;
				canExecuteRoutedEventArgs.Handled = true;
			}
		}

		private void CollectionView_LostFocus(object sender, RoutedEventArgs e)
		{
			if (SelectedItems.Count > 0)
			{
				SelectedIndex = -1;
			}
			
		}

		private void OnRemoveCollectionItemCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var value = e.Parameter as PropertyItemValue;
			if (value != null && SelectedIndex >=0)
			{
				if (Items.Count > 1)
				{
					value.RemoveItemFromCollection(SelectedIndex);
				}
			}
			
		}

		#region PropertyValue property

		/// <summary>
		/// Identifies the <see cref="PropertyValue"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty PropertyValueProperty =
			DependencyProperty.Register("PropertyValue", typeof(IEmitter), typeof(EmitterCollectionView),
				new PropertyMetadata(null, OnPropertyValueChanged));

		/// <summary>
		/// Gets or sets the property value. This is a dependency property.
		/// </summary>
		/// <value>The property value.</value>
		public IEmitter PropertyValue
		{
			get { return (IEmitter)GetValue(PropertyValueProperty); }
			set { SetValue(PropertyValueProperty, value); }
		}

		private static void OnPropertyValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			EmitterCollectionView editor = (EmitterCollectionView)sender;
			
			var newValue = e.NewValue as IEmitter;
			if (newValue == null) return;
		}

		#endregion

	}
}
