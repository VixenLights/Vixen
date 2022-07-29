// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralInformationWizardPage.cs" company="WildGums">
//   Copyright (c) 2013 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
    using System;
    using System.Globalization;

    public class GeneralInformationWizardPage : WizardPageBase
    {
        public GeneralInformationWizardPage()
        {
            
        }

        public string Name { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public DayOfWeek FirstDayOfWeek { get; set; }
        public string ShortTimeFormat { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}