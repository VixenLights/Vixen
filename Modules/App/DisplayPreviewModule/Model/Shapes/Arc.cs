namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    internal class Arc : IShape
    {
        private double _strokeThickness;

        public Arc()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Arc";
            }
        }

        public ShapeType ShapeType
        {
            get
            {
                return ShapeType.Arc;
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
            return new Arc();
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
