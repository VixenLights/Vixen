namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedCircle : Shape
    {
        private double _strokeThickness;

        public OutlinedCircle()
        {
            Initialize();
        }

        public override string Name
        {
            get
            {
                return "Outlined Circle";
            }
        }

        public override ShapeType ShapeType
        {
            get
            {
                return ShapeType.OutlinedCircle;
            }
        }

        public double StrokeThickness
        {
            get
            {
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
            return new OutlinedCircle { StrokeThickness = StrokeThickness };
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
