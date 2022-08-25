using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
	/// <summary>
	/// Wizard page that selects an existing fixture profile or initiates the creation of a new profile.
	/// </summary>
	public class SelectProfileWizardPage : SummaryWizardPageBase
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
		public SelectProfileWizardPage() :
            base("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/setup-display-elements/intelligent-fixture-wizard/select-profile/")
        {
            Title = "Select Profile";
            Description = "Select or create the profile for the fixture(s).";

            // Default to selecting an existing fixture
            SelectExistingProfile = true;
        }

		#endregion

		#region Public Properties

        /// <summary>
        /// Indicates the fixture is being created from an existing profile.
        /// </summary>
		public bool SelectExistingProfile { get; set; }

        /// <summary>
        /// Indicates the user wants to create a new profile.
        /// </summary>
        public bool CreateNewProfile { get; set; }

        /// <summary>
        /// Active fixture profile.
        /// </summary>
        public FixtureSpecification Fixture { get; set; }

        /// <summary>
        /// Name of the selected fixture.
        /// </summary>
        public string SelectedFixture { get; set; }

        /// <summary>
        /// Name of the fixture profile.
        /// </summary>
        public string ProfileName 
        { 
            get
			{
                if (Fixture != null)
				{
                    return Fixture.Name;    
				}
				else 
                {
                    return string.Empty;
                }
			}
            set
			{
                if (Fixture != null)
				{
                    Fixture.Name = value;
				}
			}
        }

        /// <summary>
        /// Comapany that makes the fixture.
        /// </summary>
        public string Manufacturer
        {
            get
            {
                if (Fixture != null)
                {
                    return Fixture.Manufacturer;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (Fixture != null)
                {
                    Fixture.Manufacturer = value;
                }
            }
        }

        /// <summary>
        /// User that created the profile.
        /// </summary>
        public string CreatedBy
        {
            get
            {
                if (Fixture != null)
                {
                    return Fixture.CreatedBy;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (Fixture != null)
                {
                    Fixture.CreatedBy = value;
                }
            }
        }

        /// <summary>
        /// Revision number of the profile.
        /// </summary>
        public string Revision
        {
            get
            {
                if (Fixture != null)
                {
                    return Fixture.Revision;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (Fixture != null)
                {
                    Fixture.Revision = value;
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns the summary string for the page.
        /// </summary>
        /// <returns>Summary string for the page</returns>
        protected override string GetSummaryString()
        {
            return SelectedFixture;            
        }

        #endregion
    }
}
