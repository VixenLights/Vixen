namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidTriangle : Shape
    {
        public override string Name
        {
            get
            {
                return "Solid Triangle";
            }
        }

        public override ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidTriangle;
            }
        }

        public override IShape Clone()
        {
            return new SolidTriangle();
        }
    }
}
