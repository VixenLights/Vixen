using Catel.Data;
using Catel.MVVM;
using System;
using System.Windows;
using System.Windows.Input;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Abstract base class for Fixture Property window view models. 
    /// </summary>
    public abstract class FixturePropertyWindowViewModelBase : ViewModelBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FixturePropertyWindowViewModelBase() 
        {
            // Create the OK button command
            OkCommand = new Command(OK, CanExecuteOK);

            // Create the Cancel button
            CancelCommand = new Command(Cancel);

            // Configure Catel to validate immediately
            DeferValidationUntilFirstSaveCall = false;
        }

        #endregion

        #region Public Catel Properties
   
        /// <summary>
        /// OK tooltip.
        /// </summary>
        public string OKTooltip
        {
            get
            {
                return GetValue<string>(OKTooltipProperty);
            }
            set
            {
                SetValue(OKTooltipProperty, value);
            }
        }

        /// <summary>
        /// Ok tooltip property data.
        /// </summary>
        public static readonly PropertyData OKTooltipProperty = RegisterProperty(nameof(OKTooltip), typeof(string), null);

        /// <summary>
        /// Determines if the Error triangle is displayed.
        /// </summary>
        public Visibility ShowError
        {
            get
            {
                return GetValue<Visibility>(ShowErrorProperty);
            }
            set
            {
                SetValue(ShowErrorProperty, value);
            }
        }

        /// <summary>
        /// Show Error property data.
        /// </summary>
        public static readonly PropertyData ShowErrorProperty = RegisterProperty(nameof(ShowError), typeof(Visibility), null);

        #endregion

        #region Public Commands

        /// <summary>
        /// Ok button command.
        /// </summary>
        public ICommand OkCommand { get; private set; }

        /// <summary>
        /// Cancel button command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Flag to prevent updating the tooltip repeatedly leading to a stack overflow exception.
        /// </summary>
        protected bool UpdatingOKTooltip { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the OK button tooltip.
        /// </summary>
        /// <param name="canOK">Flag indicating the OK button is disabled</param>
        /// <param name="childViewModel">Child view model associated with the window</param>
        protected void UpdateOKTooltip(bool canOK, IFixtureSaveable childViewModel)
        {
            // If we are not already updating the OK tooltip then...
            if (!UpdatingOKTooltip)
            {
                // Set a flag to prevent repeated attempts at updating the tooltip
                UpdatingOKTooltip = true;

                // If the OK button is disabled then...
                if (!canOK)
                {
                    // Update the OK tooltip
                    OKTooltip = childViewModel.GetValidationResults();
                    
                    // Show the Error triangle
                    ShowError = Visibility.Visible;
                }
                else
                {
                    // Hide the Error triangle
                    ShowError = Visibility.Hidden;

                    // Set the OK tooltip to empty
                    OKTooltip = string.Empty;
                }

                // Indicate we are done updating the tooltip
                UpdatingOKTooltip = false;
            }
        }

        /// <summary>
        /// Determines if the OK command can execute.
        /// </summary>
        /// <returns>True when the OK command can execute.</returns>
        protected abstract bool CanExecuteOK();
              
        /// <summary>
        /// OK command handler.
        /// </summary>
        protected virtual void OK()
        {            
            // Call Catel save processing
            this.SaveAndCloseViewModelAsync();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Cancel button command handler.
        /// </summary>
        private void Cancel()
        {
            // Call Catel processing
            this.CancelAndCloseViewModelAsync();
        }

        #endregion
    }
}
