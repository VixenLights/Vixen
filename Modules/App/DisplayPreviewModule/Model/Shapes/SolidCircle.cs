namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidCircle : IShape
    {
        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidCircle;
            }
        }
    }
}