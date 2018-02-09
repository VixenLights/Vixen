using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Common.WPFCommon.Command;
using VixenModules.App.CustomPropEditor.Adorners;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.ViewModel;

namespace VixenModules.App.CustomPropEditor.Controls
{
    public class PropDesigner : MultiSelector
    {
        private Canvas _drawingCanvas;
        private Point _originMouseStartPoint;
        private Point _previousPosition;
        private bool _isSelecting;
        private RubberbandAdorner _rubberbandAdorner;
        private ResizeAdorner _resizingAdorner;
        private PropEditorViewModel _propEditorViewModel;
        private bool _dragging;

        public PropDesigner()
        {
            Loaded += OnLoaded;      
        }

        public static readonly DependencyProperty CoordinatesProperty =
            DependencyProperty.Register("Coordinates", typeof(Point), typeof(PropDesigner));

        public Point Coordinates
        {
            get { return (Point)GetValue(CoordinatesProperty); }
            set { SetValue(CoordinatesProperty, value);}
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
        /// A reference to the collection that is the source used to populate 'Connections'.
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
            _drawingCanvas = FindVisualChild<Canvas>(this);
            _propEditorViewModel = DataContext as PropEditorViewModel;
            if (_drawingCanvas != null)
            {
                _drawingCanvas.PreviewMouseDown += _drawingCanvas_PreviewMouseDown                ;
                _drawingCanvas.MouseLeftButtonDown += _drawingCanvas_MouseLeftMouseDown;
                _drawingCanvas.MouseMove += _drawingCanvas_MouseMove;
                _drawingCanvas.MouseLeftButtonUp += _drawingCanvas_MouseLeftButtonUp;
            }

            SelectionChanged += PropDesigner_SelectionChanged;
        }

        private void PropDesigner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateResizeAdorner();
        }

        private void _drawingCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void _drawingCanvas_MouseLeftMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnMouseLeftButtonDown(e);

            _originMouseStartPoint = _previousPosition = e.GetPosition(_drawingCanvas);
            //if we are source of event, we are rubberband selecting
            if (e.Source == _drawingCanvas)
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
                    
                    
                    //if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    //{
                    //    ClearSelections();
                    //    SelectedItems.Add(l);
                    //    l.IsSelected = true;
                    //}
                    //else
                    //{
                    //    if (l.IsSelected)
                    //    {
                    //        l.IsSelected = false;
                    //        SelectedItems.Remove(l);
                    //    }
                    //    else
                    //    {
                    //        SelectedItems.Add(l);
                    //        l.IsSelected = true;
                    //    }
                    //}
                    
                }
                e.Handled = false;
            }


        }

        private void _drawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);

            if (_propEditorViewModel.DrawingPanelViewModel.IsDrawing)
            {
                return;
            }
            var position = e.GetPosition(_drawingCanvas);
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
                            UpdateResizeAdorner();
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
                            if (!_drawingCanvas.IsMouseCaptured)
                            {
                                _drawingCanvas.CaptureMouse();
                            }

                            // create rubberband adorner
                            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                            if (adornerLayer != null)
                            {
                                _rubberbandAdorner = new RubberbandAdorner(_drawingCanvas, _originMouseStartPoint);
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
            UpdateResizeAdorner();
        }

        internal void ScaleSelectedItems(double xDelta, double yDelta, Point center)
        {
            TransformCommand.Execute(new ScaleTransform(xDelta, yDelta, center.X, center.Y));
            UpdateResizeAdorner();
        }

        private void _drawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_drawingCanvas.IsMouseCaptured)
            {
                _drawingCanvas.ReleaseMouseCapture();
            }
            if (_dragging && !_isSelecting)
            {
                // remove rubberband adorner from adorner layer
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_drawingCanvas);
                if (adornerLayer != null && _rubberbandAdorner != null)
                {
                    adornerLayer.Remove(_rubberbandAdorner);
                    _rubberbandAdorner = null;
                }

                e.Handled = true;
            }
            else
            {
                if (!_propEditorViewModel.DrawingPanelViewModel.IsDrawing && !_dragging && e.Source is Path)
                {
                    var p = e.Source as Path;
                    var l = p.DataContext as ISelectable;
                    if (l != null)
                    {
                        _isSelecting = true;
                        //if (!l.IsSelected)
                        //{
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
                        //}
                    }
                    e.Handled = false;
                }
            }
            _dragging = false;
            UpdateResizeAdorner();
        }

        private void ClearSelections()
        {
            SelectedItems.Clear();
            if (ItemsSource != null)
            {
                foreach (ISelectable lvm in ItemsSource)
                {
                    lvm.IsSelected = false;
                }
            }
        }

        private void UpdateSelection(Point startPoint, Point endPoint)
        {
            Rect rubberBand = new Rect(startPoint, endPoint);
            
            foreach (ISelectable item in ItemsSource)
            {
                DependencyObject container = ItemContainerGenerator.ContainerFromItem(item);

                Rect itemRect = VisualTreeHelper.GetDescendantBounds((Visual)container);
                Rect itemBounds = ((Visual)container).TransformToAncestor(_drawingCanvas).TransformBounds(itemRect);

                if (rubberBand.Contains(itemBounds))
                {
                    item.IsSelected = true;
                    SelectedItems.Add(item);
                }
                else
                {
                    if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    {
                        SelectedItems.Remove(item);
                        item.IsSelected = false;
                    }
                }
            }
        }

        private void UpdateResizeAdorner()
        {
            if (_dragging && !_isSelecting) return;
            if (SelectedItems.Count > 1)
            {
                Rect? bounds = GetSelectedContentBounds();

                if (bounds.HasValue)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_drawingCanvas);

                    if (adornerLayer != null)
                    {
                        if (_resizingAdorner == null)
                        {
                            _resizingAdorner = new ResizeAdorner(_drawingCanvas, this);
                            adornerLayer.Add(_resizingAdorner);
                        }
                        _resizingAdorner.Bounds = bounds.Value;
                    }
                }
               
            }
            else if(_resizingAdorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_drawingCanvas);

                if (adornerLayer != null)
                {
                   adornerLayer.Remove(_resizingAdorner);
                   _resizingAdorner = null;
                }
            }
        }

        private Rect? GetSelectedContentBounds()
        {
            List<Rect> bounds = new List<Rect>();
            foreach (var selectedItem in SelectedItems)
            {
                DependencyObject container = ItemContainerGenerator.ContainerFromItem(selectedItem);

                Rect itemRect = VisualTreeHelper.GetDescendantBounds((Visual)container);
                Rect itemBounds = ((Visual)container).TransformToAncestor(_drawingCanvas).TransformBounds(itemRect);

                if (itemBounds != Rect.Empty)
                {
                    bounds.Add(itemBounds);
                }
            }

            return GetCombinedBounds(bounds);
        }

        private static Rect GetCombinedBounds(List<Rect> recs)
        {
            double xMin = recs.Min(s => s.X);
            double yMin = recs.Min(s => s.Y);
            double xMax = recs.Max(s => s.X + s.Width);
            double yMax = recs.Max(s => s.Y + s.Height);
            var rect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
            return rect;
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
