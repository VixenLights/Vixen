using VixenModules.Editor.FixturePropertyEditor.ViewModels;
using VixenModules.Editor.FixtureWizard.Wizard.ViewModels;

namespace VixenModules.Editor.FixtureWizard.Wizard
{
    using Catel;
    using Catel.IoC;
    using Catel.Services;
    using Orc.Wizard;
    using System.Linq;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using VixenModules.Editor.FixtureWizard.Wizard.Models;

    /// <summary>
    /// Wizard for creating intelligent fixtures.
    /// </summary>
    /// <remarks>This class was created from the Catel Orc Wizard example</remarks>
    public class IntelligentFixtureWizard : SideNavigationWizardBase, IFixtureWizard
    {        
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeFactory">Catel type factory</param>
        /// <param name="messageService">Catel messageService</param>
        public IntelligentFixtureWizard(ITypeFactory typeFactory, IMessageService messageService)
            : base(typeFactory)
        {
            // Validate the message service argument
            Argument.IsNotNull(() => messageService);

            // Store off the message service
            _messageService = messageService;

            // Configure the wizard title
            Title = "Intelligent Fixture Wizard"; 

            // Add the pages that make up the wizard
            this.AddPage<SelectProfileWizardPage>();
            this.AddPage<EditProfileFunctionsWizardPage>();
            this.AddPage<EditProfileWizardPage>();            
            this.AddPage<ColorSupportWizardPage>();
            this.AddPage<AutomationWizardPage>();
            this.AddPage<DimmingCurveWizardPage>();
            this.AddPage<GroupingWizardPage>();
            this.AddPage<SummaryWizardPage>();

            // Update the description on the summary page
            Pages.Single(pg => pg is SummaryWizardPage).Description = "Below is a summary of the fixture selections.";

            // Configure the size of the wizard
            MinSize = new System.Windows.Size(1100, 600);
            MaxSize = new System.Windows.Size(1400, 600);
            ResizeMode = System.Windows.ResizeMode.CanResize;

            // Have the wizard determine the space between the page ellipses
            AutoSizeSideNavigationPane = true;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Catel message service.
        /// </summary>
        private readonly IMessageService _messageService;

        #endregion

        #region IFixtureWizard

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>
        public INavigationController NavigationControllerWrapper
        {
            get { return NavigationController; }
            set { NavigationController = value; }
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>
        public bool ShowInTaskbarWrapper
        {
            get {  return ShowInTaskbar; }
            set { ShowInTaskbar = value; }
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>
        public bool ShowHelpWrapper
        {
            get { return IsHelpVisible; }
            set { IsHelpVisible = value; }
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>
        public bool AllowQuickNavigationWrapper
        {
            get { return AllowQuickNavigation; }
            set { AllowQuickNavigation = value; }
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>
        public bool HandleNavigationStatesWrapper
        {
            get {  return HandleNavigationStates; }
            set { HandleNavigationStates = value; }
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>
        public bool CacheViewsWrapper
        {
            get { return CacheViews; }
            set { CacheViews = value; }
        }

        #endregion

        #region IWizard

        /// <summary>
        /// Catel Wizard override for showing intelligent fixture specific help.
        /// </summary>
        /// <returns>Task for showing the online help</returns>
        public override Task ShowHelpAsync()
        {     
            // The summary page is provided by Catel so it has to be handled specifically
            if (CurrentPage is SummaryWizardPage)
			{
                // Launch a browser with the help URL
                return Task.FromResult(Process.Start(new ProcessStartInfo("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/setup-display-elements/intelligent-fixture-wizard/summary/")));
            }
            else
            { 
                // Otherwise launch a browser with the help URL for the current page
                return Task.FromResult(Process.Start(new ProcessStartInfo(((FixtureWizardPageBase)CurrentPage).HelpURL)));                                       
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes the enabled / disabled status of the wizard navigation buttons.
        /// </summary>
        public void RefreshNavigationCommands()
        {
            // Refresh the view with the navigation commands
            RaisePropertyChanged(nameof(CanMoveBack));
            RaisePropertyChanged(nameof(CanMoveForward));
            RaisePropertyChanged(nameof(CanResume));
            RaisePropertyChanged(nameof(CanCancel));

            // Re-evaluate the navigation commands
            NavigationController.EvaluateNavigationCommands();
        }

        #endregion

        #region protected Methods

        /// <summary>
        /// Refer to base class documentation.
        /// </summary>
        public override bool CanMoveBack
        {
	        get
	        {
                // Call the base class implementation
		        bool canMoveBack = base.CanMoveBack;

                // If the current page implements the IIntelligentFixtureWizardPageViewModel interface then...
                if (CurrentPage.ViewModel is IIntelligentFixtureWizardPageViewModel)
                {
                    // Ask the page if it safe to move back to the previous page
	                canMoveBack &= ((IIntelligentFixtureWizardPageViewModel)CurrentPage.ViewModel).CanMoveBack();
                }

                return canMoveBack;
	        }
        }

        /// <summary>
        /// Refer to base class documentation.
        /// </summary>
        public override bool CanMoveForward
        {
	        get
	        {
		        // Call the base class implementation
		        bool canMoveForward = base.CanMoveForward;

		        // If the current page implements the IIntelligentFixtureWizardPageViewModel interface then...
		        if (CurrentPage.ViewModel is IIntelligentFixtureWizardPageViewModel)
		        {
			        // Ask the page if it safe to move back to the next page
			        canMoveForward &= ((IIntelligentFixtureWizardPageViewModel)CurrentPage.ViewModel).CanMoveNext();
		        }

		        return canMoveForward;
	        }
        }


        #endregion
    }
}
