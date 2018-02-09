using System.Windows;
using System.Windows.Media.Media3D;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class LightNode : BindableBase
    {

        private double _size;
        private double _y;
        private double _x;
        private double _z;

        public LightNode(Point center, double size)
        {
            X = center.X;
            Y = center.Y;
            _size = size;
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (value.Equals(_y)) return;
                _y = value;
                OnPropertyChanged("Y");
            }
        }

        public double X
        {
            get { return _x; }
            set
            {
                if (value.Equals(_x)) return;
                _x = value;
                OnPropertyChanged("X");
            }
        }

        public double Z
        {
            get { return _z; }
            set
            {
                if (value.Equals(_z)) return;
                _z = value;
                OnPropertyChanged("Z");
            }
        }

        public double Size
        {
            get { return _size; }
            set
            {
	            if (!value.Equals(_size))
	            {
		            _size = value;
			        OnPropertyChanged("Size");
				}             
            }
        }

	}
}
