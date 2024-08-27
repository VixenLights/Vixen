using Vixen.Module;
using Vixen.Module.Property;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.Editor.FixturePropertyEditor.Views;

namespace VixenModules.Property.IntelligentFixture
{
	/// <summary>
	/// Maintins an intelligent fixture module.
	/// </summary>
    public class IntelligentFixtureModule : PropertyModuleInstanceBase
	{
        #region Fields

		/// <summary>
		/// Data associated with the fixture property.
		/// </summary>
        private IntelligentFixtureData _data;

        #endregion

        #region Public Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
        public override void SetDefaultValues()
		{
			// Create a default channel
			FixtureChannel channel = new FixtureChannel();
			channel.ChannelNumber = 1;
			_data.FixtureSpecification.ChannelDefinitions.Add(channel);
		}

		#endregion

		#region IHasSetup

		/// <summary>
		/// Property has a setup dialog.
		/// </summary>
		public override bool HasSetup => true;

		/// <summary>
		/// Displays the setup dialog.
		/// </summary>
		/// <returns>True if the property was modified</returns>
		public override bool Setup()
		{
			// Display the fixture property editor
			FixturePropertyEditorWindowView view = new FixturePropertyEditorWindowView(_data.FixtureSpecification);
			bool? result = view.ShowDialog();

			// If the user selected OK then...
			if (result.Value)
			{
				// Update the fixture profile
				_data.FixtureSpecification = view.GetFixtureSpecification();
			}
			
			// Return that the setup configuration was successful
			return true;		
		}

		#endregion

		#region IModuleInstance

		/// <summary>
		/// Module data associated with the fixture property.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get => _data;
			set
			{
				_data = value as IntelligentFixtureData;

				// If the fixture profile has a name then...
				if (!string.IsNullOrEmpty(_data.FixtureSpecification.Name))
				{
					// Retrieve the fixture specification manager
					IFixtureSpecificationManager fixtureSpecificationManager = FixtureSpecificationManager.Instance();

					// Attempt to find the specified fixture in the repository
					FixtureSpecification specification = fixtureSpecificationManager.FixtureSpecifications.SingleOrDefault(item => item.Name == _data.FixtureSpecification.Name);

					// If the profile was not found then...
					if (specification == null)					
					{
						// Add the fixture to the repository
						fixtureSpecificationManager.FixtureSpecifications.Add(_data.FixtureSpecification);
					}
				}
			}
		}

        #endregion

        #region Public Properties

		/// <summary>
		/// Fixture profile associated with the property.
		/// </summary>
        public FixtureSpecification FixtureSpecification
		{
			get
			{
				return _data.FixtureSpecification;
			}
			set
			{
				_data.FixtureSpecification = value;
			}
		}

		#endregion
	}
}