using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Common.WPFCommon.Behaviors
{
	public class TreeViewSelectedItemBehavior : Behavior<TreeView>
	{
		#region SelectedItem Property

		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeViewSelectedItemBehavior), null);

		//private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		//{
		//	var item = e.NewValue as TreeViewItem;
		//	if (item != null)
		//	{
		//		item.SetValue(TreeViewItem.IsSelectedProperty, true);
		//	}
		//}

		#endregion

		protected override void OnAttached()
		{
			base.OnAttached();

			this.AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			if (this.AssociatedObject != null)
			{
				this.AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
			}
		}

		private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			this.SelectedItem = e.NewValue;
		}
	}
}
