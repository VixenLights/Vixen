namespace VixenModules.Preview.DisplayPreview.Behaviors
{
    using System.Windows;

    /// <summary>
    ///   Business end of the drag source
    /// </summary>
    public interface IDragSource
    {
        /// <summary>
        ///   Gets the data.
        /// </summary>
        /// <param name = "dataContext">The data context.</param>
        /// <returns>The object being dragged.</returns>
        object GetData(object dataContext);

        /// <summary>
        ///   Gets the supported drop effects.
        /// </summary>
        /// <param name = "dataContext">The data context.</param>
        /// <returns>The drag effects to be applied.</returns>
        DragDropEffects GetDragEffects(object dataContext);
    }
}
