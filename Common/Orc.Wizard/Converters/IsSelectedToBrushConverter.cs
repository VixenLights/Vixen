// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardPageToBrushConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard.Converters
{
    using System;
    using System.Windows.Media;
    using Catel.MVVM.Converters;

    public class IsSelectedToBrushConverter : ValueConverterBase<bool>
    {
        private static readonly Brush SelectedBrush;
        private static readonly Brush NotSelectedBrush;

        static IsSelectedToBrushConverter()
        {
            var application = System.Windows.Application.Current;
            if (application is not null)
            {
                SelectedBrush = application.FindResource(ThemingKeys.AccentColorBrush) as Brush;
                NotSelectedBrush = application.FindResource(ThemingKeys.AccentColorBrush40) as Brush;
            }
        }

        protected override object Convert(bool value, Type targetType, object parameter)
        {
            return value ? SelectedBrush : NotSelectedBrush;
        }
    }
}
