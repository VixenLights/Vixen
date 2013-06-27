namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
	using System.Runtime.Serialization;

	[DataContract]
	internal class SolidStar : Shape
	{
		public override string Name
		{
			get { return "Solid Star"; }
		}

		public override ShapeType ShapeType
		{
			get { return ShapeType.SolidStar; }
		}

		public override IShape Clone()
		{
			return new SolidStar();
		}
	}
}