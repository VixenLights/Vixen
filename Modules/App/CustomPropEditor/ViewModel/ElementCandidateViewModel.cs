using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Common.WPFCommon.ViewModel;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModel
{
    public class ElementCandidateViewModel:BindableBase
    {
        private bool _isSelected;
        private ElementCandidate _elementCandidate;
        private ObservableCollection<LightViewModel> _lightNodeViewModel;

        public ElementCandidateViewModel(ElementCandidate ec)
        {
            ElementCandidate = ec;
        }

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
                LightNodeViewModel = new ObservableCollection<LightViewModel>(_elementCandidate.Lights.Select(x => new LightViewModel(x)));
                OnPropertyChanged("ElementCandidate");
            }
        }

        public ObservableCollection<LightViewModel> LightNodeViewModel
        {
            get { return _lightNodeViewModel; }
            set
            {
                if (Equals(value, _lightNodeViewModel)) return;
                _lightNodeViewModel = value;
                OnPropertyChanged("LightNodeViewModel");
            }
        }

        public void AddLight(Point center, int size)
        {
            var node = ElementCandidate.AddLight(center, size);
            LightNodeViewModel.Add(new LightViewModel(node));
        }
    }
}
