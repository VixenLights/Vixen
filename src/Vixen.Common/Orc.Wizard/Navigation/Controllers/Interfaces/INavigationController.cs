namespace Orc.Wizard
{
    using System.Collections.Generic;

    public interface INavigationController
    {
        IEnumerable<IWizardNavigationButton> GetNavigationButtons();
        void EvaluateNavigationCommands();
    }
}
