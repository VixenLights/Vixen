namespace VixenModules.Preview.DisplayPreview.Behaviors
{
    using System.Windows;

    /// <summary>
    ///   The behaviour for objects acting as drag sources
    /// </summary>
    public class DropTargetBehaviour
    {
        /// <summary>
        ///   DropTarget Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty DropTargetProperty = DependencyProperty.RegisterAttached(
                                                                                                           "DropTarget", 
                                                                                                           typeof(IDropTarget), 
                                                                                                           typeof(DropTargetBehaviour), 
                                                                                                           new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        ///   Gets the drop target property value.
        /// </summary>
        /// <param name = "d">The dependency object.</param>
        /// <returns>The drop target object.</returns>
        public static IDropTarget GetDropTarget(DependencyObject d)
        {
            return (IDropTarget)d.GetValue(DropTargetProperty);
        }

        /// <summary>
        ///   Sets the drop target property value.
        /// </summary>
        /// <param name = "d">The dependency object.</param>
        /// <param name = "value">The value.</param>
        public static void SetDropTarget(DependencyObject d, IDropTarget value)
        {
            d.SetValue(DropTargetProperty, value);
        }

        private static void DragOver(object sender, DragEventArgs e)
        {
            var dropTarget = GetDropTarget((DependencyObject)sender);

            e.Effects = dropTarget.GetDropEffects(e.Data);
            e.Handled = true;
        }

        private static void Drop(object sender, DragEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            var dropTarget = GetDropTarget((DependencyObject)sender);
            dropTarget.Drop(e.Data, point);
            e.Handled = true;
        }

        /// <summary>
        ///   Called when the DropTarget property changes.
        /// </summary>
        /// <param name = "d">The dependency object.</param>
        /// <param name = "e">The <see cref = "System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)d;

            if (e.NewValue != null)
            {
                element.DragOver += DragOver;
                element.Drop += Drop;
            }
            else
            {
                element.DragOver -= DragOver;
                element.Drop -= Drop;
            }
        }
    }
}
