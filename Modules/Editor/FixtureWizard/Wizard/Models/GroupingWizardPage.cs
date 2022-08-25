namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
	using System;

	/// <summary>
	/// Wizard page configures if the fixtures are part of a group and how they are named.
	/// </summary>
	public class GroupingWizardPage : SummaryWizardPageBase
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
		public GroupingWizardPage() : 
            base("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/setup-display-elements/intelligent-fixture-wizard/grouping/")
        {
            Title = "Grouping";
            Description = "Configure how many fixtures to create and if they should be grouped.";    
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns the summary string for the page.
        /// </summary>
        /// <returns>Summary string for the page</returns>
        protected override string GetSummaryString()
        {
            // Indicate the number of fixtures being created
            string summary = "# Fixtures: " + NumberOfFixtures.ToString() + Environment.NewLine;

            // If a group is being created then...
            if (CreateGroup)
            {
                // Add the group name
                summary += "Group Name: " + GroupName + Environment.NewLine;
            }

            // Add the element prefix
            summary += "Element Prefix: " + ElementPrefix;
            
            return summary;
        }

		#endregion

		#region Public Properties

        /// <summary>
        /// Number of fixture to create.
        /// </summary>
		public int NumberOfFixtures { get; set; }

        /// <summary>
        /// Prefix of the fixture elements.
        /// </summary>
        public string ElementPrefix { get; set; }

        /// <summary>
        /// Whether to create a group for the fixtures.
        /// </summary>
        public bool CreateGroup { get; set; }

        /// <summary>
        /// Optional Group name of the fixtures.
        /// </summary>
        public string GroupName { get; set; }

        #endregion
    }
}
