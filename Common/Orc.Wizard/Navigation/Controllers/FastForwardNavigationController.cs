﻿namespace Orc.Wizard
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Catel.MVVM;
    using Catel.Services;

    public class FastForwardNavigationController : DefaultNavigationController
    {
        public FastForwardNavigationController(IWizard wizard, ILanguageService languageService, IMessageService messageService)
            : base(wizard, languageService, messageService)
        {

        }

        protected override IEnumerable<IWizardNavigationButton> CreateNavigationButtons(IWizard wizard)
        {
            var buttons = new List<WizardNavigationButton>();

            buttons.Add(CreateBackButton(wizard));

            var forwardButton = CreateForwardButton(wizard);
            forwardButton.IsVisibleEvaluator = () => true;
            buttons.Add(forwardButton);

            buttons.Add(CreateFinishButton(wizard));
            buttons.Add(CreateCancelButton(wizard));

            return buttons;
        }

        protected override WizardNavigationButton CreateFinishButton(IWizard wizard)
        {
            var button = new WizardNavigationButton
            {
                Content = _languageService.GetString("Wizard_Finish"),
                IsVisibleEvaluator = () => true,
                StyleEvaluator = (x) =>
                {
                    var styleName = wizard.IsLastPage() ? "WizardNavigationPrimaryButtonStyle" : "WizardNavigationButtonStyle";

                    var application = System.Windows.Application.Current;
                    return application?.TryFindResource(styleName) as Style;
                },
                Command = new TaskCommand(async () =>
                {
                    if (wizard.IsLastPage())
                    {
                        // Just resume
                        await wizard.ResumeAsync();
                        return;
                    }

                    var wizardPages = wizard.Pages.ToList();

                    var summaryPage = wizardPages.LastOrDefault(x => x is SummaryWizardPage) as SummaryWizardPage;
                    if (summaryPage is not null)
                    {
                        // Navigate to summary page
                        var typedWizard = wizard as WizardBase;
                        if (typedWizard is null)
                        {
                            // Manually move next
                            while (true)
                            {
                                if (wizard.IsLastPage())
                                {
                                    break;
                                }

                                if (ReferenceEquals(wizardPages, typedWizard))
                                {
                                    break;
                                }
                            }

                            return;
                        }

                        await typedWizard.MoveToPageAsync(summaryPage);
                        return;
                    }

                    // Last resort: try to resume normally
                    await wizard.ResumeAsync();
                },
                () =>
                {
                    if (!wizard.HandleNavigationStates)
                    {
                        return true;
                    }

                    //var validationSummary = this.GetValidationSummary(true);
                    //return !validationSummary.HasErrors && !validationSummary.HasWarnings && Wizard.CanResume;

                    return wizard.CanResume;
                })
            };

            return button;
        }
    }
}
