using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace VixenModules.Editor.EffectEditor.Input
{
	public static class DragDropManager
	{
		private static readonly string DragOffsetFormat = "DnD.DragOffset";

		public static readonly DependencyProperty DragSourceAdvisorProperty =
			DependencyProperty.RegisterAttached("DragSourceAdvisor", typeof (IDragSourceAdvisor),
				typeof (DragDropManager),
				new FrameworkPropertyMetadata(
					OnDragSourceAdvisorChanged));

		public static readonly DependencyProperty DropTargetAdvisorProperty =
			DependencyProperty.RegisterAttached("DropTargetAdvisor", typeof (IDropTargetAdvisor),
				typeof (DragDropManager),
				new FrameworkPropertyMetadata(
					OnDropTargetAdvisorChanged));

		private static Point _adornerPosition;

		private static UIElement _draggedElt;
		private static Point _dragStartPoint;
		private static Point _offsetPoint;
		private static DropPreviewAdorner _overlayElt;
		private static IDragSourceAdvisor s_currentDragSourceAdvisor;
		private static IDropTargetAdvisor s_currentDropTargetAdvisor;

		private static IDragSourceAdvisor CurrentDragSourceAdvisor
		{
			get { return s_currentDragSourceAdvisor; }
			set { s_currentDragSourceAdvisor = value; }
		}

		private static IDropTargetAdvisor CurrentDropTargetAdvisor
		{
			get { return s_currentDropTargetAdvisor; }
			set { s_currentDropTargetAdvisor = value; }
		}

		#region Dependency Properties Getter/Setters

		public static void SetDragSourceAdvisor(DependencyObject depObj, IDragSourceAdvisor advisor)
		{
			depObj.SetValue(DragSourceAdvisorProperty, advisor);
		}

		public static void SetDropTargetAdvisor(DependencyObject depObj, IDropTargetAdvisor advisor)
		{
			depObj.SetValue(DropTargetAdvisorProperty, advisor);
		}

		public static IDragSourceAdvisor GetDragSourceAdvisor(DependencyObject depObj)
		{
			return depObj.GetValue(DragSourceAdvisorProperty) as IDragSourceAdvisor;
		}

		public static IDropTargetAdvisor GetDropTargetAdvisor(DependencyObject depObj)
		{
			return depObj.GetValue(DropTargetAdvisorProperty) as IDropTargetAdvisor;
		}

		#endregion

		#region Property Change handlers

		private static void OnDragSourceAdvisorChanged(DependencyObject depObj,
			DependencyPropertyChangedEventArgs args)
		{
			UIElement sourceElt = depObj as UIElement;
			if (args.NewValue != null && args.OldValue == null)
			{
				sourceElt.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
				sourceElt.PreviewMouseMove += DragSource_PreviewMouseMove;
				sourceElt.PreviewMouseUp += DragSource_PreviewMouseUp;

			}
			else if (args.NewValue == null && args.OldValue != null)
			{
				sourceElt.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
				sourceElt.PreviewMouseMove -= DragSource_PreviewMouseMove;
				sourceElt.PreviewMouseUp -= DragSource_PreviewMouseUp;
			}
		}

		private static void OnDropTargetAdvisorChanged(DependencyObject depObj,
			DependencyPropertyChangedEventArgs args)
		{
			UIElement targetElt = depObj as UIElement;
			if (args.NewValue != null && args.OldValue == null)
			{
				targetElt.PreviewDragEnter += DropTarget_PreviewDragEnter;
				targetElt.PreviewDragOver += DropTarget_PreviewDragOver;
				targetElt.PreviewDragLeave += DropTarget_PreviewDragLeave;
				targetElt.PreviewDrop += DropTarget_PreviewDrop;
				targetElt.AllowDrop = true;
			}
			else if (args.NewValue == null && args.OldValue != null)
			{
				targetElt.PreviewDragEnter -= DropTarget_PreviewDragEnter;
				targetElt.PreviewDragOver -= DropTarget_PreviewDragOver;
				targetElt.PreviewDragLeave -= DropTarget_PreviewDragLeave;
				targetElt.PreviewDrop -= DropTarget_PreviewDrop;
				targetElt.AllowDrop = false;
			}
		}

		#endregion

		/* ____________________________________________________________________
		 *		Drop Target events 
		 * ____________________________________________________________________
		 */

		private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
		{
			UpdateEffects(e);

			Point dropPoint = e.GetPosition(sender as UIElement);

			// Calculate displacement for (Left, Top)
			Point offset = e.GetPosition(_overlayElt);
			dropPoint.X = dropPoint.X - offset.X;
			dropPoint.Y = dropPoint.Y - offset.Y;

			RemovePreviewAdorner();
			_offsetPoint = new Point(0, 0);

			if (CurrentDropTargetAdvisor.IsValidDataObject(e.Data))
			{
				CurrentDropTargetAdvisor.OnDropCompleted(e.Data, dropPoint);
			}
			e.Handled = true;
		}

		private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
		{
			UpdateEffects(e);

			RemovePreviewAdorner();
			e.Handled = true;
		}

		private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
		{
			UpdateEffects(e);

			// Update position of the preview Adorner
			_adornerPosition = e.GetPosition(sender as UIElement);
			PositionAdorner();

			e.Handled = true;
		}

		private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
		{
			// Get the current drop target advisor
			CurrentDropTargetAdvisor = GetDropTargetAdvisor(sender as DependencyObject);
			CurrentDropTargetAdvisor.TargetUI = sender as UIElement;

			UpdateEffects(e);

			// Setup the preview Adorner
			_offsetPoint = new Point();
			if (CurrentDropTargetAdvisor.ApplyMouseOffset && e.Data.GetData(DragOffsetFormat) != null)
			{
				_offsetPoint = (Point) e.Data.GetData(DragOffsetFormat);
			}
			CreatePreviewAdorner(sender as UIElement, e.Data);

			e.Handled = true;
		}

		private static void UpdateEffects(DragEventArgs e)
		{
			if (CurrentDropTargetAdvisor.IsValidDataObject(e.Data) == false)
			{
				e.Effects = DragDropEffects.None;
			}

			else if ((e.AllowedEffects & DragDropEffects.Move) == 0 &&
			         (e.AllowedEffects & DragDropEffects.Copy) == 0)
			{
				e.Effects = DragDropEffects.None;
			}

			else if ((e.AllowedEffects & DragDropEffects.Move) != 0 &&
			         (e.AllowedEffects & DragDropEffects.Copy) != 0)
			{
				e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0)
					? DragDropEffects.Copy
					: DragDropEffects.Move;
			}
		}

		/* ____________________________________________________________________
		 *		Drag Source events 
		 * ____________________________________________________________________
		 */

		private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount > 1) return;
			// Make this the new drag source
			CurrentDragSourceAdvisor = GetDragSourceAdvisor(sender as DependencyObject);
			CurrentDragSourceAdvisor.SourceUI = sender as UIElement;

			if (CurrentDragSourceAdvisor.IsDraggable(e.Source as UIElement) == false)
			{
				return;
			}

			_draggedElt = e.Source as UIElement;
			_dragStartPoint = e.GetPosition(CurrentDragSourceAdvisor.GetTopContainer());
			_offsetPoint = e.GetPosition(_draggedElt);
		}

		private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (CurrentDragSourceAdvisor != null && e.LeftButton == MouseButtonState.Pressed &&
			    IsDragGesture(e.GetPosition(CurrentDragSourceAdvisor.GetTopContainer())))
			{
				DragStarted(sender as UIElement);
			}
			else
			{
				Mouse.Capture(null);
			}
		}

		private static void DragSource_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			_draggedElt = null;
			if (CurrentDragSourceAdvisor != null)
			{
				CurrentDragSourceAdvisor.SourceUI = null;
				CurrentDragSourceAdvisor = null;
			}
			Mouse.Capture(null);
		}

		private static void DragStarted(UIElement uiElt)
		{
			if (uiElt == null || _draggedElt == null) return;
			Mouse.Capture(uiElt);

			DataObject data = CurrentDragSourceAdvisor.GetDataObject(_draggedElt);

			data.SetData(DragOffsetFormat, _offsetPoint);
			DragDropEffects supportedEffects = CurrentDragSourceAdvisor.SupportedEffects;

			// Perform DragDrop

			DragDropEffects effects = DragDrop.DoDragDrop(_draggedElt, data, supportedEffects);
			CurrentDragSourceAdvisor.FinishDrag(_draggedElt, effects);

			// Clean up
			RemovePreviewAdorner();
			Mouse.Capture(null);
			_draggedElt = null;
		}

		private static bool IsDragGesture(Point point)
		{
			bool hGesture = Math.Abs(point.X - _dragStartPoint.X) >
			                SystemParameters.MinimumHorizontalDragDistance;
			bool vGesture = Math.Abs(point.Y - _dragStartPoint.Y) >
			                SystemParameters.MinimumVerticalDragDistance;

			return (hGesture | vGesture);
		}

		/* ____________________________________________________________________
		 *		Utility functions
		 * ____________________________________________________________________
		 */

		private static void CreatePreviewAdorner(UIElement adornedElt, IDataObject data)
		{
			if (_overlayElt != null)
			{
				return;
			}

			AdornerLayer layer = AdornerLayer.GetAdornerLayer(CurrentDropTargetAdvisor.GetTopContainer());
			UIElement feedbackUI = CurrentDropTargetAdvisor.GetVisualFeedback(data);
			_overlayElt = new DropPreviewAdorner(feedbackUI, adornedElt);
			PositionAdorner();
			layer.Add(_overlayElt);
		}

		private static void PositionAdorner()
		{
			_overlayElt.Left = _adornerPosition.X - _offsetPoint.X;
			_overlayElt.Top = _adornerPosition.Y - _offsetPoint.Y;
		}

		private static void RemovePreviewAdorner()
		{
			if (_overlayElt != null)
			{
				AdornerLayer.GetAdornerLayer(CurrentDropTargetAdvisor.GetTopContainer()).Remove(_overlayElt);
				_overlayElt = null;
			}
		}
	}
}