using System.ComponentModel;
using System.Drawing;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Model
{
    public class LightNode : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private Point _location;
        private int _size;

        public LightNode(Point location, int size)
        {
            _location = location;
            _size = size;
        }

        public Point Location
        {
            get { return _location; }
            set
            {
                var changing = value.Equals(_location);
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Location"));
                _location = value;
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Location"));
                
            }
        }

        public int Size
        {
            get { return _size; }
            set
            {
                var changing = value.Equals(_size);
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Location"));
                _size = value;
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Location"));
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
