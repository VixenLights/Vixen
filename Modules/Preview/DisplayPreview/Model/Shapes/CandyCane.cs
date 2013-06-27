namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
	using System.Runtime.Serialization;

	[DataContract]
	internal class CandyCane : Shape
	{
		public override string Name
		{
			get { return "Candy Cane"; }
		}

		public override ShapeType ShapeType
		{
			get { return ShapeType.CandyCane; }
		}

		public override IShape Clone()
		{
			return new CandyCane();
		}
	}
}