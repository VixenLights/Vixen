using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common.WPFCommon.Command;
using Common.WPFCommon.ViewModel;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModel
{
    public class DrawingPanelViewModel : BindableBase
    {
        private Prop _prop;
        private ObservableCollection<LightViewModel> _lightNodes;
        private ObservableCollection<LightViewModel> _selectedItems;
        private bool _isDrawing;
        private bool _isSelected;
        private double _width;
        private double _height;
        private double _x;
        private double _y;
        

        public DrawingPanelViewModel():this(new Prop())
        {
            
        }

        public DrawingPanelViewModel(Prop p)
        {
            Prop = Prop;
            Width = 30;
            Height = 30;
            X = 20;
            Y = 20;
            AddLightCommand = new RelayCommand<Point>(AddLightAt);
            TransformCommand = new RelayCommand<Transform>(Transform);
            SelectedItems = new ObservableCollection<LightViewModel>();
        }

        
        #region Properties

        public Prop Prop
        {
            get { return _prop; }
            set
            {
                if (Equals(value, _prop)) return;
                _prop = value;
                OnPropertyChanged("Prop");
                InitializeModel();
            }
        }

        public ObservableCollection<LightViewModel> SelectedItems { get; set; }

        public ObservableCollection<LightViewModel> LightNodes
        {
            get { return _lightNodes; }
            set
            {
                if (Equals(value, _lightNodes)) return;
                _lightNodes = value;
                OnPropertyChanged("LightNodes");
            }
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

        public double Width
        {
            get { return _width; }
            set
            {
                if (value.Equals(_width)) return;
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (value.Equals(_height)) return;
                _height = value;
                OnPropertyChanged("Height");
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


        public bool IsDrawing
        {
            get { return _isDrawing; }
            set
            {
                if (value == _isDrawing) return;
                _isDrawing = value;
                IsSelected = !value;
                OnPropertyChanged("IsDrawing");
            }
        }

        #endregion


        private void InitializeModel()
        {
            LightNodes = new ObservableCollection<LightViewModel>(Prop.GetLeafNodes().SelectMany(x => x.Lights)
                .Select(x => new LightViewModel(x)));

            
            //ElementCandidateViewModels = new ObservableCollection<ElementCandidateViewModel>(Prop.ElementCandidates.Select(x => new ElementCandidateViewModel(x)));
        }

        public void AddLightAt(Point p)
        {
            var ec = new ElementCandidate("New One");
            var ln = ec.AddLight(p, 3);
            Prop.AddElementCandidate(ec);
            LightNodes.Add(new LightViewModel(ln));
        }

        public void Transform(Transform t)
        {
            foreach (LightViewModel lvm in SelectedItems)
            {
                if (lvm.IsSelected)
                {
                    lvm.Center = t.Transform(lvm.Center);
                    //lvm.X += delta.X;
                    //lvm.Y += delta.Y;
                }
            }
        }



        #region Commands

        public RelayCommand<Point> AddLightCommand { get; private set; }

        public RelayCommand<Transform> TransformCommand { get; private set; }

        #endregion


    }
}
