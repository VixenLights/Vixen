namespace VixenModules.App.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Windows.Media;

    [DataContract]
    internal class SolidCircle : IShape
    {
        private Color _nodeColor;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return "Solid Circle";
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
                return ShapeType.SolidCircle;
            }
        }

        public IShape Clone()
        {
            return new SolidCircle();
        }
    }
}
