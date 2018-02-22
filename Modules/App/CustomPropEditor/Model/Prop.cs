using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class Prop : BindableBase
    {
        private BitmapSource _image;
        private string _name;
        private ElementModel _rootNode;
        private double _height;
        private double _width;
        private double _opacity;

        public Prop(string name) : this()
        {
            Name = name;
        }

        public Prop()
        {
            _rootNode = new ElementModel();
            Image = CreateBitmapSource(800, 600, Color.FromRgb(0,0,0));
            Opacity = 1;
            Name = "Default";
        }

        public ElementModel RootNode
        {
            get { return _rootNode; }
            private set
            {
                if (Equals(value, _rootNode)) return;
                _rootNode = value;
                OnPropertyChanged(nameof(RootNode));
            }
        }


        public void AddElementModel(ElementModel ec)
        {
            _rootNode.Children.Add(ec);
        }

        public void AddElementModels(IEnumerable<ElementModel> elementCandidates)
        {
            foreach (var elementCandidate in elementCandidates)
            {
                _rootNode.Children.Add(elementCandidate);
            }
        }

        public bool RemoveFromParent(ElementModel child, ElementModel parent)
        {
            parent.RemoveChild(child);
            return child.RemoveParent(parent);
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                _rootNode.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public BitmapSource Image
        {
            get { return _image; }
            set
            {
                if (value != null && !value.Equals(_image))
                {
                    _image = value;
                    OnPropertyChanged(nameof(Image));
                    Height = _image.Height;
                    Width = _image.Width;
                }
            }
        }

        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (value.Equals(_opacity)) return;
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (value == _height) return;
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public IEnumerable<ElementModel> GetLeafNodes()
        {
            // Don't want to return the root node.
            // note: this may very well return duplicate nodes, if they are part of different groups.
            return _rootNode.Children.SelectMany(x => x.GetLeafEnumerator());
        }

        public IEnumerable<ElementModel> GetAll()
        {
            var list = _rootNode.GetChildEnumerator().ToList();
            list.Add(RootNode);
            return list;
        }

        #region Utilities

        private BitmapSource CreateBitmapSource(int width, int height, Color color)
        {
            int stride = width / 8;
            byte[] pixels = new byte[height * stride];

            List<Color> colors = new List<Color>();
            colors.Add(color);
            BitmapPalette myPalette = new BitmapPalette(colors);

            BitmapSource image = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Indexed1,
                myPalette,
                pixels,
                stride);

            return image;
        }

        #endregion
    }
}
