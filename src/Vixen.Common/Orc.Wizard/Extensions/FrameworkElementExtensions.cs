namespace Orc.Wizard
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Catel;
    using Catel.Logging;

    internal static class FrameworkElementExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static SolidColorBrush GetAccentColorBrush(this FrameworkElement frameworkElement, bool isSelected = true)
        {
            Argument.IsNotNull(() => frameworkElement);

            var resourceName = isSelected ? ThemingKeys.AccentColorBrush : ThemingKeys.AccentColorBrush40;

            var brush = frameworkElement.TryFindResource(resourceName) as SolidColorBrush;
            if (brush is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Theming is not yet initialized, make sure to initialize a theme via ThemeManager first");
            }

            return brush;
        }
    }
}
