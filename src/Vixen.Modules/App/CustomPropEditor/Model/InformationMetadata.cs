using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class InformationMetadata:BindableBase
    {
	    private string _notes;

	    public string Notes
	    {
		    get { return _notes; }
		    set
		    {
			    if (value == _notes) return;
			    _notes = value;
			    OnPropertyChanged(nameof(Notes));
		    }
	    }
    }
}
