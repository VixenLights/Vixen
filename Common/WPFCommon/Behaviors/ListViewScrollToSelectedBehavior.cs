using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Common.WPFCommon.Behaviors
{
	public class ListViewScrollToSelectedBehavior : Behavior<ListView>
	{
		protected override void OnAttached()
		{
			AssociatedObject.AddHandler(ListView.SelectionChangedEvent,
				new RoutedEventHandler(OnSelectionChanged));
		}

		private void OnSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (sender is ListView lv)
			{
				lv.ScrollIntoView(lv.SelectedItem);
				lv.Focus();
			}
		}

		protected override void OnDetaching()
		{
			AssociatedObject.RemoveHandler(ListView.SelectionChangedEvent,
				new RoutedEventHandler(OnSelectionChanged));
		}

		
	}
}
