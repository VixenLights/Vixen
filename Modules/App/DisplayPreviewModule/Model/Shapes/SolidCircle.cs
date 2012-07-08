namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidCircle : Shape
    {
        public override string Name
        {
            get
            {
                return "Solid Circle";
            }
        }

        public override ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidCircle;
            }
        }

        public override IShape Clone()
        {
            return new SolidCircle();
        }
    }
}
