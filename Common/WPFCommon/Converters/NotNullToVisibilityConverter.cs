using System.Windows;

namespace Common.WPFCommon.Converters
{
	public class NotNullToVisibilityConverter:NullConverter<Visibility>
	{
		public NotNullToVisibilityConverter():base(Visibility.Collapsed, Visibility.Visible)
		{
			
		}
	}
}
