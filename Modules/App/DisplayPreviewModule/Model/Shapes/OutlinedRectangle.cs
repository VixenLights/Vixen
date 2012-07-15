namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedRectangle : Shape
    {
        private double _strokeThickness;

        public OutlinedRectangle()
        {
            Initialize();
        }

        public override string Name
        {
            get
            {
                return "Outlined Rectangle";
            }
        }

        public override ShapeType ShapeType
        {
            get
            {
                return ShapeType.OutlinedRectangle;
            }
        }

        [DataMember]
        public double StrokeThickness
        {
            get
            {
                if (_strokeThickness <= 0)
                {
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
            return new OutlinedRectangle { StrokeThickness = StrokeThickness };
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
