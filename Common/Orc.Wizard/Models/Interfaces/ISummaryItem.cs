// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISummaryItem.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
    public interface ISummaryItem
    {
        IWizardPage Page { get; set; }

        string Title { get; set; }
        string Summary { get; set; }
    }
}
