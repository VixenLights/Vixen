namespace VixenPlayer.FSeqReader
{
	/// <summary>
	/// Maintains frame data for a controller.
	/// </summary>
	public class ControllerInfo : IControllerInfo
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ControllerInfo()
		{
			FrameData = new List<byte[]>();
		}

		#endregion

		#region IControllerInfo

		/// <inheritdoc/>
		public Guid ID { get; set; }

		/// <inheritdoc/>
		public int NumberOfChannels { get; set; }

		/// <inheritdoc/>
		public List<byte[]> FrameData { get; private set; }

		#endregion
	}
}
