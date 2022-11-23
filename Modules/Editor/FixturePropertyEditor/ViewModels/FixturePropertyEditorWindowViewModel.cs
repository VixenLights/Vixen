using System;
using Catel.Data;
using Catel.MVVM;
using System.Collections.Generic;
using System.Linq;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// View model for the fixture property editor window.
    /// </summary>
    public class FixturePropertyEditorWindowViewModel : FixturePropertyWindowViewModelBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FixturePropertyEditorWindowViewModel()
        {            
        }

        #endregion

        #region Public Catel Properties

        /// <summary>
        /// Fixture specification associated with the property.
        /// </summary>
        public Tuple<FixtureSpecification, Action, bool> FixtureSpecification
        {
            get
            {
                return GetValue<Tuple<FixtureSpecification, Action, bool>>(FixtureSpecificationProperty);
            }
            set
            {
                SetValue(FixtureSpecificationProperty, value);
            }
        }

        /// <summary>
        /// Fixture specification property data.
        /// </summary>
        public static readonly PropertyData FixtureSpecificationProperty = RegisterProperty(nameof(FixtureSpecification), typeof(Tuple<FixtureSpecification, Action, bool>), null);

        #endregion

        #region Protected Methods

        /// <summary>
        /// <inheritdoc/> Refer to base class documentation.
        /// </summary>       
        protected override bool CanExecuteOK()
        {
            // Default to enabling the OK command
            bool canOK = true;            

            // Get the child view model (This corresponds to the view model associated with child control)
            FixturePropertyEditorViewModel fixturePropertyEditorVM = GetChildViewModel();
            
            // If the child VM has been created then...
            if (fixturePropertyEditorVM != null)
            {
                // Force Catel to validate
                fixturePropertyEditorVM.Validate(true);

                // Check with the child VM if all required data is populated
                canOK = fixturePropertyEditorVM.CanSave();

                // Update the OK button tooltip
                UpdateOKTooltip(canOK, fixturePropertyEditorVM);
            }

            return canOK;
        }

        /// <summary>
        /// <inheritdoc/> Refer to base class documentation.
        /// </summary>
        protected override void OK()
        {                        
            // Get the updated fixture specification from the child view model
            FixtureSpecification = new Tuple<FixtureSpecification, Action, bool>(GetChildViewModel().GetFixtureSpecification(), FixtureSpecification.Item2, true);

            // Save the fixture to the local fixture repository
            GetChildViewModel().SaveSpecification();

            // Call base class to perform default Catel processing
            base.OK();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the child view model associated with the fixture property editor control view.
        /// </summary>
        /// <returns>Fixture property editor view model</returns>
        private FixturePropertyEditorViewModel GetChildViewModel()
        {
            // Default the child view model to null
            FixturePropertyEditorViewModel childVM = null;

            // Get the list of child view models from the Catel base class
            IList<IViewModel> childViewModels = GetChildViewModels().ToList();

            // If at least one child view model was returned then...
            if (childViewModels.Count > 0)
            {
                // Cast the view model to the derived type
                childVM = (FixturePropertyEditorViewModel)childViewModels[0];
            }

            // Return the fixture property editor control view model
            return childVM;
        }

        #endregion
    }
}
