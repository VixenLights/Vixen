using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VixenModules.Editor.EffectEditor.Input;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class CollectionView: ListView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionView"/> class.
		/// </summary>
		public CollectionView()
		{
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.RemoveCollectionItem, OnRemoveCollectionItemCommand, CanExecuteDelete));
			LostFocus += CollectionView_LostFocus;
			//IsSynchronizedWithCurrentItem = false;
		}

		private void CanExecuteDelete(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
		{
			if (canExecuteRoutedEventArgs.Parameter != null)
			{
				if (canExecuteRoutedEventArgs.Parameter.Equals(PropertyValue))
				{
					var value = canExecuteRoutedEventArgs.Parameter as PropertyItemValue;
					canExecuteRoutedEventArgs.CanExecute = value.CanRemove() && SelectedItems.Count > 0;
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
				if (value.CanRemove())
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
			DependencyProperty.Register("PropertyValue", typeof(PropertyItemValue), typeof(CollectionView),
				new PropertyMetadata(null, OnPropertyValueChanged));

		/// <summary>
		/// Gets or sets the property value. This is a dependency property.
		/// </summary>
		/// <value>The property value.</value>
		public PropertyItemValue PropertyValue
		{
			get { return (PropertyItemValue)GetValue(PropertyValueProperty); }
			set { SetValue(PropertyValueProperty, value); }
		}

		private static void OnPropertyValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var editor = (CollectionView)sender;
			
			var newValue = e.NewValue as PropertyItemValue;
			if (newValue == null) return;

			editor.ItemsSource = newValue.CollectionValues;
		}

		#endregion

	}
}
