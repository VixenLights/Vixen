namespace VixenModules.Preview.DisplayPreview.Model.Shapes
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Windows.Media;

    [DataContract]
    public abstract class Shape : IShape
    {
        private Brush _brush;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract string Name { get; }

        public Brush Brush
        {
            get
            {
                return this._brush;
            }

            set
            {
                this._brush = value;
                NotifyPropertyChanged("Brush");
            }
        }

        public abstract ShapeType ShapeType { get; }

        public abstract IShape Clone();

        protected void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged.NotifyPropertyChanged(propertyName, this);
        }

		protected void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
    }
}
