namespace Orc.Wizard
{
    using System.Collections.Generic;
    using ControlzEx.Theming;

    public class LibraryThemeProvider : ControlzEx.Theming.LibraryThemeProvider
    {
        public LibraryThemeProvider()
            : base(true)
        {
        }

        public override void FillColorSchemeValues(Dictionary<string, string> values, RuntimeThemeColorValues colorValues)
        {            
            // Orc / Orchestra colors
            values.Add("Orc.Colors.AccentBaseColor", colorValues.AccentBaseColor.ToString());

            // Custom theming if required, see Orc.Theming for more details
        }
    }
}
