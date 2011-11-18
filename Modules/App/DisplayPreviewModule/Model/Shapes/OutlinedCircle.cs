namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedCircle : IShape
    {
        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.OutlinedCircle;
            }
        }
    }
}
