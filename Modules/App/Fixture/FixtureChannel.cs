using System.Runtime.Serialization;

namespace VixenModules.App.Fixture
{
	/// <summary>
	/// Maintains metadata about a fixture channel.
	/// </summary>
    [DataContract]
	public class FixtureChannel : FixtureItem
	{
        #region Public Properties
		
		/// <summary>
		/// Function supported by the channel.
		/// </summary>
		[DataMember]
		public string Function { get; set; }

		/// <summary>
		/// Channel number.
		/// </summary>
		[DataMember]
		public int ChannelNumber { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a clone of the channel.
		/// </summary>
		/// <returns>Clone of the channel.</returns>
		public FixtureChannel CreateInstanceForClone()
		{
			// Make a copy of the channel
			FixtureChannel channel = new FixtureChannel
			{
				Name = Name,
				Function = Function,
				ChannelNumber = ChannelNumber,
			};

			return channel;
		}

		#endregion
	}
}
