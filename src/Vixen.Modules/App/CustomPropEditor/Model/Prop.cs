using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common.WPFCommon.ViewModel;
using LiteDB;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class Prop : BindableBase
    {
        private BitmapSource _image;
        private ElementModel _rootNode;
        private double _height;
        private double _width;
        private double _opacity;
	    private DateTime _creationDate;
	    private DateTime _modifiedDate;
	    private string _createdBy;
	    private string _type;

	    public Prop(string name) : this(name, 800, 600)
        {
            Name = name;
        }

        public Prop():this("New Prop", 800, 600)
        {
			
        }

        public Prop(string name, int x, int y)
        {
	        Id = Guid.NewGuid();
	        RootNode = new ElementModel();
			Image = CreateBitmapSource(x, y, Color.FromRgb(0, 0, 0));
			Opacity = 1;
	        Name = name;
	        CreationDate = DateTime.Now;
	        ModifiedDate = CreationDate;
	        CreatedBy = Environment.UserName;
	        VendorMetadata = new VendorMetadata();
	        PhysicalMetadata = new PhysicalMetadata();
	        InformationMetadata = new InformationMetadata();
		}

	    public Guid Id { get; private set; }

        public ElementModel RootNode
        {
            get { return _rootNode; }
            private set
            {
                if (Equals(value, _rootNode)) return;
	            if (_rootNode != null)
	            {
		            _rootNode.PropertyChanged -= RootNode_PropertyChanged;
	            }
                _rootNode = value;
				_rootNode.PropertyChanged += RootNode_PropertyChanged;
                OnPropertyChanged(nameof(RootNode));
            }
        }

		private void RootNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("Name"))
			{
				OnPropertyChanged(nameof(Name));
			}
		}

		public string Name
        {
            get { return _rootNode.Name; }
            set
            {
                if (value == _rootNode.Name) return;
				_rootNode.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

		public string Type
	    {
		    get { return _type; }
		    set
		    {
			    if (value == _type) return;
			    _type = value;
			    OnPropertyChanged(nameof(Type));
		    }
	    }
		
	    public string CreatedBy
	    {
		    get { return _createdBy; }
		    set
		    {
			    if (value == _createdBy) return;
			    _createdBy = value;
			    OnPropertyChanged(nameof(CreatedBy));
		    }
	    }

	    public DateTime CreationDate
		{
		    get { return _creationDate; }
		    private set
		    {
			    if (value.Equals(_creationDate)) return;
			    _creationDate = value;
			    OnPropertyChanged(nameof(CreationDate));
		    }
	    }

	    public DateTime ModifiedDate
	    {
		    get { return _modifiedDate; }
		    set
		    {
			    if (value.Equals(_modifiedDate)) return;
			    _modifiedDate = value;
			    OnPropertyChanged(nameof(ModifiedDate));
		    }
	    }

	    public VendorMetadata VendorMetadata { get; set; }

	    public PhysicalMetadata PhysicalMetadata { get; set; }

	    public InformationMetadata InformationMetadata { get; set; }

	    
        /// <summary>
        /// Gets or sets the source bitmap displayed on the logical prop canvas.
        /// </summary>
        /// <value>The source bitmap. Assigning a bitmap initializes invalid <see cref="Width" /> and <see cref="Height" /> values from its native pixel dimensions.</value>
        [BsonIgnore]
        public BitmapSource Image
        {
            get { return _image; }
            set
            {
                if (value != null && !value.Equals(_image))
                {
                    _image = value;
                    OnPropertyChanged(nameof(Image));
                    if (!AreValidCanvasDimensions())
                    {
                        Height = _image.PixelHeight;
                        Width = _image.PixelWidth;
                    }
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

        /// <summary>
        /// Gets or sets the logical height of the prop editor canvas.
        /// </summary>
        /// <value>The logical canvas height in editor pixels. A valid value is preserved when a persisted source bitmap is attached.</value>
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

        /// <summary>
        /// Gets or sets the logical width of the prop editor canvas.
        /// </summary>
        /// <value>The logical canvas width in editor pixels. A valid value is preserved when a persisted source bitmap is attached.</value>
        public double Width
        {
            get { return _width; }
            set
            {
	            if (value == _width) return;
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
	        return _rootNode.GetNodeEnumerator().ToList();
	    }

		internal void Hydrate(Guid id, ElementModel rootNode, BitmapSource image, string type, string createdBy,
			DateTime creationDate, DateTime modifiedDate, double opacity, double width, double height,
			VendorMetadata vendorMetadata, PhysicalMetadata physicalMetadata, InformationMetadata informationMetadata)
		{
			if (rootNode == null) throw new ArgumentNullException(nameof(rootNode));
			if (image == null) throw new ArgumentNullException(nameof(image));

			Id = id;
			RootNode = rootNode;
			Type = type;
			CreatedBy = createdBy;
			CreationDate = creationDate;
			ModifiedDate = modifiedDate;
			Opacity = opacity;
			Width = width;
			Height = height;
			Image = image;
			VendorMetadata = vendorMetadata ?? new VendorMetadata();
			PhysicalMetadata = physicalMetadata ?? new PhysicalMetadata();
			InformationMetadata = informationMetadata ?? new InformationMetadata();
		}

        #region Utilities

        private bool AreValidCanvasDimensions()
        {
            return IsValidCanvasDimension(Width) && IsValidCanvasDimension(Height);
        }

        private static bool IsValidCanvasDimension(double value)
        {
            return double.IsFinite(value) && value is >= 1 and <= 100000;
        }

        private BitmapSource CreateBitmapSource(int width, int height, Color color)
        {
            //int stride = width / 8;
            int stride = ((width * PixelFormats.Indexed1.BitsPerPixel + 7) / 8 + 3) & ~3;
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
