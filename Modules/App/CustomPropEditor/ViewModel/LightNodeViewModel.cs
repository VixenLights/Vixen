using Common.WPFCommon.ViewModel;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModel
{
    public class LightNodeViewModel: BindableBase
    {
        private bool _isSelected;
        private ElementCandidate _elementCandidate;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public ElementCandidate ElementCandidate
        {
            get { return _elementCandidate; }
            set
            {
                if (Equals(value, _elementCandidate)) return;
                _elementCandidate = value;
                OnPropertyChanged("ElementCandidate");
            }
        }
    }
}
