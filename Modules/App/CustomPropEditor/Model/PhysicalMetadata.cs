using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class PhysicalMetadata: BindableBase
    {
	    private string _height;
	    private string _width;
	    private string _material;
	    private string _bulbType;
	    private ColorMode _colorMode;
	    private string _depth;
	    private string _nodeCount;

	    public string Height
	    {
		    get { return _height; }
		    set
		    {
			    if (value == _height) return;
			    _height = value;
			    OnPropertyChanged(nameof(Height));
		    }
	    }

	    public string Width
	    {
		    get { return _width; }
		    set
		    {
			    if (value == _width) return;
			    _width = value;
			    OnPropertyChanged(nameof(Width));
		    }
	    }

	    public string Depth
	    {
		    get { return _depth; }
		    set
		    {
			    if (value == _depth) return;
			    _depth = value;
			    OnPropertyChanged(nameof(Depth));
		    }
	    }

	    public string Material
	    {
		    get { return _material; }
		    set
		    {
			    if (value == _material) return;
			    _material = value;
			    OnPropertyChanged(nameof(Material));
		    }
	    }

	    public string NodeCount
	    {
		    get => _nodeCount;
		    set
		    {
			    if (value == _nodeCount) return;
			    _nodeCount = value;
			    OnPropertyChanged(nameof(NodeCount));
		    }
	    }

	    public string BulbType
	    {
		    get { return _bulbType; }
		    set
		    {
			    if (value == _bulbType) return;
			    _bulbType = value;
			    OnPropertyChanged(nameof(BulbType));
		    }
	    }

	    public ColorMode ColorMode
	    {
		    get { return _colorMode; }
		    set
		    {
			    if (value == _colorMode) return;
			    _colorMode = value;
			    OnPropertyChanged(nameof(ColorMode));
		    }
	    }
    }
}
