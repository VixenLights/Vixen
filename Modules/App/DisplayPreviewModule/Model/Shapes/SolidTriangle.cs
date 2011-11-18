namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidTriangle : IShape
    {
        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidTriangle;
            }
        }
    }
}