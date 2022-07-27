﻿namespace Orc.Wizard.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Fody;
    using Catel.MVVM;
    using Catel.Services;

    public class WizardViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        private readonly ILanguageService _languageService;

        private bool _isCanceling;

        #region Constructors
        public WizardViewModel(IWizard wizard, IMessageService messageService, ILanguageService languageService)
        {
            Argument.IsNotNull(() => wizard);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => languageService);

            DeferValidationUntilFirstSaveCall = true;

            Wizard = wizard;
            WizardPages = new List<IWizardPage>(wizard.Pages);
            _messageService = messageService;
            _languageService = languageService;

            ShowHelp = new TaskCommand(OnShowHelpExecuteAsync, OnShowHelpCanExecute);
        }
        #endregion

        #region Properties
        [Model(SupportIEditableObject = false)]
        [Expose(nameof(IWizard.CurrentPage))]
        [Expose(nameof(IWizard.ResizeMode))]
        [Expose(nameof(IWizard.MinSize))]
        [Expose(nameof(IWizard.MaxSize))]
        [Expose(nameof(IWizard.IsHelpVisible))]
        [Expose(nameof(IWizard.ShowInTaskbar))]
        public IWizard Wizard { get; set; }

        public IEnumerable<IWizardPage> WizardPages { get; private set; }

        public IEnumerable<IWizardNavigationButton> WizardButtons { get; private set; }

        public string PageTitle { get; private set; }

        public string PageDescription { get; private set; }

        public bool IsPageOptional { get; private set; }
        #endregion

        #region Commands
        public TaskCommand ShowHelp { get; set; }

        private bool OnShowHelpCanExecute()
        {
            return Wizard.CanShowHelp;
        }

        private Task OnShowHelpExecuteAsync()
        {
            return Wizard.ShowHelpAsync();
        }
        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Wizard.CurrentPageChanged += OnWizardCurrentPageChanged;
            Wizard.MovedBack += OnWizardMovedBack;
            Wizard.MovedForward += OnWizardMovedForward;
            Wizard.Canceled += OnWizardCanceled;
            Wizard.Resumed += OnWizardResumed;

            await Wizard.InitializeAsync();

            foreach (var page in Wizard.Pages)
            {
                await page.InitializeAsync();
            }

            UpdateState();
        }

        protected override async Task<bool> CancelAsync()
        {
            if (!_isCanceling)
            {
                // Special case, we need to execute the cancel command if users are using ALT + F4
                if (!Wizard.CanCancel)
                {
                    return false;
                }

                if (!await CancelWizardAsync())
                {
                    return false;
                }
            }

            return await base.CancelAsync();
        }

        protected override async Task CloseAsync()
        {
            Wizard.CurrentPageChanged -= OnWizardCurrentPageChanged;
            Wizard.MovedBack -= OnWizardMovedBack;
            Wizard.MovedForward -= OnWizardMovedForward;
            Wizard.Canceled -= OnWizardCanceled;
            Wizard.Resumed -= OnWizardResumed;

            WizardButtons = null;

            await Wizard.CloseAsync();

            await base.CloseAsync();
        }

        private async Task<bool> CancelWizardAsync()
        {
            using (new DisposableToken<WizardViewModel>(this, x => x.Instance._isCanceling = true, x => x.Instance._isCanceling = false))
            {
                if (await _messageService.ShowAsync(_languageService.GetString("Wizard_AreYouSureYouWantToCancelWizard"), button: MessageButton.YesNo) == MessageResult.No)
                {
                    return false;
                }

                if (!await CancelAsync())
                {
                    return false;
                }

                await Wizard.CancelAsync();
                return true;
            }
        }

        private void OnWizardCurrentPageChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void OnWizardMovedBack(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void OnWizardMovedForward(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void OnWizardCanceled(object sender, EventArgs e)
        {
            CloseViewModelAsync(false);
        }

        private void OnWizardResumed(object sender, EventArgs e)
        {
            CloseViewModelAsync(true);
        }

        private void UpdateState()
        {
            var page = Wizard.CurrentPage;

            PageTitle = page?.Title ?? string.Empty;
            PageDescription = page?.Description ?? string.Empty;
            IsPageOptional = page?.IsOptional ?? false;

            var currentIndex = Wizard.Pages.TakeWhile(wizardPage => !ReferenceEquals(wizardPage, page)).Count() + 1;
            var totalPages = Wizard.Pages.Count();

            var title = Wizard.Title;
            if (!string.IsNullOrEmpty(title))
            {
                title += " - ";
            }

            title += string.Format(_languageService.GetString("Wizard_XofY"), currentIndex, totalPages);
            Title = title;

            WizardButtons = Wizard.NavigationController.GetNavigationButtons();
        }
        #endregion
    }
}
