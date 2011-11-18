namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedTriangle : IShape
    {
        private double _strokeThickness;

        public OutlinedTriangle()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Outlined Triangle";
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.OutlinedTriangle;
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
            return new OutlinedTriangle { StrokeThickness = StrokeThickness };
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
