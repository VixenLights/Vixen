namespace Orc.Wizard
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Catel.Data;
    using Catel.MVVM;

    public class WizardNavigationButton : ModelBase, IWizardNavigationButton
    {
        public WizardNavigationButton()
        {
            var application = System.Windows.Application.Current;
            if (application != null)
            {
                //Style = application.TryFindResource("WizardNavigationButtonStyle") as Style;
            }
        }

        public Func<string> ContentEvaluator { get; set; }

        public string Content { get; set; }

        public Func<bool> IsVisibleEvaluator { get; set; }

        public bool IsVisible { get; set; }

        public Func<IWizardNavigationButton, Style> StyleEvaluator { get; set; }

        public Style Style { get; set; }

        public ICommand Command { get; set; }

        public void Update()
        {
            if (Command is ICatelCommand catelCommand)
            {
                catelCommand.RaiseCanExecuteChanged();
            }

            var contentEvaluator = ContentEvaluator;
            if (contentEvaluator != null)
            {
                Content = contentEvaluator();
            }

            var isVisibleEvaluator = IsVisibleEvaluator;
            if (isVisibleEvaluator != null)
            {
                IsVisible = isVisibleEvaluator();
            }

            var styleEvaluator = StyleEvaluator;
            if (styleEvaluator != null)
            {
                //Style = styleEvaluator(this);
            }
        }
    }
}
