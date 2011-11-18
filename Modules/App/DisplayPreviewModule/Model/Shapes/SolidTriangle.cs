namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidTriangle : IShape
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Solid Triangle";
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidTriangle;
            }
        }

        public IShape Clone()
        {
            return new SolidTriangle();
        }
    }
}
