using Catel.Data;
using Catel.MVVM;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Maintains a window used to edit the fixture function types.
    /// </summary>
    public class FunctionTypeWindowViewModel : FixturePropertyWindowViewModelBase
    {
        #region Constructor 

        /// <summary>
        /// Constructor
        /// </summary>
        public FunctionTypeWindowViewModel()
        {            
        }

        #endregion

        #region Catel Public Properties

        /// <summary>
        /// Tuple that contains the functions associated with the fixture and a specific function to select.
        /// </summary>
        public Tuple<List<FixtureFunction>, string, Action> Functions
        {
            get
            {
                return GetValue<Tuple<List<FixtureFunction>, string, Action>>(FunctionsProperty);
            }
            set
            {
                SetValue(FunctionsProperty, value);
            }
        }

        /// <summary>
        /// Functions property data.
        /// </summary>
        public static readonly PropertyData FunctionsProperty = RegisterProperty(nameof(Functions), typeof(Tuple<List<FixtureFunction>, string, Action>), null);

        #endregion

        #region Public Properties

        /// <summary>
        /// Functions that were updated FunctionTypeViewModel (User Control).
        /// </summary>
        public List<FixtureFunction> UpdatedFunctions { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the view model with the functions associated with the fixture and which function to select initially.
        /// </summary>
        /// <param name="functions">Functions associated with the fixture</param>
        /// <param name="functionToSelect">Function to select</param>
        public void InitializeChildViewModels(List<FixtureFunction> functions, string functionToSelect)
        {
            // Store off the fixture functions and which function to select as a Tuple
            Functions = new Tuple<List<FixtureFunction>, string, Action>(functions, functionToSelect, ((Command)OkCommand).RaiseCanExecuteChanged);
        }

        #endregion

        #region Protected Methods
        
        /// <summary>
        /// Returns true if the OK command can be executed.
        /// </summary>
        /// <returns>True if the OK command can be executed</returns>
        protected override bool CanExecuteOK()
        {
            // Default to disabling the OK command
            bool canOK = false;

            // Force Catel to validate
            Validate(true);

            // Attempt to get the child view model
            FunctionTypeViewModel functionTypeViewModel = GetChildViewModel();

            // If the child view model has been created then...
            if (functionTypeViewModel != null)
            {                
                // Delegate to the child view model
                canOK = functionTypeViewModel.CanSave();

                // Update the OK button tooltip
                UpdateOKTooltip(canOK, functionTypeViewModel);                
            }

            // Return whether the OK command can be executed
            return canOK;
        }
              
        /// <summary>
        /// OK command handler.
        /// </summary>
        protected override void OK()
        {
            // Update the functions retrieving them from the child view model
            UpdatedFunctions = GetChildViewModel().GetFunctionData();
                                                        
            // Call base class Catel processing
            base.OK();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the child view model.  This is the view model associated with the user control.
        /// </summary>
        /// <returns>Child view model</returns>
        private FunctionTypeViewModel GetChildViewModel()
        {
            // Default the child view model to null
            FunctionTypeViewModel childViewModel = null;

            // Get the collection of child view models from Catel base classes
            IList<IViewModel> childViewModels = GetChildViewModels().ToList();

            // If the child view models have been created then...
            if (childViewModels.Count > 0)
            {
                // Cast the view model to the specific type
                childViewModel = (FunctionTypeViewModel)childViewModels[0];
            }

            return childViewModel;
        }

        #endregion
    }
}
