namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedCircle : IShape, INotifyPropertyChanged
    {
        private double _strokeThickness;

        public OutlinedCircle()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Outlined Circle";
            }
        }

        public ShapeType ShapeType
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
                PropertyChanged.NotifyPropertyChanged("StrokeThickness", this);
            }
        }

        public IShape Clone()
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
