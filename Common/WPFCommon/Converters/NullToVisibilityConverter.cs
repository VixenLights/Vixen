using System.Windows;

namespace Common.WPFCommon.Converters
{
	public class NullToVisibilityConverter : NullConverter<Visibility>
	{
		public NullToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed)
		{

		}
	}
}
