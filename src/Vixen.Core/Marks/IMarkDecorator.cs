using System.ComponentModel;

namespace Vixen.Marks
{
	public interface IMarkDecorator: INotifyPropertyChanged, ICloneable
	{
		bool IsBold { get; set; }
		bool IsSolidLine { get; set; }
		Color Color { get; set; }
	}
}
