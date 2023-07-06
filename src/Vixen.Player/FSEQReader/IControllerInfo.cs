namespace VixenPlayer.FSeqReader
{
	/// <summary>
	/// Maintains frame data for a controller.
	/// </summary>
	public interface IControllerInfo
	{
		/// <summary>
		/// ID of the controller.
		/// </summary>
		public Guid ID { get; set; }

		/// <summary>
		/// Number of channels associated with the controller.
		/// </summary>
		public int NumberOfChannels { get; set; }

		/// <summary>
		/// Frame data for the controller.
		/// </summary>
		public List<byte[]> FrameData { get; }
	}
}
