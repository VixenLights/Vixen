using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.Fixture;

namespace VixenModules.Property.IntelligentFixture
{
    /// <summary>
    /// Intelligent fixture property data module.
    /// </summary>
    [DataContract]
	public class IntelligentFixtureData : ModuleDataModelBase
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public IntelligentFixtureData()
		{
			// Create a new empty fixture profile
			FixtureSpecification = new FixtureSpecification();
			FixtureSpecification.InitializeBuiltInFunctions();			
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Clones the intelligent fixture property.
        /// </summary>
        /// <returns>Clone of the intelligent fixture property</returns>
        public override IModuleDataModel Clone()
		{
			// Create a new intelligent fixture data module
			IntelligentFixtureData clone = new IntelligentFixtureData();

			// Clone the fixture profile
			clone.FixtureSpecification = FixtureSpecification.CreateInstanceForClone();

			// Return the clone fixture property data module
			return clone;
		}

        #endregion

        #region Public Properties

		/// <summary>
		/// Fixture specification or profile associated with the property.
		/// </summary>
        [DataMember]
		public FixtureSpecification FixtureSpecification
		{
			get;
			set;
		}

		#endregion			
	}
}