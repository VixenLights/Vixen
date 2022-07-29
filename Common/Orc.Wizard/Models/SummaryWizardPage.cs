// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SummaryWizardPage.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
    using Catel;
    using Catel.Services;

    public class SummaryWizardPage : WizardPageBase
    {
        public SummaryWizardPage(ILanguageService languageService)
        {
            Argument.IsNotNull(() => languageService);

            Title = languageService.GetString("Wizard_SummaryTitle");
            Description = languageService.GetString("Wizard_SummaryDescription");
        }
    }
}
