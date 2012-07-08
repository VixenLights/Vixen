namespace VixenModules.App.DisplayPreview.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    ///   The view model base.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        ///   Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name = "propertyName">
        ///   The property that has a new value.
        /// </param>
        protected void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///   Warns the developer if this object does not have
        ///   a public property with the specified name. This 
        ///   method does not exist in a Release build.
        /// </summary>
        /// <param name = "propertyName">
        ///   The property Name.
        /// </param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new Exception(string.Format("Invalid property name: {0}", propertyName));
            }
        }
    }
}
