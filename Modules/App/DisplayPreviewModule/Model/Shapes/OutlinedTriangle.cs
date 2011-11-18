namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedTriangle : IShape
    {
        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.OutlinedTriangle;
            }
        }
    }
}
