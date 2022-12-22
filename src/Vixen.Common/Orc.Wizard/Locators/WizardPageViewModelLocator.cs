// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardPageViewModelLocator.cs" company="WildGums">
//   Copyright (c) 2013 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
	using Catel.MVVM;

    public class WizardPageViewModelLocator : ViewModelLocator, IWizardPageViewModelLocator
    {
        public WizardPageViewModelLocator()
        {
            NamingConventions.Add("[CURRENT].ViewModels.[VW]PageViewModel");
            NamingConventions.Add("[CURRENT].ViewModels.[VW]ViewModel");
        }
    }
}
