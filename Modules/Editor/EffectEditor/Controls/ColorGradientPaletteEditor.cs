using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VixenModules.Editor.EffectEditor.Input;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class ColorGradientPaletteEditor: ListView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorGradientPaletteEditor"/> class.
		/// </summary>
		public ColorGradientPaletteEditor()
		{
			CommandBindings.Add(new CommandBinding(PropertyEditorCommands.RemoveCollectionItem, OnRemoveCollectionItemCommand));
			//IsSynchronizedWithCurrentItem = false;
		}

		private void OnRemoveCollectionItemCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var value = e.Parameter as PropertyItemValue;
			if (value != null)
			{
				value.RemoveItemFromCollection(SelectedIndex);
			}
		}

		#region Fields

		private bool _wrappedEvents;

		#endregion

		#region PropertyValue property

		/// <summary>
		/// Identifies the <see cref="PropertyValue"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty PropertyValueProperty =
			DependencyProperty.Register("PropertyValue", typeof(PropertyItemValue), typeof(ColorGradientPaletteEditor),
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
			var editor = (ColorGradientPaletteEditor)sender;
			
			var newValue = e.NewValue as PropertyItemValue;
			if (newValue == null) return;

			editor.ItemsSource = newValue.CollectionValues;
			
		}

		#endregion

		
	}
}
