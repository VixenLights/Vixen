using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Common.WPFCommon.Command;
using Common.WPFCommon.ViewModel;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModel
{
    public class DrawingPanelViewModel : ViewModelBase
    {
        private Prop _prop;
        private ObservableCollection<LightViewModel> _lightNodes;
        private bool _isDrawing;
        private bool _isSelected;
        private double _width;
        private double _height;
        private double _x;
        private double _y;
        private Dictionary<Guid, List<LightViewModel>> _elementModelMap;

        public DrawingPanelViewModel():this(new Prop())
        {
            
        }

        public DrawingPanelViewModel(Prop p)
        {
            _elementModelMap = new Dictionary<Guid, List<LightViewModel>>();
            LightNodes = new ObservableCollection<LightViewModel>();
            Prop = p;
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

        #region Prop model property

        /// <summary>
        /// Gets or sets the Prop value.
        /// </summary>
        [Model]
        public Prop Prop
        {
            get { return GetValue<Prop>(PropProperty); }
            private set
            {
                SetValue(PropProperty, value);
                InitializeLightViewModels();
            }
        }

        /// <summary>
        /// Prop property data.
        /// </summary>
        public static readonly PropertyData PropProperty = RegisterProperty("Prop", typeof(Prop));

        #endregion

        public ObservableCollection<LightViewModel> SelectedItems { get; set; }

        #region LightNodes property

        /// <summary>
        /// Gets or sets the LightNodes value.
        /// </summary>
        public ObservableCollection<LightViewModel> LightNodes
        {
            get { return GetValue<ObservableCollection<LightViewModel>>(LightNodesProperty); }
            set { SetValue(LightNodesProperty, value); }
        }

        /// <summary>
        /// LightNodes property data.
        /// </summary>
        public static readonly PropertyData LightNodesProperty = RegisterProperty("LightNodes", typeof(ObservableCollection<LightViewModel>));

        #endregion

        #region Width property

        /// <summary>
        /// Gets or sets the Width value.
        /// </summary>
        public double Width
        {
            get { return GetValue<double>(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Width property data.
        /// </summary>
        public static readonly PropertyData WidthProperty = RegisterProperty("Width", typeof(double));

        #endregion

        #region Height property

        /// <summary>
        /// Gets or sets the Height value.
        /// </summary>
        public double Height
        {
            get { return GetValue<double>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Height property data.
        /// </summary>
        public static readonly PropertyData HeightProperty = RegisterProperty("Height", typeof(double));

        #endregion

        #region X property

        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        public double X
        {
            get { return GetValue<double>(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// X property data.
        /// </summary>
        public static readonly PropertyData XProperty = RegisterProperty("X", typeof(double));

        #endregion

        #region Y property

        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        public double Y
        {
            get { return GetValue<double>(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Y property data.
        /// </summary>
        public static readonly PropertyData YProperty = RegisterProperty("Y", typeof(double));

        #endregion

        #region IsDrawing property

        /// <summary>
        /// Gets or sets the IsDrawing value.
        /// </summary>
        public bool IsDrawing
        {
            get { return GetValue<bool>(IsDrawingProperty); }
            set { SetValue(IsDrawingProperty, value); }
        }

        /// <summary>
        /// IsDrawing property data.
        /// </summary>
        public static readonly PropertyData IsDrawingProperty = RegisterProperty("IsDrawing", typeof(bool));

        #endregion

        #endregion Properties

        private void InitializeLightViewModels()
        {
            _elementModelMap.Clear();
            LightNodes.Clear();
            foreach (var elementModel in Prop.GetLeafNodes())
            {
               LightNodes.AddRange(CreateLightViewModels(elementModel));
            }
        }

        private List<LightViewModel> CreateLightViewModels(ElementModel em)
        {
            var lvmList = em.Lights.Select(x => new LightViewModel(x)).ToList();
            _elementModelMap.Add(em.Id, lvmList);
            return lvmList;
        }

        public void AddLightAt(Point p)
        {
            var em = new ElementModel("New One");
            em.AddLight(p);
            Prop.AddElementModel(em);
            LightNodes.AddRange(CreateLightViewModels(em));
        }

        public void DeleteSelectedLights()
        {
            foreach (var lightViewModel in SelectedItems)
            {
                
            }
        }

        public void DeselectAll()
        {
            LightNodes.ForEach(l => l.IsSelected = false);
            SelectedItems.Clear();
        }

        public void SelectLights(IEnumerable<ElementModel> em)
        {
            DeselectAll();
            SelectLightsForElementModels(em);
        }

        private void SelectLightsForElementModels(IEnumerable<ElementModel> em)
        {
            foreach (var elementModel in em)
            {
                if (!elementModel.IsLeaf)
                {
                    SelectLightsForElementModels(elementModel.Children);
                }
                else
                {
                    List<LightViewModel> lvmList;
                    if (_elementModelMap.TryGetValue(elementModel.Id, out lvmList))
                    {
                        lvmList.ForEach(l => l.IsSelected = true);
                        SelectedItems.AddRange(lvmList);
                    }
                }
                
            }
        }

        public void Transform(Transform t)
        {
            foreach (LightViewModel lvm in SelectedItems)
            {
                if (lvm.IsSelected)
                {
                    lvm.Center = t.Transform(lvm.Center);
                }
            }
        }

        public void AlignTops()
        {
            var ln = SelectedItems.First();
            foreach (var lightViewModel in SelectedItems)
            {
                lightViewModel.Light.Y = ln.Y;
            }
        }

        public void AlignBottoms()
        {
            var ln = SelectedItems.First();
            foreach (var lightViewModel in SelectedItems)
            {
                lightViewModel.Light.Y = ln.Y;
            }
        }

        public void AlignLeft()
        {
            var ln = SelectedItems.First();
            foreach (var lightViewModel in SelectedItems)
            {
                lightViewModel.Light.X = ln.X;
            }
        }

        public void AlignRight()
        {
            var ln = SelectedItems.First();
            foreach (var lightViewModel in SelectedItems)
            {
                lightViewModel.Light.X = ln.X;
            }
        }

        public void DistributeHorizontally()
        {
            if (SelectedItems.Count > 2)
            {
                var minX = SelectedItems.Min(x => x.Light.X);
                var maxX = SelectedItems.Max(x => x.Light.X);
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
                var minY = SelectedItems.Min(x => x.Light.Y);
                var maxY = SelectedItems.Max(x => x.Light.Y);
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

