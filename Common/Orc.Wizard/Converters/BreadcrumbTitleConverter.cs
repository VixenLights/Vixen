// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BreadcrumbTitleConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard.Converters
{
    using System;
    using Catel.MVVM.Converters;

    public class BreadcrumbTitleConverter : ValueConverterBase<IWizardPage>
    {
        protected override object Convert(IWizardPage value, Type targetType, object parameter)
        {
            if (value is null)
            {
                return null;
            }

            var title = value.BreadcrumbTitle;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = value.Title;
            }

            return title;
        }
    }
}
