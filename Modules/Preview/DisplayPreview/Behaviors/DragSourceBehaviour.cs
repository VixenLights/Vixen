namespace VixenModules.Preview.DisplayPreview.Behaviors
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    ///   The behaviour for objects acting as drag sources
    /// </summary>
    public class DragSourceBehaviour
    {
        // This can be static since the mouse is inherently single-threaded singleton :)

        /// <summary>
        ///   DragSource attached property
        /// </summary>
        public static readonly DependencyProperty DragSourceProperty = DependencyProperty.RegisterAttached(
                                                                                                           "DragSource", 
                                                                                                           typeof(IDragSource), 
                                                                                                           typeof(DragSourceBehaviour), 
                                                                                                           new PropertyMetadata(null, OnPropertyChanged));

        private static Point? _startPoint;

        /// <summary>
        ///   Gets the DragSource property.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object.</param>
        /// <returns>The drag source object.</returns>
        public static IDragSource GetDragSource(DependencyObject dependencyObject)
        {
            return (IDragSource)dependencyObject.GetValue(DragSourceProperty);
        }

        /// <summary>
        ///   Sets the DragSource property.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object.</param>
        /// <param name = "value">The value.</param>
        public static void SetDragSource(DependencyObject dependencyObject, IDragSource value)
        {
            dependencyObject.SetValue(DragSourceProperty, value);
        }

        private static bool HasMouseMovedFarEnough(MouseEventArgs e)
        {
            Debug.Assert(_startPoint != null);

            var delta = _startPoint.Value - e.GetPosition(null);

            return Math.Abs(delta.X) > SystemParameters.MinimumHorizontalDragDistance
                   || Math.Abs(delta.Y) > SystemParameters.MinimumVerticalDragDistance;
        }

        private static void MouseLeave(object sender, MouseEventArgs e)
        {
            // Need to reset since the mouse left in order to prevent mouse movement 
            // in another element to pick drag an drop
            _startPoint = null;
        }

        /// <summary>
        ///   Called when the DragSource property changes.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object.</param>
        /// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)dependencyObject;

            if (e.NewValue != null)
            {
                element.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
                element.PreviewMouseMove += PreviewMouseMove;
                element.MouseLeave += MouseLeave;
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= PreviewMouseLeftButtonDown;
                element.PreviewMouseMove -= PreviewMouseMove;
                element.MouseLeave -= MouseLeave;
            }
        }

        private static void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private static void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed
                || _startPoint == null)
            {
                return;
            }

            if (!HasMouseMovedFarEnough(e))
            {
                return;
            }

            var dependencyObject = (FrameworkElement)sender;
            var dataContext = dependencyObject.GetValue(FrameworkElement.DataContextProperty);
            var dragSource = GetDragSource(dependencyObject);

            if (dragSource.GetDragEffects(dataContext)
                == DragDropEffects.None)
            {
                return;
            }

            DragDrop.DoDragDrop(dependencyObject, dragSource.GetData(dataContext), dragSource.GetDragEffects(dataContext));
        }
    }
}
