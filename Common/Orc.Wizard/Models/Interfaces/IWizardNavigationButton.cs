namespace Orc.Wizard
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    public interface IWizardNavigationButton
    {
        string Content { get; }
        bool IsVisible { get; }
        Style Style { get; }
        ICommand Command { get; }

        void Update();
    }
}
