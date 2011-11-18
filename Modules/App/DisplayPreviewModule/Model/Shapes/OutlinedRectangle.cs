namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedRectangle : IShape
    {
        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.OutlinedRectangle;
            }
        }
    }
}
