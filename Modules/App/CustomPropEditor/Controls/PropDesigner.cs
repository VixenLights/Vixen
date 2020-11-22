using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Catel.MVVM;
using Common.WPFCommon.Command;
using VixenModules.App.CustomPropEditor.Adorners;
using VixenModules.App.CustomPropEditor.ViewModels;

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
			set { SetValue(CoordinatesProperty, value); }
		}

		public static readonly DependencyProperty IsDrawingProperty =
			DependencyProperty.Register("IsDrawing", typeof(bool), typeof(PropDesigner),
				new FrameworkPropertyMetadata(IsDrawing_PropertyChanged));

		private static void IsDrawing_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var p = d as PropDesigner;
			if (p != null)
			{
				if (p.IsDrawing)
				{
					p.RemoveResizeAdorner();
				}
				else
				{
					p.UpdateResizeAdorner();
				}
			}
		}

		public bool IsDrawing
		{
			get { return (bool)GetValue(IsDrawingProperty); }
			set { SetValue(IsDrawingProperty, value); }
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


		public static readonly DependencyProperty SelectedModelsProperty = DependencyProperty.Register(
			"SelectedModels", typeof(IList), typeof(PropDesigner), new FrameworkPropertyMetadata(OnSelectedModelsChanged));

		private static void OnSelectedModelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var propDesigner = d as PropDesigner;
			if (propDesigner == null)
				return;

			var oldValue = e.OldValue as INotifyCollectionChanged;
			var newValue = e.NewValue as INotifyCollectionChanged;

			if (oldValue != null)
			{
				oldValue.CollectionChanged -= propDesigner.Selected_CollectionChanged;
			}
			if (newValue != null)
			{
				propDesigner.SelectedItems.Clear();
				foreach (var item in (IEnumerable)newValue)
				{
					propDesigner.SelectedItems.Add(item);
				}
				newValue.CollectionChanged += propDesigner.Selected_CollectionChanged;
			}
		}

		public IList SelectedModels
		{
			get { return (IList)GetValue(SelectedModelsProperty); }
			set { SetValue(SelectedModelsProperty, value); }
		}


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

		public static DependencyProperty AddLightCommandProperty = DependencyProperty.Register(
			"AddLightCommand",
			typeof(Command<Point>),
			typeof(PropDesigner));

		public Command<Point> AddLightCommand
		{
			get
			{
				return (Command<Point>)GetValue(AddLightCommandProperty);
			}

			set
			{
				SetValue(AddLightCommandProperty, value);
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			_drawingCanvas = FindVisualChild<Canvas>(this);
			_propEditorViewModel = DataContext as PropEditorViewModel;
			if (_drawingCanvas != null)
			{
				_drawingCanvas.PreviewMouseDown += _drawingCanvas_PreviewMouseDown;
				_drawingCanvas.MouseLeftButtonDown += _drawingCanvas_MouseLeftMouseDown;
				_drawingCanvas.MouseMove += _drawingCanvas_MouseMove;
				_drawingCanvas.MouseLeftButtonUp += _drawingCanvas_MouseLeftButtonUp;
			}

			if (SelectedModels is INotifyCollectionChanged)
			{
				var selected = SelectedModels as INotifyCollectionChanged;
				selected.CollectionChanged += Selected_CollectionChanged;
			}
			//SelectionChanged += PropDesigner_SelectionChanged;
		}

		private void Selected_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!IsDrawing)
			{
				RemoveResizeAdorner();
				UpdateResizeAdorner();
			}
		}

		//private void PropDesigner_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//      {
		//          if (!IsDrawing)
		//          {
		//              UpdateResizeAdorner();
		//          }
		//      }

		private void _drawingCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void _drawingCanvas_MouseLeftMouseDown(object sender, MouseButtonEventArgs e)
		{
			OnMouseLeftButtonDown(e);

			Keyboard.Focus(_drawingCanvas);

			_originMouseStartPoint = _previousPosition = e.GetPosition(_drawingCanvas);
			//if we are source of event, we are rubberband selecting
			if (e.Source == _drawingCanvas)
			{
				if (!IsDrawing && !(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
				{
					ClearSelections();
				}

				if (_propEditorViewModel.DrawingPanelViewModel.IsDrawing)
				{
					AddLightCommand.Execute(_originMouseStartPoint);
					//ClearSelections();

				}

				_isSelecting = false;
				e.Handled = true;
			}
			else if (!IsDrawing && e.Source is Path)
			{
				var p = e.Source as Path;
				var l = p.DataContext as ISelectable;
				if (l != null)
				{
					_isSelecting = true;


					if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
					{
						if (!l.IsSelected)
						{
							ClearSelections();
							SelectedModels.Add(l);
							l.IsSelected = true;
						}

					}
					else
					{
						if (l.IsSelected)
						{
							l.IsSelected = false;
							SelectedModels.Remove(l);
						}
						else
						{
							SelectedModels.Add(l);
							l.IsSelected = true;
						}
					}

				}
				e.Handled = true;
			}


		}

		private void _drawingCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			OnMouseMove(e);

			var position = e.GetPosition(_drawingCanvas);
			if (_propEditorViewModel != null)
			{
				Coordinates = position;
			}

			if (IsDrawing)
			{
				return;
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
							RemoveResizeAdorner();
							UpdateResizeAdorner();
						}
						else
						{
							if (_rubberbandAdorner != null)
							{
								_rubberbandAdorner.EndPoint = position;
							}
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

		internal void MoveSelectedItems(double xDelta, double yDelta)
		{
			TransformCommand.Execute(new TranslateTransform(xDelta, yDelta));
		}

		internal void MoveSelectedItems(Transform transform)
		{
			TransformCommand.Execute(transform);
			RefreshResizeAdorner();
		}

		internal void ScaleSelectedItems(double xDelta, double yDelta, Point center)
		{
			TransformCommand.Execute(new ScaleTransform(xDelta, yDelta, center.X, center.Y));
			RefreshResizeAdorner();
		}

		internal void TransformSelectedItems(TransformGroup t)
		{
			TransformCommand.Execute(t);
			RefreshResizeAdorner();
		}

		internal void RotateSelectedItems(double angle, Point center)
		{
			TransformCommand.Execute(new RotateTransform(angle, center.X, center.Y));
			RefreshResizeAdorner();
		}

		private void RefreshResizeAdorner()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_drawingCanvas);

			if (adornerLayer != null)
			{
				adornerLayer.Update();
			}
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

			_dragging = false;
			RemoveResizeAdorner();
			UpdateResizeAdorner();
		}

		private void ClearSelections()
		{
			if (SelectedModels.Count > 0)
			{
				SelectedModels.Clear();
				if (ItemsSource != null)
				{
					foreach (ISelectable lvm in ItemsSource)
					{
						lvm.IsSelected = false;
					}
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
					if (!item.IsSelected)
					{
						item.IsSelected = true;
						SelectedModels.Add(item);
					}

				}
				else
				{
					if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
					{
						SelectedModels.Remove(item);
						item.IsSelected = false;
					}
				}
			}
		}

		private void UpdateResizeAdorner()
		{
			if (IsDrawing) return;

			if (_dragging && !_isSelecting) return;

			if (SelectedModels.Count > 1)
			{
				Rect? bounds = GetSelectedContentBounds();

				if (bounds.HasValue && bounds != Rect.Empty)
				{
					AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_drawingCanvas);

					if (adornerLayer != null)
					{
						if (_resizingAdorner == null)
						{
							_resizingAdorner = new ResizeAdorner(_drawingCanvas, this, bounds.Value);
							adornerLayer.Add(_resizingAdorner);
							//_resizingAdorner.Bounds = bounds.Value;
						}
						UpdateLayout();
						adornerLayer.Update();
					}
				}

			}
			else 
			{
				RemoveResizeAdorner();
			}
		}

		public void RemoveResizeAdorner()
		{
			if (_resizingAdorner != null)
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
			foreach (var selectedItem in SelectedModels)
			{
				DependencyObject container = ItemContainerGenerator.ContainerFromItem(selectedItem);

				Rect itemRect = VisualTreeHelper.GetDescendantBounds((Visual)container);
				if (itemRect == Rect.Empty)
				{
					//try to derive from the light coord.
					var lm = selectedItem as LightViewModel;
					if (lm != null)
					{
						itemRect = new Rect(lm.X-lm.Size, lm.Y-lm.Size, lm.Size*2, lm.Size*2);
					}
				}
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
			if(!recs.Any()) return Rect.Empty;
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
