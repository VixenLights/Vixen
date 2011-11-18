namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Windows.Media;

    [DataContract]
    internal class Line : IShape
    {
        private double _angle;
        private Color _nodeColor;
        private double _strokeThickness;

        public Line()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public double Angle
        {
            get
            {
                return _angle;
            }

            set
            {
                _angle = value;
                PropertyChanged.NotifyPropertyChanged("Angle", this);
            }
        }

        public string Name
        {
            get
            {
                return "Line";
            }
        }

        public Color NodeColor
        {
            get
            {
                return _nodeColor;
            }

            set
            {
                _nodeColor = value;
                PropertyChanged.NotifyPropertyChanged("NodeColor", this);
            }
        }

        public ShapeType ShapeType
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
                return _strokeThickness;
            }

            set
            {
                _strokeThickness = value;
                PropertyChanged.NotifyPropertyChanged("StrokeThickness", this);
            }
        }

        public IShape Clone()
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
