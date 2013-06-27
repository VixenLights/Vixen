namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
	using System.ComponentModel;
	using System.Windows.Media;

	public interface IShape : INotifyPropertyChanged
	{
		string Name { get; }

		Brush Brush { get; set; }

		ShapeType ShapeType { get; }

		IShape Clone();
	}
}