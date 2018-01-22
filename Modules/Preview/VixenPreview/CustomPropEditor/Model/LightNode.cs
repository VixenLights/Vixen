using System.ComponentModel;
using System.Windows;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Model
{
    public class LightNode : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private Point _center;
        private double _size;

        public LightNode(Point center, int size)
        {
            _center = center;
            _size = size;
        }

        public Point Center
        {
            get { return _center; }
            set
            {
                var changing = !value.Equals(_center);
                if (changing)
                {
                    OnPropertyChanging(new PropertyChangingEventArgs("Center"));
                    OnPropertyChanging(new PropertyChangingEventArgs("Top"));
                    OnPropertyChanging(new PropertyChangingEventArgs("Left"));
                }
                _center = value;
                if (changing)
                {
                    OnPropertyChanged(new PropertyChangedEventArgs("Center"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Top"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Left"));
                }


            }
        }

        public double Top
        {
            get { return Center.Y - Size/2; }
        }

        public double Left
        {
            get { return Center.X - Size/2; }
        }

        public double Size
        {
            get { return _size; }
            set
            {
                var changing = !value.Equals(_size);
                if (changing)
                {
                    OnPropertyChanging(new PropertyChangingEventArgs("Size")); 
                    OnPropertyChanging(new PropertyChangingEventArgs("Top"));
                    OnPropertyChanging(new PropertyChangingEventArgs("Left"));
                }
                _size = value;
                if (changing)
                {
                    OnPropertyChanged(new PropertyChangedEventArgs("Size"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Top"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Left"));
                }
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, e);
        }

        #endregion  
    }
}
