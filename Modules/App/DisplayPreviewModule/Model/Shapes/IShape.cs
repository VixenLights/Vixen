namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;

    public interface IShape : INotifyPropertyChanged
    {
        ShapeType ShapeType { get; }

        string Name { get; }

        IShape Clone();
    }
}