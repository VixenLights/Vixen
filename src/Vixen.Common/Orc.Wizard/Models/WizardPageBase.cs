﻿namespace Orc.Wizard
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Fody;
    using Catel.MVVM;
    using Catel.Threading;

    public abstract class WizardPageBase : ModelBase, IWizardPage
    {
        private IViewModel _viewModel;

        public IViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (!ObjectHelper.AreEqual(_viewModel, value))
                {
                    var oldVm = _viewModel;
                    _viewModel = value;

                    RaisePropertyChanged(nameof(ViewModel));
                    ViewModelChanged?.Invoke(this, new ViewModelChangedEventArgs(oldVm, value));
                }
            }
        }

        public virtual ISummaryItem GetSummary()
        {
            return null;
        }

        [NoWeaving]
        public IWizard Wizard { get; set; }

        public event EventHandler<ViewModelChangedEventArgs> ViewModelChanged;
        public string Title { get; set; }
        public string BreadcrumbTitle { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public bool IsOptional { get; protected set; }
        public bool IsVisited { get; set; }

        public virtual Task InitializeAsync()
        {
            return TaskHelper.Completed;
        }

        public virtual Task CancelAsync()
        {
            return TaskHelper.Completed;
        }

        public virtual Task SaveAsync()
        {
            return TaskHelper.Completed;
        }

        public virtual Task AfterWizardPagesSavedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
