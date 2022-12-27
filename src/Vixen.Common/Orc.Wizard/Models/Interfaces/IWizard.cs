// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWizard.cs" company="WildGums">
//   Copyright (c) 2013 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Data;

    public interface IWizard
    {
        #region Properties
        IWizardPage CurrentPage { get; }
        IEnumerable<IWizardPage> Pages { get; }
        INavigationStrategy NavigationStrategy { get; }

        INavigationController NavigationController { get; }
        string Title { get; }
        System.Windows.ResizeMode ResizeMode { get; }
        System.Windows.Size MinSize { get; }
        System.Windows.Size MaxSize { get; }

        bool CacheViews { get; }
        bool RestoreScrollPositionPerPage { get; }
        bool HandleNavigationStates { get; }
        bool CanResume { get; }
        bool CanCancel { get; }
        bool CanMoveForward { get; }
        bool CanMoveBack { get; }
        bool IsHelpVisible { get; }
        bool CanShowHelp { get; }
        bool ShowInTaskbar { get; }
        bool AllowQuickNavigation { get; }
        bool AutoSizeSideNavigationPane { get; set; }
        #endregion

        Task CancelAsync();
        Task ResumeAsync();

        Task InitializeAsync();
        Task CloseAsync();

        Task MoveForwardAsync();
        Task MoveBackAsync();
        Task MoveToPageAsync(int indexOfNextPage);
        Task ShowHelpAsync();

        event EventHandler<EventArgs> CurrentPageChanged;
        event EventHandler<EventArgs> MovedForward;
        event EventHandler<EventArgs> MovedBack;
        event EventHandler<EventArgs> Resumed;
        event EventHandler<EventArgs> Canceled;
        event EventHandler<EventArgs> HelpShown;
        event EventHandler<NavigatingEventArgs> MovingBack;
        event EventHandler<NavigatingEventArgs> MovingForward;

        void InsertPage(int index, IWizardPage page);
        void RemovePage(IWizardPage page);

        IValidationContext GetValidationContextForCurrentPage(bool validate = true);
    }
}
