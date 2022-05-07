// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewProjectWizardWindow.xaml.cs" company="WildGums">
//   Copyright (c) 2013 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Wizard.Views
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Catel.Threading;
    using Catel.Windows;
    using Catel.Windows.Threading;
    using ViewModels;

    /// <summary>
    /// Interaction logic for WizardWindow.xaml
    /// </summary>
    public partial class WizardWindow
    {
        #region Constructors
        public WizardWindow()
            : this(null)
        {
        }

        public WizardWindow(WizardViewModel viewModel)
            : base(viewModel, DataWindowMode.Custom, infoBarMessageControlGenerationMode: InfoBarMessageControlGenerationMode.Overlay)
        {
            InitializeComponent();
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            Dispatcher.BeginInvoke(() =>
            {
                UpdateOpacityMask();
            });
        }

        protected override void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnViewModelPropertyChanged(e);

#pragma warning disable WPF1014
            if (e.HasPropertyChanged(nameof(IWizard.CurrentPage)))
#pragma warning restore WPF1014
            {
#pragma warning disable AvoidAsyncVoid
                Dispatcher.BeginInvoke(async () =>
                {
#pragma warning restore AvoidAsyncVoid
                    var vm = (WizardViewModel) ViewModel;

                    breadcrumb.CenterSelectedItem();
                    breadcrumbProgress.UpdateProgress(vm.Wizard.CurrentPage.Number, vm.Wizard.Pages.Count());

                    // We need to await the animation
                    await TaskShim.Delay(WizardConfiguration.AnimationDuration);

                    UpdateOpacityMask();
                });
            }
        }

        private void UpdateOpacityMask()
        {
            var scrollViewer = breadcrumb.FindVisualDescendantByType<ScrollViewer>();
            if (scrollViewer is null)
            {
                return;
            }

            var opacityMask = new LinearGradientBrush();
            if (scrollViewer.HorizontalOffset > 0d)
            {
                opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 0d));
                opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0.05d));
            }

            var scrollableWidth = scrollViewer.ScrollableWidth;
            if (scrollableWidth > scrollViewer.HorizontalOffset)
            {
                opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0.95d));
                opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 1d));
            }

            breadcrumb.SetCurrentValue(OpacityMaskProperty, opacityMask.GradientStops.Count > 0 ? opacityMask : null);
        }
        #endregion
    }
}
