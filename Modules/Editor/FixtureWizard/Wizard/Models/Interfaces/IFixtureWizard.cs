using Orc.Wizard;

namespace VixenModules.Editor.FixtureWizard.Wizard
{
    /// <summary>
    /// Configures the Intelligent Fixture wizard.
    /// </summary>
    /// <remarks>This interface is based on Catel Orc Wizard example</remarks>
    public interface IFixtureWizard : IWizard
    {
        /// <summary>
        /// Configures whether user is allowed to jump to any page in the wizard.
        /// </summary>
        bool AllowQuickNavigationWrapper { get; set; }

        /// <summary>
        /// Configures whether Catel/Orc helps determine if it is valid to move to the next or previous wizard page.
        /// </summary>
        bool HandleNavigationStatesWrapper { get; set; }

        /// <summary>
        /// Navigation controller to use with the wizard.
        /// </summary>
        INavigationController NavigationControllerWrapper { get; set; }

        /// <summary>
        /// Determines if the Help button is visible on the Wizard pages.
        /// </summary>
        bool ShowHelpWrapper { get; set; }

        /// <summary>
        /// Configures whether the wizard shows up in the Window taskbar.
        /// </summary>
        bool ShowInTaskbarWrapper { get; set; }

        /// <summary>
        /// Configures whether Catel/Orc wizard caches views.
        /// </summary>
        bool CacheViewsWrapper { get; set; }
    }
}
