namespace Orc.Wizard.Controls
{
	using System.Linq;
	using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Catel.Collections;
    using Orc.Wizard;

    public partial class SideNavigationBreadcrumbItem
    {
        // These constants and static margins exist to keep all the sizing logic in one spot.
        public const double EllipseDiameter = 26;
        public const int NavigationGridYMargin = 5;
        public const int NavigationItemLineLengthDefault = 48;
        public const int NavigationItemBottomMarginDefault = 56;
        public const int NavigationItemLineTopDefault = 35;

        public static readonly Thickness NavigationItemMarginDefault = new Thickness(0, 0, 0, NavigationItemBottomMarginDefault);
        public static readonly Thickness CanvasLineMargin = new Thickness(2, 2, 2, 2);
        public static readonly Thickness EllipseMargin = new Thickness(15, NavigationGridYMargin, 25, NavigationGridYMargin);

        // This value is from SideNavigationWizardWindow.xaml
        private const int ParentMarginTop = 12;

        public SideNavigationBreadcrumbItem()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        public IWizardPage Page
        {
            get { return (IWizardPage)GetValue(PageProperty); }
            set { SetValue(PageProperty, value); }
        }

        public static readonly DependencyProperty PageProperty = DependencyProperty.Register(nameof(Page), typeof(IWizardPage),
            typeof(SideNavigationBreadcrumbItem), new PropertyMetadata(null, (sender, e) => ((SideNavigationBreadcrumbItem)sender).OnPageChanged()));


        public IWizardPage CurrentPage
        {
            get { return (IWizardPage)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(nameof(CurrentPage), typeof(IWizardPage),
            typeof(SideNavigationBreadcrumbItem), new PropertyMetadata(null, (sender, e) => ((SideNavigationBreadcrumbItem)sender).OnCurrentPageChanged()));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string),
            typeof(SideNavigationBreadcrumbItem), new PropertyMetadata(string.Empty));


        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string),
            typeof(SideNavigationBreadcrumbItem), new PropertyMetadata(string.Empty));


        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(nameof(Number), typeof(int),
            typeof(SideNavigationBreadcrumbItem), new PropertyMetadata(0));

        public int NavigationItemLineLength
        {
            get { return (int)GetValue(NavigationItemLineLengthProperty); }
            set { SetValue(NavigationItemLineLengthProperty, value); }
        }

        public static readonly DependencyProperty NavigationItemLineLengthProperty = DependencyProperty.Register(nameof(NavigationItemLineLength), typeof(int),
            typeof(SideNavigationBreadcrumbItem), new PropertyMetadata(NavigationItemLineLengthDefault));

        public int NavigationItemLineTop
        {
            get { return (int)GetValue(NavigationItemLineTopProperty); }
            set { SetValue(NavigationItemLineTopProperty, value); }
        }

        public static readonly DependencyProperty NavigationItemLineTopProperty = DependencyProperty.Register(nameof(NavigationItemLineTop), typeof(int),
            typeof(SideNavigationBreadcrumbItem), new PropertyMetadata(NavigationItemLineTopDefault));

        public Thickness NavigationItemMargin
        {
            get { return (Thickness)GetValue(NavigationItemMarginProperty); }
            set { SetValue(NavigationItemMarginProperty, value); }
        }

        public static readonly DependencyProperty NavigationItemMarginProperty = DependencyProperty.Register(nameof(NavigationItemMargin), typeof(Thickness),
            typeof(SideNavigationBreadcrumbItem), new PropertyMetadata(NavigationItemMarginDefault));


        private void OnPageChanged()
        {
            var page = Page;
            if (page != null)
            {                
                SetCurrentValue(NumberProperty, page.Number);
                SetCurrentValue(TitleProperty, page.BreadcrumbTitle ?? page.Title);
                SetCurrentValue(DescriptionProperty, page.Description);

                pathline.SetCurrentValue(VisibilityProperty, page.Wizard.IsLastPage(page) ? Visibility.Collapsed : Visibility.Visible);
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Page.Wizard.AutoSizeSideNavigationPane)
            {
                AutoSizeNavigationPane();
            }
        }

        private void AutoSizeNavigationPane()
		{            
            // Determine the height required for the ellipse and its margins
            const int EllipseHeightAndMargin = (int)EllipseDiameter + (2 * NavigationGridYMargin);

            // Calculate the space required for all the navigation bubbles
            var totalSpaceNeeded = EllipseHeightAndMargin * Page.Wizard.Pages.Count(); 

            var TitleBarHeight = (int)SystemParameters.WindowCaptionHeight;

            // Using the Wizard Minimum Height calculate the extra vertical space
            // Note the TitleBarHeight is not really reliable so doubling should give us enough margin
            // to avoid the navigation pane needing to scroll.
            var spaceAvailable = (int)Page.Wizard.MinSize.Height - 2 * TitleBarHeight - ParentMarginTop; 
            
            // Determine the space left over
            var spaceLeftOver = spaceAvailable - totalSpaceNeeded;

            // Divide the space left over by the number of wizard pages to determine the margin of the navigation pane grid
            var gridMargin = spaceLeftOver / (Page.Wizard.Pages.Count());

            // If the grid margin exceeds the default then...
            if (gridMargin > NavigationItemBottomMarginDefault)
			{
                // Reset the grid margin back to the default
                gridMargin = NavigationItemBottomMarginDefault;
            }

            // Determine the height of one bubble and the associated line
            var navigationPaneHeight = gridMargin + EllipseHeightAndMargin;    
            
            // Calculate the margin for the grid
            SetCurrentValue(NavigationItemMarginProperty, new Thickness(0, 0, 0, gridMargin));

            // Arbitrary margin between the line and the ellipse
            const int LineMargin = 4;

            // Calculate the navigation pane line length
            SetCurrentValue(NavigationItemLineLengthProperty, navigationPaneHeight - (int)EllipseDiameter - 2 * LineMargin); 
            
            // If the line length exceeds the default then...
            if (NavigationItemLineLength > NavigationItemLineLengthDefault)
			{
                // Reset the line length back to the default
                SetCurrentValue(NavigationItemLineLengthProperty, NavigationItemLineLengthDefault);

                // Reset the start point back to the default
                SetCurrentValue(NavigationItemLineTopProperty, NavigationItemLineTopDefault);
            }
            else
			{
                // Determine the top of the navigation line
                SetCurrentValue(NavigationItemLineTopProperty, (int)EllipseDiameter + NavigationGridYMargin + LineMargin - (int)CanvasLineMargin.Top);
            }            
        }

        private void OnCurrentPageChanged()
        {
            var isSelected = ReferenceEquals(CurrentPage, Page);
            var isCompleted = Page.Number < CurrentPage.Number;
            var isVisited = Page.IsVisited;

            SetCurrentValue(CursorProperty, (Page.Wizard.AllowQuickNavigation && isVisited) ? System.Windows.Input.Cursors.Hand : null);
            UpdateContent(isCompleted);
            UpdateSelection(isSelected, isCompleted, isVisited);
        }

        private void UpdateSelection(bool isSelected, bool isCompleted, bool isVisited)
        {
            UpdateShapeColor(pathline, isCompleted && !isSelected);
            UpdateShapeColor(ellipse, isSelected || isVisited);

            txtTitle.SetCurrentValue(System.Windows.Controls.TextBlock.ForegroundProperty, isSelected ?
                TryFindResource("Orc.Brushes.Black") : (isVisited ?
                TryFindResource("Orc.Brushes.GrayBrush2") :
                TryFindResource("Orc.Brushes.GrayBrush1")));
        }

        private void UpdateContent(bool isCompleted)
        {
            ellipseText.SetCurrentValue(VisibilityProperty, isCompleted ? Visibility.Hidden : Visibility.Visible);
            ellipseCheck.SetCurrentValue(VisibilityProperty, isCompleted ? Visibility.Visible : Visibility.Hidden);
        }

        private void UpdateShapeColor(Shape shape, bool isSelected)
        {
            var storyboard = new Storyboard();

            if (shape != null && shape.Fill is null)
            {
#pragma warning disable WPF0041 // Set mutable dependency properties using SetCurrentValue.
                shape.Fill = (SolidColorBrush)TryFindResource(ThemingKeys.AccentColorBrush40);
#pragma warning restore WPF0041 // Set mutable dependency properties using SetCurrentValue.
            }

            var fromColor = ((SolidColorBrush)shape?.Fill)?.Color ?? Colors.Transparent;
            var targetColor = this.GetAccentColorBrush(isSelected).Color;

            var colorAnimation = new ColorAnimation(fromColor, (Color)targetColor, WizardConfiguration.AnimationDuration);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Fill.(SolidColorBrush.Color)", ArrayShim.Empty<object>()));

            storyboard.Children.Add(colorAnimation);

            storyboard.Begin(shape);
        }
	}
}
