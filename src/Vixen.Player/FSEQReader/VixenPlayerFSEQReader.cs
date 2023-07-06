using System.Diagnostics;
using System.IO;
using System.IO.Compression;

using Zstandard.Net;

namespace VixenPlayer.FSeqReader
{
	/// <summary>
	/// Reads a Vixen Player fseq file.
	/// </summary>
	public class VixenPlayerFSEQReader : IFSeqReader
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public VixenPlayerFSEQReader()
		{
			ControllerInfo = new List<IControllerInfo>();
			FrameData = new List<byte[]>();
		}

		#endregion

		#region Fields

		/// <summary>
		/// Block contained in the fseq file.
		/// </summary>
		private readonly List<Tuple<uint, uint>> _blocks = new List<Tuple<uint, uint>>();

		/// <summary>
		/// Flag to keep track if we found the Vixen Player extended data.
		/// </summary>
		private bool _vixenPlayerExtendedDataFound;

		/// <summary>
		/// Offset to the Vixen Player extended data.
		/// </summary>
		private ulong _offsetToExtendedDataHeader;

		#endregion

		#region IFSeqReader

		/// <inheritdoc/>
		public string FileIdentifier { get; private set; }

		/// <inheritdoc/>
		public string Version { get; private set; }

		/// <inheritdoc/>
		public uint ChannelsPerFrame { get; private set; }

		/// <inheritdoc/>
		public int StepTime { get; private set; }

		/// <inheritdoc/>
		public bool CompressionEnabled { get; private set; }

		/// <inheritdoc/>
		public uint NumberOfFrames { get; private set; }

		/// <inheritdoc/>
		public List<byte[]> FrameData { get; private set; }

		/// <inheritdoc/>
		public string SequenceAudioFileName { get; set; }

		/// <inheritdoc/>
		public List<IControllerInfo> ControllerInfo { get; private set; }

		/// <inheritdoc/>
		public DateTime SequenceTimeStamp { get; set; }

		/// <inheritdoc/>
		public string VixenPlayerHeaderVersion { get; private set; }

		/// <inheritdoc/>
		public void ReadFileHeader(BinaryReader reader, bool onlyReadHeader = false, bool readFrameData = true)
		{
			// Header Information
			
			// Read Format Identifier
			// Byte 0
			char formatIdentifier1 = reader.ReadChar(); 
			Debug.Assert(formatIdentifier1 == 'P');
			
			// Byte 1
			char formatIdentifier2 = reader.ReadChar(); 
			Debug.Assert(formatIdentifier2 == 'S');
			
			// Byte 2
			char formatIdentifier3 = reader.ReadChar();
			Debug.Assert(formatIdentifier3 == 'E');
			
			// Byte 3
			char formatIdentifier4 = reader.ReadChar();
			Debug.Assert(formatIdentifier4 == 'Q');

			// Format the file identifier
			char[] formatIndentifier = { formatIdentifier1, formatIdentifier2, formatIdentifier3, formatIdentifier4 };
			FileIdentifier = new string(formatIndentifier);

			// Byte 4,5 - Offset to start of channel data
			uint offsetToChannelData = reader.ReadUInt16();
		
			// Byte 6 - Minor Version
			byte vMinor = reader.ReadByte();
			
			// Byte 7 - Major Version
			byte vMajor = reader.ReadByte();
			
			// Format the Version Number
			Version = vMajor + "." + vMinor;

			// Byte 8, 9 - Index offset to variable header 
			UInt16 offsetToVariableHeader = reader.ReadUInt16();

			// Bytes 10,11,12,13 - Channels per frame 4 Byte value 
			ChannelsPerFrame = reader.ReadUInt32();

			// Bytes 14,15,16,17 - Number of Frames 
			NumberOfFrames = reader.ReadUInt32();

			// Byte 18 - Step time in milliseconds
			StepTime = reader.ReadByte();

			// Byte 19 - bit flags/reserved should be 0  
			byte flagsReserved = reader.ReadByte();

			// Byte 20 - Compression type 0 for uncompressed, 1 for zstd, 2 for libz/gzip
			byte byte20  = reader.ReadByte();
			CompressionEnabled = (byte20 & 0x70) != 0;

			// Byte 21 - Number of compression blocks, 0 if uncompressed 
			byte compressionBlocksUppper = (byte)(byte20 & 0x0F);
			byte compressionBlocks = reader.ReadByte();
			int compressionBlcks = compressionBlocks + (compressionBlocksUppper << 8);

			// Byte 22 - Number of sparse ranges, 0  if none 
			byte numberSparseRanges = reader.ReadByte();

			// bit flags/reserved, unused right now, should be 0  Byte 23
			byte unused = reader.ReadByte();

			// 64bit unique identifier, clock tic count Bytes 24-31 
			long id = reader.ReadInt64();

			// If NOT only reading the fixed size header then...
			if (!onlyReadHeader)
			{
				// Read the block information
				ReadBlockInformation(compressionBlocks, reader);

				// Skip to the variable length headers
				//var stream = reader.BaseStream;
				//long bytesToSkip = offsetToVariableHeader - stream.Position;
				//SkipBytes((uint)bytesToSkip, reader);

				// Read variable length headers
				ReadVariableHeaders(reader, offsetToChannelData);

				// If reading the frame data then...
				if (readFrameData)
				{
					// If compression is enabled then...
					if (CompressionEnabled)
					{
						// Read the compressed frame data
						ReadCompressedFrameData(reader);
					}
					else
					{
						// Read the uncompressed frame data
						ReadUncompressedFrameData(reader);
					}

					// If the Vixen Player extended data variable header was found then...
					if (_vixenPlayerExtendedDataFound)
					{
						// Seek to the Vixen Player extended frame data
						reader.BaseStream.Seek((long)_offsetToExtendedDataHeader, SeekOrigin.Begin);

						// Read the Vixen Player extended data variable header
						ReadVixenPlayerExtendedDataVariableHeader(reader);
					}
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Reads the Vixen Player extended data header.
		/// </summary>
		/// <param name="reader">Binary reader to read from</param>
		private void ReadVixenPlayerExtendedDataVariableHeader(BinaryReader reader)
		{
			// Read the Header Version
			VixenPlayerHeaderVersion = reader.ReadString();

			// Read the controller count
			int controllerCount = reader.ReadInt32();

			// Loop over the controllers 
			for (int index = 0; index < controllerCount; index++)
			{
				// Read the GUID length
				char guidLength = reader.ReadChar();

				// Read the controller ID
				string controllerID = new string(reader.ReadChars(36));

				// Create a ControllerInfo object
				ControllerInfo controller = new ControllerInfo();
				controller.ID = new Guid(controllerID);

				// Store off the number of channels the controller supports
				controller.NumberOfChannels = reader.ReadInt32();

				// Store off the controller info
				ControllerInfo.Add(controller);
			}

			// Loop over the frames
			for (int frame = 0; frame < NumberOfFrames; frame++)
			{
				// Keep track of where we are in the frame
				int indexIntoFrame = 0;

				// Loop over the controllers
				foreach (ControllerInfo controller in ControllerInfo)
				{
					// Retrieve the controller's slice of frame data
					controller.FrameData.Add(
						FrameData[frame].Skip(indexIntoFrame).Take(controller.NumberOfChannels).ToArray());

					// Update the index into the frame data
					indexIntoFrame += controller.NumberOfChannels;
				}
			}
			
			// Read sequence time stamp
			SequenceTimeStamp = DateTime.FromBinary(reader.ReadInt64());
		}

		/// <summary>
		/// Reads uncompressed frame channel data.
		/// </summary>
		/// <param name="reader">Binary reader to read from</param>
		private void ReadUncompressedFrameData(BinaryReader reader)
		{
			// Loop over the number of expected frames
			for (int frame = 0; frame < NumberOfFrames; frame++)
			{
				// Read a frame of data
				byte[] frameData = reader.ReadBytes((int)ChannelsPerFrame);

				// Save off the frame of data
				FrameData.Add(frameData);
			}
		}

		/// <summary>
		/// Reads compressed frame channel data.
		/// </summary>
		/// <param name="reader">Binary reader to read from</param>
		private void ReadCompressedFrameData(BinaryReader reader)
		{
			// Create a collection to hold channel data
			List<byte> channelData = new List<byte>();

			// Loop over the compressed blocks
			foreach (Tuple<uint, uint> compressedBlock in _blocks)
			{
				// Determine the block size
				uint blockSize = compressedBlock.Item2;

				// Read the block data
				byte[] blockData = reader.ReadBytes((int)blockSize);

				// Uncompress the block
				byte[] decompressedData = Decompress(blockData);

				// Add the decompressed channel data to the collection
				channelData.AddRange(decompressedData);
			}

			// Convert the uncompressed channel data collection into an array and wrap with a memory stream
			using (MemoryStream memStream = new MemoryStream(channelData.ToArray()))
			{
				// Create a binary reader using the memory stream as the data source
				using (BinaryReader memReader = new BinaryReader(memStream))
				{
					// Break the channel data up into frames
					ReadUncompressedFrameData(reader);
				}
			}
		}

		/// <summary>
		/// Reads the variable sized headers.
		/// </summary>
		/// <param name="reader">Binary reader to read from</param>
		/// <param name="offsetToChannelData">Offset to the channel frame data from the beginning of the file</param>
		private void ReadVariableHeaders(BinaryReader reader, uint offsetToChannelData)
		{
			// Determine the number of bytes until the channel data
			Stream stream = reader.BaseStream;
			long offsetToDataInBytes = offsetToChannelData - stream.Position;

			// While there are more variable spaced headers to read loop
			while (offsetToDataInBytes > 0)
			{
				// Read the format identifier of the header
				char formatIdentifier1 = reader.ReadChar();
				char formatIdentifier2 = reader.ReadChar();
				char formatIdentifier3 = reader.ReadChar();
				char formatIdentifier4 = reader.ReadChar();

				// If the header is the sequence producer then...
				if (formatIdentifier3 == 's' && formatIdentifier4 == 'p')
				{
					string sequenceProducer = ReadSequenceProducer(reader);
				}
				// If the header is the sequence audio file then...
				else if (formatIdentifier3 == 'm' && formatIdentifier4 == 'f')
				{
					SequenceAudioFileName = ReadSequenceAudioFileName(reader);
				}
				// If the header is extended data header then...
				else if (formatIdentifier3 == 'E' && formatIdentifier4 == 'D')
				{
					// Read the extended data format identifier
					char extendedDataFormatIdentifier1 = reader.ReadChar();
					char extendedDataformatIdentifier2 = reader.ReadChar();

					// If the header is the Vixen Player header then...
					if (extendedDataFormatIdentifier1 == 'V' &&
					    extendedDataformatIdentifier2 == 'P')
					{
						// Remember that we found the Vixen Player header
						_vixenPlayerExtendedDataFound = true;
					}
					else
					{
						throw new Exception("Unsupported Extended Data Header");
					}

					// Read the offset to the extended data
					byte[] offsetToHeader = new byte[8];
					offsetToHeader[0] = reader.ReadByte();
					offsetToHeader[1] = reader.ReadByte();
					offsetToHeader[2] = reader.ReadByte();
					offsetToHeader[3] = reader.ReadByte();
					offsetToHeader[4] = reader.ReadByte();
					offsetToHeader[5] = reader.ReadByte();
					offsetToHeader[6] = reader.ReadByte();
					offsetToHeader[7] = reader.ReadByte();

					// Convert the byte array to a ulong
					_offsetToExtendedDataHeader = BitConverter.ToUInt64(offsetToHeader);

					// Read the length of the extended data
					byte[] lengthOfExtendedData = new byte[4];
					lengthOfExtendedData[0] = reader.ReadByte();
					lengthOfExtendedData[1] = reader.ReadByte();
					lengthOfExtendedData[2] = reader.ReadByte();
					lengthOfExtendedData[3] = reader.ReadByte();

					// Convert the byte array to a uint
					uint lengthOfExtData = BitConverter.ToUInt32(lengthOfExtendedData);
				}
				else
				{
					throw new Exception("Unsupported Variable Spaced Header");
				}

				// Calculate the remaining variable header data
				offsetToDataInBytes = offsetToChannelData - stream.Position;
			}
		}

		/// <summary>
		/// Skips the specified number of bytes by reading them.
		/// </summary>
		/// <param name="skipBytes">Number of bytes to skip</param>
		/// <param name="reader">Binary read to read from</param>
		private void SkipBytes(uint skipBytes, BinaryReader reader)
		{
			// Loop over the number bytes to skip
			for (int skipIndex = 0; skipIndex < skipBytes; skipIndex++)
			{
				// Advance to the next byte
				reader.ReadByte();
			}
		}

		/// <summary>
		/// Reads the block information from the specified binary reader.
		/// </summary>
		/// <param name="compressionBlocks">Number of block to read</param>
		/// <param name="reader">Binary reader to read the block information from</param>
		private void ReadBlockInformation(int compressionBlocks, BinaryReader reader)
		{
			// Loop over the compression blocks
			for (int index = 0; index < compressionBlocks; index++)
			{
				// Read the frame number of the block
				uint frameNumber = reader.ReadUInt32();

				// Read the length of the block
				uint lengthOfBlock = reader.ReadUInt32();

				// Add the blocking information to the collection
				_blocks.Add(new Tuple<uint, uint>(frameNumber, lengthOfBlock));
			}
		}

		/// <summary>
		/// Reads the sequence producer from the specified binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to read the sequence producer from</param>
		/// <returns>Sequence Producer</returns>
		private string ReadSequenceProducer(BinaryReader reader)
		{
			// Return the sequence producer
			return ReadNullTerminatedString(reader);
		}

		/// <summary>
		/// Reads the sequence audio file name from the specified binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to read the file name from</param>
		/// <returns>Audio file name associated with the sequence</returns>
		private string ReadSequenceAudioFileName(BinaryReader reader)
		{
			// Read the audio file name
			return ReadNullTerminatedString(reader);
		}

		/// <summary>
		/// Reads the null terminated string from the specified binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to read the string from</param>
		/// <returns>Read string</returns>
		private string ReadNullTerminatedString(BinaryReader reader)
		{
			// Default return value string to empty
			string returnValue = string.Empty;

			// Read the first character
			char nextChar = reader.ReadChar();

			// Continue reading characters until a null character is found
			while (nextChar != '\0')
			{
				// Add the characater to the return value
				returnValue += nextChar;

				// Read the next character
				nextChar = reader.ReadChar();
			}

			return returnValue;
		}

		/// <summary>
		/// Decompresses the specified frame data.
		/// </summary>
		/// <param name="data">Compressed data to uncompress</param>
		/// <returns>Uncompressed data</returns>
		private byte[] Decompress(byte[] data)
		{
			using (MemoryStream memoryStream = new MemoryStream(data))
			using (ZstandardStream compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
			using (MemoryStream temp = new MemoryStream())
			{
				// Copy from the compressed stream to the uncompressed stream
				compressionStream.CopyTo(temp);

				// Convert the stream into a byte array
				return temp.ToArray();
			}
		}

		#endregion
	}
}
