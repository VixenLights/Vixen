using System.IO;

namespace VixenPlayer.FSeqReader
{
	/// <summary>
	/// Reads an fseq file.
	/// </summary>
	public interface IFSeqReader
	{
		/// <summary>
		/// Reads a Vixen Player fseq file.
		/// </summary>
		/// <param name="reader">Binary file reader</param>
		/// <param name="onlyReadHeader">Flag to only read the header information</param>
		/// <param name="skipReadingData">Flag to skip reading the frame data</param>
		void ReadFileHeader(BinaryReader reader, bool onlyReadHeader = false, bool skipReadingData = false);

		/// <summary>
		/// File identifier (usually 'PSEQ').
		/// </summary>
		string FileIdentifier { get; }
		
		/// <summary>
		/// Version of the fseq file.
		/// </summary>
		string Version { get; }

		/// <summary>
		/// Number of channels per frame of data.
		/// </summary>
		UInt32 ChannelsPerFrame { get; }

		/// <summary>
		/// Step time or refresh rate in milliseconds.
		/// </summary>
		int StepTime { get; }

		/// <summary>
		/// True when compression is enabled.
		/// </summary>
		bool CompressionEnabled { get; }

		/// <summary>
		/// Number of data frames in the file.
		/// </summary>
		UInt32 NumberOfFrames { get; }

		/// <summary>
		/// Frame data contained in the file.
		/// </summary>
		List<Byte[]> FrameData { get; }

		/// <summary>
		/// Sequence audio file name.
		/// </summary>
		string SequenceAudioFileName { get; set; }

		// NOTE: The following properties are from the Vixen Player extended data variable header.

		/// <summary>
		/// Version of the Vixen Player extended data variable length header.
		/// </summary>
		string VixenPlayerHeaderVersion { get; }

		/// <summary>
		/// Controller frame data contained in the file.
		/// </summary>
		List<IControllerInfo> ControllerInfo { get; }

		/// <summary>
		/// Time stamp of the sequence used to make the file.
		/// </summary>
		DateTime SequenceTimeStamp { get; set; }
	}
}
