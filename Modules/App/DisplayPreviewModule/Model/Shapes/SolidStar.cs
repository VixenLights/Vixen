namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidStar : IShape
    {
        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidStar;
            }
        }
    }
}