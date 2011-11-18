namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidStar : IShape
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Solid Star";
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidStar;
            }
        }

        public IShape Clone()
        {
            return new SolidStar();
        }
    }
}
