using System.Runtime.Serialization;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains the rotation limits of the fixture function.
	/// </summary>
	[DataContract]
	public class FixtureRotationLimits
    {
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureRotationLimits()
        {
			StartPosition = 0;
			StopPosition = 360;
        }

		#endregion

		#region Public Propeerties

		/// <summary>
		/// Start position of the rotation in degrees.
		/// </summary>
		[DataMember]
        public int StartPosition { get; set; }

		/// <summary>
		/// Stop position of the rotation in degrees.
		/// </summary>
        [DataMember]
        public int StopPosition { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a clone of the rotation limits.
		/// </summary>
		/// <returns>Clone of the rotation limits</returns>
		public FixtureRotationLimits CreateInstanceForClone()
		{
			// Clone the rotation limits
			FixtureRotationLimits rotationLimits = new FixtureRotationLimits
			{
				StartPosition = StartPosition,
				StopPosition = StopPosition,				
			};

			return rotationLimits;
		}

		#endregion
	}
}
