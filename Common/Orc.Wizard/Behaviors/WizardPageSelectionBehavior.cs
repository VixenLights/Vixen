// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardPageSelectionBehavior.cs" company="WildGums">
//   Copyright (c) 2013 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.MVVM.Views;
    using Catel.Windows;
    using Catel.Windows.Interactivity;

    public class WizardPageSelectionBehavior : BehaviorBase<ContentControl>
    {
        private readonly ConditionalWeakTable<object, ScrollInfo> _scrollPositions = new ConditionalWeakTable<object, ScrollInfo>();
        private readonly ConditionalWeakTable<object, CachedView> _cachedViews = new ConditionalWeakTable<object, CachedView>();

        private ScrollViewer _scrollViewer;
        private IWizardPage _lastPage;

        #region Properties
        public IWizard Wizard
        {
            get { return (IWizard)GetValue(WizardProperty); }
            set { SetValue(WizardProperty, value); }
        }

        public static readonly DependencyProperty WizardProperty = DependencyProperty.Register(nameof(Wizard), typeof(IWizard),
            typeof(WizardPageSelectionBehavior), new PropertyMetadata(OnWizardChanged));

        private bool CacheViews
        {
            get
            {
                return Wizard?.CacheViews ?? true;
            }
        }
        #endregion

        private static void OnWizardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as WizardPageSelectionBehavior;
            if (behavior != null)
            {
                behavior.UpdatePage();

                var oldWizard = e.OldValue as IWizard;
                if (oldWizard != null)
                {
                    oldWizard.CurrentPageChanged -= behavior.OnCurrentPageChanged;
                    oldWizard.MovedBack -= behavior.OnMovedBack;
                    oldWizard.MovedForward -= behavior.OnMovedForward;
                }

                var wizard = e.NewValue as IWizard;
                if (wizard != null)
                {
                    wizard.CurrentPageChanged += behavior.OnCurrentPageChanged;
                    wizard.MovedBack += behavior.OnMovedBack;
                    wizard.MovedForward += behavior.OnMovedForward;
                }
            }
        }

        protected override void OnAssociatedObjectLoaded()
        {
            _scrollViewer = AssociatedObject?.FindLogicalOrVisualAncestorByType<ScrollViewer>();

            UpdatePage();
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            base.OnAssociatedObjectUnloaded();

            var wizard = Wizard;
            if (wizard is null)
            {
                return;
            }

            wizard.CurrentPageChanged -= OnCurrentPageChanged;
            wizard.MovedBack -= OnMovedBack;
            wizard.MovedForward -= OnMovedForward;

            SetCurrentValue(WizardProperty, null);
        }

        private void OnCurrentPageChanged(object sender, EventArgs e)
        {
            UpdatePage();
        }

        private void OnMovedForward(object sender, EventArgs e)
        {
            UpdatePage();
        }

        private void OnMovedBack(object sender, EventArgs e)
        {
            UpdatePage();
        }

#pragma warning disable WPF0005 // Name of PropertyChangedCallback should match registered name.
        private void UpdatePage()
#pragma warning restore WPF0005 // Name of PropertyChangedCallback should match registered name.
        {
            if (AssociatedObject is null)
            {
                return;
            }

            var wizard = Wizard;
            if (wizard is null)
            {
                return;
            }

            var lastPage = _lastPage;
            if (lastPage != null)
            {
                if (ReferenceEquals(lastPage, wizard.CurrentPage))
                {
                    // Nothing has really changed
                    return;
                }

                ScrollInfo dontCare = null;
                if (_scrollPositions.TryGetValue(lastPage, out dontCare))
                {
                    _scrollPositions.Remove(lastPage);
                }
                
                _scrollPositions.Add(lastPage, new ScrollInfo
                {
                    VerticalOffset = _scrollViewer.VerticalOffset,
                    HorizontalOffset = _scrollViewer.HorizontalOffset
                });

                // Even though we cache views, we need to re-use the vm's since the view models will be closed when moving next
                //_lastPage.ViewModel = null;

                if (CacheViews)
                {
                    CachedView dontCareView = null;
                    if (_cachedViews.TryGetValue(lastPage, out dontCareView))
                    { 
                        _cachedViews.Remove(lastPage);
                    }

                    _cachedViews.Add(lastPage, new CachedView
                    {
                        View = AssociatedObject.Content as IView
                    });
                }

                _lastPage = null;
            }

            _lastPage = wizard.CurrentPage;

            var dependencyResolver = this.GetDependencyResolver();
            var viewModelLocator = dependencyResolver.Resolve<IWizardPageViewModelLocator>();
            var pageViewModelType = viewModelLocator.ResolveViewModel(_lastPage.GetType());

            var viewLocator = dependencyResolver.Resolve<IViewLocator>();
            var viewType = viewLocator.ResolveView(pageViewModelType);

            var typeFactory = dependencyResolver.Resolve<ITypeFactory>();

            IView view = null;

            if (_cachedViews.TryGetValue(_lastPage, out var cachedView))
            {
                view = cachedView.View;
            }

            if (view is null)
            {
                view = typeFactory.CreateInstance(viewType) as IView;
                if (view is null)
                {
                    return;
                }
            }

            // For now always recreate a vm since it could be closed (and we really don't want to mess with the lifetime of a view)
            //var viewModel = view.DataContext as IViewModel;
            IViewModel viewModel = null;
            if (viewModel is null)
            {
                var viewModelFactory = dependencyResolver.Resolve<IViewModelFactory>();
                viewModel = viewModelFactory.CreateViewModel(pageViewModelType, wizard.CurrentPage, null);

                view.DataContext = viewModel;
            }

            _lastPage.ViewModel = viewModel;

            AssociatedObject.SetCurrentValue(ContentControl.ContentProperty, view);

            var verticalScrollViewerOffset = 0d;
            var horizontalScrollViewerOffset = 0d;

            if (_scrollPositions.TryGetValue(_lastPage, out var scrollInfo))
            {
                verticalScrollViewerOffset = scrollInfo.VerticalOffset;
                horizontalScrollViewerOffset = scrollInfo.HorizontalOffset;
            }

            var scrollViewer = _scrollViewer;
                if (scrollViewer != null &&
                (Wizard?.RestoreScrollPositionPerPage ?? true))
            {
                scrollViewer.ScrollToVerticalOffset(verticalScrollViewerOffset);
                scrollViewer.ScrollToHorizontalOffset(horizontalScrollViewerOffset);
            }
        }

        private class ScrollInfo
        {
            public double VerticalOffset { get; set; }

            public double HorizontalOffset { get; set; }
        }

        private class CachedView
        {
            public IView View { get; set; }
        }
    }
}
