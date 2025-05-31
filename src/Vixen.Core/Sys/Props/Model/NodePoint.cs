#nullable enable
using Vixen.Model;

namespace Vixen.Sys.Props.Model
{
    /// <summary>
    /// Coordinates for light nodes
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public class NodePoint(double x, double y, double z = 0):BindableBase
    {
        private ElementNode? _node;
        private int _size = 2;
        private double _x = x;
        private double _y = y;
        private double _z = z;

		public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        public double Z
        {
            get => _z;
            set => SetProperty(ref _z, value);
        }

        public ElementNode? Node
        {
            get => _node;
            set => SetProperty(ref _node, value);
        }

        public int Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }
    }
}