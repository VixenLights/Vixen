namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
    using System.Runtime.Serialization;

    [DataContract]
    internal class Line : Shape
    {
        private double _angle;
        private double _strokeThickness;

        public Line()
        {
            Initialize();
        }

        [DataMember]
        public double Angle
        {
            get
            {
                if (_strokeThickness <= 0)
                {
                    _strokeThickness = 5;
                }

                return _angle;
            }

            set
            {
                this._angle = value > 360 || value < 0 ? 0 : value;
                NotifyPropertyChanged("Angle");
            }
        }

        public override string Name
        {
            get
            {
                return "Line";
            }
        }

        public override ShapeType ShapeType
        {
            get
            {
                return ShapeType.Line;
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
            return new Line { Angle = Angle, StrokeThickness = StrokeThickness };
        }

        private void Initialize()
        {
            _strokeThickness = 5;
            _angle = 0;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }
    }
}
