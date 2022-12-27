// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWizardService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
    using System.Threading.Tasks;

    public interface IWizardService
    {
        Task<bool?> ShowWizardAsync(IWizard wizard);
    }
}