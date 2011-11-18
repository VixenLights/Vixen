namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Windows.Media;

    public interface IShape : INotifyPropertyChanged
    {
        ShapeType ShapeType { get; }

        string Name { get; }

        IShape Clone();

        Color NodeColor { get; set; }
    }
}