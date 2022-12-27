using System.Windows;

namespace Common.WPFCommon.Input
{
	public class DragDropTargetType: DependencyObject
	{
		public static readonly DependencyProperty TargetTypeProperty = DependencyProperty.RegisterAttached(
			  "TargetType", typeof(string), typeof(DragDropTargetType), new PropertyMetadata(""));

		public static string GetTargetType(DependencyObject d)
		{
			return (string)d.GetValue(TargetTypeProperty);
		}
		public static void SetTargetType(DependencyObject d, string value)
		{
			d.SetValue(TargetTypeProperty, value);
		}

		
	}
}
