using Catel.MVVM;
using System.Collections.ObjectModel;

namespace VixenModules.Editor.FixtureWizard.Wizard.ViewModels
{
    /// <summary>
    /// View model for displaying a preview of how the fixtures are going to look like in the element tree.
    /// </summary>
	public class FixtureWizardTreeItem : ViewModelBase
    {
		#region Constructor 
		
        /// <summary>
        /// Constructor
        /// </summary>
        public FixtureWizardTreeItem()
        {
            // Create the collection of children tree items
            Children = new ObservableCollection<FixtureWizardTreeItem>();            
        }

		#endregion

		#region Public Properties

        /// <summary>
        /// Determines the visibility of the green dot.
        /// Only leaves display a green dot.
        /// </summary>
		public bool Visible 
        {
            get
            {
                // Only display the green dot for leaves
                return (Children.Count == 0);
            }            
        }

        /// <summary>
        /// Collection of children.
        /// </summary>
        public ObservableCollection<FixtureWizardTreeItem> Children { get; }

        /// <summary>
        /// Name of the tree item.  This is the text displayed in the preview.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}
