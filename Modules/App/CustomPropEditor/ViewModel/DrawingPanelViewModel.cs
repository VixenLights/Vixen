using System;
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

            AlignTopsCommand = new RelayCommand(AlignTops, CanExecuteAlignmentMethod);
            AlignBottomsCommand = new RelayCommand(AlignBottoms, CanExecuteAlignmentMethod);
            AlignLeftCommand = new RelayCommand(AlignLeft, CanExecuteAlignmentMethod);
            AlignRightCommand = new RelayCommand(AlignRight, CanExecuteAlignmentMethod);
            DistributeHorizontallyCommand = new RelayCommand(DistributeHorizontally, CanExecuteAlignmentMethod);
            DistributeVerticallyCommand = new RelayCommand(DistributeVertically, CanExecuteAlignmentMethod);
            
            SelectedItems = new ObservableCollection<LightViewModel>();

            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AlignLeftCommand.RaiseCanExecuteChanged();
            AlignRightCommand.RaiseCanExecuteChanged();
            AlignBottomsCommand.RaiseCanExecuteChanged();
            AlignTopsCommand.RaiseCanExecuteChanged();
            DistributeVerticallyCommand.RaiseCanExecuteChanged();
            DistributeHorizontallyCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteAlignmentMethod()
        {
            return SelectedItems.Any();
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

        public void AlignTops()
        {
            var ln = SelectedItems.First();
            foreach (var lightViewModel in SelectedItems)
            {
                lightViewModel.LightNode.Y = ln.Y;
            }
        }

        public void AlignBottoms()
        {
            var ln = SelectedItems.First();
            foreach (var lightViewModel in SelectedItems)
            {
                lightViewModel.LightNode.Y = ln.Y;
            }
        }

        public void AlignLeft()
        {
            var ln = SelectedItems.First();
            foreach (var lightViewModel in SelectedItems)
            {
                lightViewModel.LightNode.X = ln.X;
            }
        }

        public void AlignRight()
        {
            var ln = SelectedItems.First();
            foreach (var lightViewModel in SelectedItems)
            {
                lightViewModel.LightNode.X = ln.X;
            }
        }

        public void DistributeHorizontally()
        {
            if (SelectedItems.Count > 2)
            {
                var minX = SelectedItems.Min(x => x.LightNode.X);
                var maxX = SelectedItems.Max(x => x.LightNode.X);
                var count = SelectedItems.Count - 1;

                var dist = (maxX - minX) / count;

                int y = 0;
                double holdValue = minX;
                foreach (var lightViewModel in SelectedItems.OrderBy(x => x.X))
                {
                    if (y != 0)
                    {
                        holdValue += dist;
                        lightViewModel.X = holdValue;
                    }

                    y++;
                }

            }
           
        }

        public void DistributeVertically()
        {
            if (SelectedItems.Count > 2)
            {
                var minY = SelectedItems.Min(x => x.LightNode.Y);
                var maxY = SelectedItems.Max(x => x.LightNode.Y);
                var count = SelectedItems.Count - 1;

                var dist = (maxY - minY) / count;

                int y = 0;
                double holdValue = minY;
                foreach (var lightViewModel in SelectedItems.OrderBy(x => x.Y))
                {
                    if (y != 0)
                    {
                        holdValue += dist;
                        lightViewModel.Y = holdValue;
                    }

                    y++;
                }

            }
        }
        #region Commands

        public RelayCommand<Point> AddLightCommand { get; private set; }

        public RelayCommand<Transform> TransformCommand { get; private set; }

        #region Alignment Commands

        public RelayCommand AlignLeftCommand { get; private set; }
        public RelayCommand AlignRightCommand { get; private set; }
        public RelayCommand AlignTopsCommand { get; private set; }
        public RelayCommand AlignBottomsCommand { get; private set; }
        public RelayCommand DistributeHorizontallyCommand { get; private set; }
        public RelayCommand DistributeVerticallyCommand { get; private set; }


        #endregion

        #endregion


    }
}
