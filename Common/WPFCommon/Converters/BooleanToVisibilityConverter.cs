using System.Windows;

namespace Common.WPFCommon.Converters
{
	/// <summary>
	/// A flexible boolean to visibility converter that allows it to be configured as to
	/// what values true and false map to.
	/// </summary>
	public sealed class BooleanToVisibilityConverter:BooleanConverter<Visibility>
	{
		public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed)
		{
			
		}
	}
}
