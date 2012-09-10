namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class SolidRectangle : Shape
    {
        public override string Name
        {
            get
            {
                return "Solid Rectangle";
            }
        }

        public override ShapeType ShapeType
        {
            get
            {
                return ShapeType.SolidRectangle;
            }
        }

        public override IShape Clone()
        {
            return new SolidRectangle();
        }
    }
}
