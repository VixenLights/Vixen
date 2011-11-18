namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidRectangle : IShape
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Solid Rectangle";
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidRectangle;
            }
        }

        public IShape Clone()
        {
            return new SolidRectangle();
        }
    }
}
