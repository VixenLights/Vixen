namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    internal class OutlinedStar : IShape
    {
        private double _strokeThickness;

        public OutlinedStar()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Outlined Star";
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.OutlinedStar;
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
            return new OutlinedStar { StrokeThickness = StrokeThickness };
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
