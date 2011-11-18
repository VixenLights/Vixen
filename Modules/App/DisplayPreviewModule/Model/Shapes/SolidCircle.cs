namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidCircle : IShape
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Solid Circle";
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidCircle;
            }
        }

        public IShape Clone()
        {
            return new SolidCircle();
        }
    }
}
