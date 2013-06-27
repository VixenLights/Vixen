namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
	using System.Runtime.Serialization;

	[DataContract]
	internal class OutlinedTriangle : Shape
	{
		private double _strokeThickness;

		public OutlinedTriangle()
		{
			Initialize();
		}

		public override string Name
		{
			get { return "Outlined Triangle"; }
		}

		public override ShapeType ShapeType
		{
			get { return ShapeType.OutlinedTriangle; }
		}

		[DataMember]
		public double StrokeThickness
		{
			get
			{
				if (_strokeThickness <= 0) {
					_strokeThickness = 5;
				}

				return _strokeThickness;
			}

			set
			{
				_strokeThickness = value;
				NotifyPropertyChanged("StrokeThickness");
			}
		}

		public override IShape Clone()
		{
			return new OutlinedTriangle {StrokeThickness = StrokeThickness};
		}

		private void Initialize()
		{
			_strokeThickness = 5;
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			Initialize();
		}
	}
}