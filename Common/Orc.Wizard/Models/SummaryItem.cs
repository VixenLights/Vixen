// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SummaryItem.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
    public class SummaryItem : ISummaryItem
    {
        public IWizardPage Page { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }
    }
}
