namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidRectangle : IShape
    {
        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidRectangle;
            }
        }
    }
}