namespace VixenModules.Editor.FixtureWizard.Wizard.ViewModels
{
	using Catel.Collections;
	using Catel.Data;
	using Catel.Fody;
	using Catel.MVVM;
	using Orc.Wizard;
	using System.Collections.ObjectModel;
	using VixenModules.Editor.FixtureWizard.Wizard.Models;

    /// <summary>
	/// Wizard view model page for grouping and naming the fixture profiles created.
	/// </summary>
	[NoWeaving]
    public class GroupingWizardPageViewModel : WizardPageViewModelBase<GroupingWizardPage>
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wizardPage">Grouping wizard page model</param>
		public GroupingWizardPageViewModel(GroupingWizardPage wizardPage) :
            base(wizardPage)
        {                        
            // Create the add fixture command
            AddCommand = new Command(Add);
            
            // Create the remove fixture command
            SubtractCommand = new Command(Subtract, CanSubtract);

            // Default the group name to 'Fixtures'
            const string DefaultGroupName = "Fixtures";
            
            // Create the preview tree with a root
            _previewTreeWithRoot = new ObservableCollection<FixtureWizardTreeItem>();

            // Default the preview tree to having a root
            PreviewTree = _previewTreeWithRoot;

            // Create the root of the preview tree
            FixtureWizardTreeItem root = new FixtureWizardTreeItem();
            root.Name = DefaultGroupName;

            // Add the root to the preview tree
            PreviewTree.Add(root);            

            // Create the default fixture leaf
            FixtureWizardTreeItem child1 = new FixtureWizardTreeItem();
            child1.Name = "Fixture_1";

            // Add the fixture to the tree
            root.Children.Add(child1);

            // Save off the children of the tree
            // If the user selects not to have a group the children form the preview of the elements
            _children = root.Children;

            // Default to only creating one fixture
            NumberOfFixtures = 1;

            // Default the fixture prefix to 'Fixture_'
            ElementPrefix = "Fixture_";

            // Default to creating a group for the fixtures
            CreateGroup = true;

            // Default the group name to 'Fixtures'
            GroupName = DefaultGroupName;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Tree item children in the preview tree.
        /// The children are maintained separately so that when grouping is turned off the preview can show a list of fixture nodes.
        /// </summary>
        private ObservableCollection<FixtureWizardTreeItem> _children;
        
        /// <summary>
        /// Preview tree including the root.
        /// </summary>
        private ObservableCollection<FixtureWizardTreeItem> _previewTreeWithRoot;

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns true if fixtures can be removed.
        /// </summary>
        /// <returns></returns>
        private bool CanSubtract()
        {
            // Need to always create at least one fixture
            return (NumberOfFixtures > 1);
        }

        /// <summary>
        /// Adds a tree item to the preview.
        /// </summary>
        protected void Add()
        {
            // Increase the number of fixtures
            NumberOfFixtures++;

            // Create a new preview item
            FixtureWizardTreeItem item = new FixtureWizardTreeItem();

            // Initialize the nuame of the preview item
            item.Name = ElementPrefix + NumberOfFixtures.ToString();

            // If a group is being created then...
            if (CreateGroup)
            {
                // Add a new child item
                PreviewTree[0].Children.Add(item);
            }           
            else
			{
                // Otherwise just add a new item
                PreviewTree.Add(item);
            }
        }

        /// <summary>
        /// Removes a tree item from the preview.
        /// </summary>
        protected void Subtract()
        {
            // Decrease the number of fixtures
            NumberOfFixtures--;

            // If a group is being created then...
            if (CreateGroup)
            {
                // Remove the last item from the preview
                PreviewTree[0].Children.RemoveLast();
            }
            else
			{
                // Otherwise just remove an item
                PreviewTree.RemoveLast();
            }                
        }

        #endregion

        #region Public Catel Properties

        /// <summary>
        /// Preview tree of what the fixtures are going to look like in the element tree.
        /// </summary>
        public ObservableCollection<FixtureWizardTreeItem> PreviewTree
        {
            get
            {
                return GetValue<ObservableCollection<FixtureWizardTreeItem>>(PreviewTreeProperty);
            }
            set
            {
                SetValue(PreviewTreeProperty, value);
            }
        }

        /// <summary>
        /// Preview tree property data.
        /// </summary>
        public static readonly PropertyData PreviewTreeProperty = RegisterProperty(nameof(PreviewTree), typeof(ObservableCollection<FixtureWizardTreeItem>));

        /// <summary>
        /// Number of fixtures to create.
        /// </summary>
        [ViewModelToModel]
        public int NumberOfFixtures
        {
            get
            {
                return GetValue<int>(NumberOfFixturesProperty);
            }
            set
            {
                SetValue(NumberOfFixturesProperty, value);
            }
        }

        /// <summary>
        /// Number of fixtures property data.
        /// </summary>
        public static readonly PropertyData NumberOfFixturesProperty = RegisterProperty(nameof(NumberOfFixtures), typeof(int));

        /// <summary>
        /// Element prefix of the fixtures.
        /// </summary>
        [ViewModelToModel]
        public string ElementPrefix
        {
            get
            {
                return GetValue<string>(ElementPrefixProperty);
            }
            set
            {
                SetValue(ElementPrefixProperty, value);
                
                // Loop through all the preview children
                foreach(FixtureWizardTreeItem treeItem in _children)
                {
                    // Update the name of the child preview item
                    treeItem.Name = ElementPrefix + (_children.IndexOf(treeItem) + 1).ToString();
                }
            }
        }

        /// <summary>
        /// Element prefix property data.
        /// </summary>
        public static readonly PropertyData ElementPrefixProperty = RegisterProperty(nameof(ElementPrefix), typeof(string));

        /// <summary>
        /// Indicates is a group is created to contain the fixtures.
        /// </summary>
        [ViewModelToModel]
        public bool CreateGroup
        {
            get
            {
                return GetValue<bool>(CreateGroupProperty);
            }
            set
            {
                SetValue(CreateGroupProperty, value);

                // If a group is being created then...
                if (value)
                {
                    // Display the tree with a root
                    PreviewTree = _previewTreeWithRoot;
                }
                else
                {
                    // Otherwise just display the list of children
                    PreviewTree = _children;
                }
            }
        }

        /// <summary>
        /// Create group property data.
        /// </summary>
        public static readonly PropertyData CreateGroupProperty = RegisterProperty(nameof(CreateGroup), typeof(bool));

        /// <summary>
        /// Name of the fixture group.  This name is used as the root of the preview tree.
        /// </summary>
        [ViewModelToModel]
        public string GroupName
        {
            get
            {
                return GetValue<string>(GroupNameProperty);
            }
            set
            {
                SetValue(GroupNameProperty, value);
                PreviewTree[0].Name = value;
            }
        }

        /// <summary>
        /// Group name property data.
        /// </summary>
        public static readonly PropertyData GroupNameProperty = RegisterProperty(nameof(GroupName), typeof(string));

		#endregion

		#region Public Properties

		/// <summary>
		/// Command for adding a preview item.
		/// </summary>
		public Command AddCommand { get; private set; }

        /// <summary>
        /// Command for deleting a preview item.
        /// </summary>
        public Command SubtractCommand { get; private set; }

        #endregion      
    }
}
