namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedStar : IShape
    {
        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.OutlinedStar;
            }
        }
    }
}
