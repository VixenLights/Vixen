using System.Windows;

namespace VixenModules.Editor.PolygonEditor.Views
{
	/// <summary>
	/// This class assists with binding the Point data grid to <c>SelectedPointsReadOnly</c>.
	/// </summary>
	/// <remarks>This class is from the following post:
	/// https://thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
	/// </remarks>
	public class BindingProxy : Freezable
	{
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
		
		public object Data
		{
			get
			{
				return (object)GetValue(DataProperty);
			}
			set
			{
				SetValue(DataProperty, value);
			}
		}
		protected override Freezable CreateInstanceCore()
		{
			return new BindingProxy();
		}
	}
}
