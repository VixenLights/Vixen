using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.WPFCommon.Command;
using VixenModules.App.CustomPropEditor.Adorners;
using VixenModules.App.CustomPropEditor.ViewModel;

namespace VixenModules.App.CustomPropEditor.Controls
{
    public class PropDesignerCanvas:Canvas
    {
        private Point _originMouseStartPoint;
        private Point _previousPosition;
        private bool _isSelecting;
        private bool _dragging;
        private RubberbandAdorner _rubberbandAdorner;
        private PropEditorViewModel _propEditorViewModel;

        public PropDesignerCanvas()
        {
            Loaded += OnLoaded;
        }

        public static readonly DependencyProperty CoordinatesProperty =
            DependencyProperty.Register("Coordinates", typeof(Point), typeof(PropDesigner));

        public Point Coordinates
        {
            get { return (Point)GetValue(CoordinatesProperty); }
            set { SetValue(CoordinatesProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(PropDesigner));


        public IList SelectedItems
        {
            get
            {
                return (IList)GetValue(SelectedItemsProperty);
            }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }


        public static readonly DependencyProperty LightNodeViewModelsSourceProperty =
            DependencyProperty.Register("LightNodeViewModelsSource", typeof(IEnumerable), typeof(PropDesigner),
                new FrameworkPropertyMetadata(LightNodeViewModelsSource_PropertyChanged));


        private static void LightNodeViewModelsSource_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as PropDesigner;
            if (p != null)
            {
                p.ItemsSource = p.LightNodeViewModelsSource;
            }
        }

        /// <summary>
        /// A reference to the collection that is the source used to populate 'LightNodeViewModels'.
        /// Used in the same way as 'ItemsSource' in 'ItemsControl'.
        /// </summary>
        public IEnumerable LightNodeViewModelsSource
        {
            get
            {
                return (IEnumerable)GetValue(LightNodeViewModelsSourceProperty);
            }
            set
            {
                SetValue(LightNodeViewModelsSourceProperty, value);
            }
        }

        public static DependencyProperty TransformCommandProperty = DependencyProperty.Register(
                "TransformCommand",
                typeof(RelayCommand<Transform>),
                typeof(PropDesigner));

        public RelayCommand<Transform> TransformCommand
        {
            get
            {
                return (RelayCommand<Transform>)GetValue(TransformCommandProperty);
            }

            set
            {
                SetValue(TransformCommandProperty, value);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            
            _propEditorViewModel = DataContext as PropEditorViewModel;
            
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _originMouseStartPoint = _previousPosition = e.GetPosition(this);
            //if we are source of event, we are rubberband selecting
            if (e.Source == this)
            {
                if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    ClearSelections();
                }

                if (_propEditorViewModel.DrawingPanelViewModel.IsDrawing)
                {
                    _propEditorViewModel.DrawingPanelViewModel.AddLightAt(_originMouseStartPoint);
                }

                _isSelecting = false;
                e.Handled = true;
            }
            else if (!_propEditorViewModel.DrawingPanelViewModel.IsDrawing && e.Source is Path)
            {
                var p = e.Source as Path;
                var l = p.DataContext as ISelectable;
                if (l != null)
                {
                    _isSelecting = true;
                    if (!l.IsSelected)
                    {
                        if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                        {
                            ClearSelections();
                            SelectedItems.Add(l);
                            l.IsSelected = true;
                        }
                        else
                        {
                            if (l.IsSelected)
                            {
                                l.IsSelected = false;
                                SelectedItems.Remove(l);
                            }
                            else
                            {
                                SelectedItems.Add(l);
                                l.IsSelected = true;
                            }
                        }
                    }
                }
                e.Handled = false;
            }


        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_propEditorViewModel.DrawingPanelViewModel.IsDrawing)
            {
                return;
            }
            var position = e.GetPosition(this);
            if (_propEditorViewModel != null)
            {
                Coordinates = position;
            }

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                switch (_dragging)
                {
                    case true:
                        if (_isSelecting)
                        {
                            double XDelta = position.X - _previousPosition.X;
                            double YDelta = position.Y - _previousPosition.Y;
                            //Console.Out.WriteLine("Start: {0},{1} - Current {2}, {3} - Delta {4}, {5}", _originMouseStartPoint.X, _originMouseStartPoint.Y,
                            //    position.X, position.Y, XDelta, YDelta);
                            MoveSelectedItems(XDelta, YDelta);
                            _previousPosition = position;
                        }
                        else
                        {
                            _rubberbandAdorner.EndPoint = position;
                            UpdateSelection(_originMouseStartPoint, position);
                            e.Handled = true;
                        }

                        break;
                    case false:
                        if (!_isSelecting)
                        {
                            if (!IsMouseCaptured)
                            {
                                CaptureMouse();
                            }

                            // create rubberband adorner
                            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                            if (adornerLayer != null)
                            {
                                _rubberbandAdorner = new RubberbandAdorner(this, _originMouseStartPoint);
                                adornerLayer.Add(_rubberbandAdorner);
                            }
                        }
                        e.Handled = true;
                        _dragging = true;
                        break;

                }
            }

        }

        private void MoveSelectedItems(double xDelta, double yDelta)
        {
            TransformCommand.Execute(new TranslateTransform(xDelta, yDelta));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
            if (_dragging && !_isSelecting)
            {
                // remove rubberband adorner from adorner layer
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(_rubberbandAdorner);
                }

                e.Handled = true;
            }
            _dragging = false;
        }

        private void ClearSelections()
        {
            SelectedItems.Clear();
            
            foreach (ISelectable lvm in Children)
            {
                lvm.IsSelected = false;
            }
            
        }

        private void UpdateSelection(Point startPoint, Point endPoint)
        {
            Rect rubberBand = new Rect(startPoint, endPoint);

            foreach (Control item in Children)
            {
                //DependencyObject container = ItemContainerGenerator.ContainerFromItem(item);

                Rect itemRect = VisualTreeHelper.GetDescendantBounds(item);
                Rect itemBounds = item.TransformToAncestor(this).TransformBounds(itemRect);

                ISelectable selectableItem = item.DataContext as ISelectable;

                if (selectableItem != null)
                {
                    if (rubberBand.Contains(itemBounds))
                    {
                        selectableItem.IsSelected = true;
                        SelectedItems.Add(item);
                    }
                    else
                    {
                        if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                        {
                            SelectedItems.Remove(item);
                            selectableItem.IsSelected = false;
                        }
                    }
                }
                
            }
        }

        private void UpdateResizeAdorner()
        {
            if (SelectedItems.Count > 0)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    //var resizingAdorner = new ResizeAdorner(this, TransformCommand);
                    //resizingAdorner.
                    //resizingAdorner.Height = 50;
                    //resizingAdorner.Width = 50;
                    //adornerLayer.Add(resizingAdorner);
                }
            }
        }

        private static T FindVisualChild<T>(DependencyObject element) where T : class
        {
            if (element == null) return default(T);
            if (element is T) return element as T;
            if (VisualTreeHelper.GetChildrenCount(element) > 0)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    object child = VisualTreeHelper.GetChild(element, i);
                    if (child is T)
                        return child as T;

                    var res = FindVisualChild<T>(child as DependencyObject);
                    if (res == null) continue;
                    return res;
                }
            }
            return null;
        }

        private T GetParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent is T)
                return parent as T;

            if (parent is DependencyObject)
                return GetParent<T>(parent);
            return null;
        }
    }
}

