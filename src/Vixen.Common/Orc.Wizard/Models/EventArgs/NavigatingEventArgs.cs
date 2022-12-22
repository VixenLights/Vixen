namespace Orc.Wizard
{
    using System;

    public class NavigatingEventArgs : EventArgs
    {
        public NavigatingEventArgs(IWizardPage from, IWizardPage to)
        {
            From = from;
            To = to;
        }

        public IWizardPage From { get; }

        public IWizardPage To { get; }

        public bool Cancel { get; set; }
    }
}
