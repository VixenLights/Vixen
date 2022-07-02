using System;
using Vixen.Module.Property;

namespace VixenModules.Property.IntelligentFixture
{
	/// <summary>
	/// Descriptor for the Intelligent fixture property.
	/// </summary>
	public class IntelligentFixtureDescriptor : PropertyModuleDescriptorBase
	{
		#region Public Static Properties

		/// <summary>
		/// Module ID for the property.
		/// </summary>
		public static Guid _typeId = new Guid("{FDD74C85-1779-4521-B559-0DD5EA71054E}");

		#endregion

		#region IModuleDescriptor

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string TypeName => "Intelligent Fixture";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Guid TypeId => _typeId;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Description => "Provides ability to specify capabilities of an intelligent fixture";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Type ModuleClass => typeof(IntelligentFixtureModule);

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Author => "Vixen Team";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Version => "1.0";

        #endregion

        #region Public Properties

		/// <summary>
		/// Type of the module data class.
		/// </summary>
		public override Type ModuleDataClass => typeof (IntelligentFixtureData);

		#endregion
	}
}