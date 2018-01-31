using System.Windows;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class LightNode : BindableBase
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
	            if (!value.Equals(_center))
	            {
		            _center = value;
		            
			        OnPropertyChanged("Center");
			        OnPropertyChanged("Top");
			        OnPropertyChanged("Left");
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
	            if (!value.Equals(_size))
	            {
		            _size = value;
		            
			        OnPropertyChanged("Size");
			        OnPropertyChanged("Top");
			        OnPropertyChanged("Left");
				}
                
                
            }
        }

	}
}
