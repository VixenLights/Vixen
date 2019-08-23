using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using NLog;
using Vixen.Export.FPP;
using Zstandard.Net;

namespace Vixen.Export
{
	public sealed class FSEQCompressedWriter : ExportWriterBase
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		//Constants
		private const byte VMinor = 0;
		private const byte VMajor = 2;
		private const ushort FixedHeaderLength = 32;

		//Working fields
		private uint _offsetToChannelData = 0;
		private uint _channelsPerFrame = 0;
		private uint _numberFrames = 0;
		private uint _framesPerBlock = 0;
		private byte _numberCompressionBlocks;
		private byte _numberSparseRanges = 0;
		private uint _currentFrame = 0;
		private ushort _currentFrameInBlock = 0;
		private ushort _currentBlock = 0;
		private uint _blockStartFrame = 0;
		private readonly List<VariableHeader> _variableHeaders;

		private FileStream _outfs = null;
		private MemoryStream _memoryStream;
		private ZstandardStream _zStdStream;
		private BinaryWriter _dataOut = null;

		private readonly Dictionary<uint, uint> _compressBlockMap;
		private readonly Dictionary<uint, uint> _sparseRangeBlocks;

		public FSEQCompressedWriter()
		{
			FileType = "fseq";
			FileTypeDescr = "Falcon Player Sequence 2.6+";
			CanCompress = true;
			Version = @"2.6";
			IsFalconFormat = true;
			EnableCompression = true;
			_compressBlockMap = new Dictionary<uint, uint>();
			_sparseRangeBlocks = new Dictionary<uint, uint>();
			_variableHeaders = new List<VariableHeader>();
		}

		private void Reset()
		{
			_currentBlock = 0;
			_currentFrame = 0;
			_currentBlock = 0;
			_currentFrameInBlock = 0;
			_blockStartFrame = 0;
			_compressBlockMap.Clear();
			_sparseRangeBlocks.Clear();
			_variableHeaders.Clear();
		}

		public void WriteFileHeader(BinaryWriter writer)
		{

			//Reference to file spec.
			//https://github.com/FalconChristmas/fpp/blob/master/docs/FSEQ_Sequence_File_Format.txt
			
			var header = new byte[FixedHeaderLength];

			var length = FixedHeaderLength + _compressBlockMap.Count * 8 + _sparseRangeBlocks.Count * 6;
			
			// Header Information
			// Format Identifier
			header[0] = (byte)'P'; //Byte 0
			header[1] = (byte)'S'; //Byte 1
			header[2] = (byte)'E'; //Byte 2
			header[3] = (byte)'Q'; //Byte 3

			// Offset to start of channel data Byte 4, 5 
			header[4] = (byte)(_offsetToChannelData & 0xFF);
			header[5] = (byte)((_offsetToChannelData >> 8) & 0xFF);

			// Version info Byte 6, 7
			header[6] =VMinor;
			header[7] =VMajor;

			// Index offset to variable header  Byte 8, 9
			header[8] = (byte) (length & 0xFF);
			header[9] = (byte) ((length >> 8) & 0xFF);

			// Channels per frame 4 Byte value Bytes 10,11,12,13
			header[10] = (byte) (_channelsPerFrame & 0xFF);
			header[11] = (byte) ((_channelsPerFrame >> 8) & 0xFF);
			header[12] = (byte) ((_channelsPerFrame >> 16) & 0xFF);
			header[13] = (byte) ((_channelsPerFrame >> 24) & 0xFF);

			// Number of Frames Bytes 14,15,16,17
			header[14] = (byte) (_numberFrames & 0xFF);
			header[15] = (byte) ((_numberFrames >> 8) & 0xFF);
			header[16] = (byte) ((_numberFrames >> 16) & 0xFF);
			header[17] = (byte) ((_numberFrames >> 24) & 0xFF);

			// Step time in ms  Byte 18
			header[18] = (byte) (SeqPeriodTime & 0xFF);

			// bit flags/reserved should be 0  Byte 19
			header[19] = byte.MinValue;

			//Compression flag 0 for uncompressed, 1 for zstd, 2 for libz/gzip Byte 20
			header[20] = EnableCompression ? (byte) 1 : byte.MinValue;

			//number of compression blocks, 0 if uncompressed Byte 21
			header[21] = _numberCompressionBlocks;

			//number of sparse ranges, 0  if none Byte 22
			header[22] = _numberSparseRanges;

			// bit flags/reserved, unused right now, should be 0  Byte 23
			header[23] = byte.MinValue;

			// 64bit unique identifier, likely a timestamp or uuid Bytes 24-31 
			var id = DateTime.Now.Ticks;
			header[24] = (byte) (id & 0xFF);
			header[25] = (byte) ((id >> 8) & 0xFF);
			header[26] = (byte) ((id >> 16) & 0xFF);
			header[27] = (byte) ((id >> 24) & 0xFF);
			header[28] = (byte) ((id >> 30) & 0xFF);
			header[29] = (byte) ((id >> 38) & 0xFF);
			header[30] = (byte) ((id >> 46) & 0xFF);
			header[31] = (byte)((id >> 54) & 0xFF);

			writer.Write(header);

			foreach (var keyValuePair in _compressBlockMap)
			{
				// frame number
				writer.Write((byte)(keyValuePair.Key & 0xFF));
				writer.Write((byte)((keyValuePair.Key >> 8) & 0xFF));
				writer.Write((byte)((keyValuePair.Key >> 16) & 0xFF));
				writer.Write((byte)((keyValuePair.Key >> 24) & 0xFF));

				// length of block in bytes
				writer.Write((byte)(keyValuePair.Value & 0xFF));
				writer.Write((byte)((keyValuePair.Value >> 8) & 0xFF));
				writer.Write((byte)((keyValuePair.Value >> 16) & 0xFF));
				writer.Write((byte)((keyValuePair.Value >> 24) & 0xFF));
			}

			foreach (var keyValuePair in _sparseRangeBlocks)
			{
				// start channel number
				writer.Write((byte)(keyValuePair.Key & 0xFF));
				writer.Write((byte)((keyValuePair.Key >> 8) & 0xFF));
				writer.Write((byte)((keyValuePair.Key >> 16) & 0xFF));
				
				// number of channels
				writer.Write((byte)(keyValuePair.Value & 0xFF));
				writer.Write((byte)((keyValuePair.Value >> 8) & 0xFF));
				writer.Write((byte)((keyValuePair.Value >> 16) & 0xFF));
				
			}

			foreach (var variableHeader in _variableHeaders)
			{
				writer.Write(variableHeader.GetHeaderBytes());
			}
			
		}

		
		/// <inheritdoc />
		public override void OpenSession(SequenceSessionData data)
		{
			Reset();
			SeqPeriodTime = data.PeriodMS;
			_numberFrames = (uint)data.NumPeriods;
			_channelsPerFrame = (uint)data.ChannelNames.Count();
			if (!string.IsNullOrEmpty(data.OutputAudioFileName))
			{
				_variableHeaders.Add(new VariableHeader(data.OutputAudioFileName));
			}

			_variableHeaders.Add(new VariableHeader(HeaderType.SequenceProducer));
			
			var blockCount = ComputeMaxBlockCount();
			_numberCompressionBlocks = (byte) blockCount;

			_offsetToChannelData = FixedHeaderLength + blockCount * 8; //need to add for sparse if we do that
			_offsetToChannelData += (uint)_variableHeaders.Sum(x => x.HeaderLength); //Account for variable header length

#if DEBUG
			Logging.Info($"Total frames count {_numberFrames}");
			Logging.Info($"Block count {blockCount}");
			Logging.Info($"Frames per block count {_framesPerBlock}");
			Logging.Info($"Offset to channel data {_offsetToChannelData}");
#endif
			
			OpenSession(data.OutFileName, _offsetToChannelData, data.ChannelNames.Count());
		}

		private void OpenSession(string fileName, uint headerLength, Int32 numChannels)
		{
			try
			{
				_outfs = File.Create(fileName, numChannels * 2, FileOptions.None);
				_dataOut = new BinaryWriter(_outfs);
				_dataOut.Write(new Byte[headerLength]);
				InitStream();
			}
			catch (Exception e)
			{
				_outfs = null;
				_dataOut = null;
				Logging.Error(e, "An error occurred opening the filestreams for export.");
				throw e;
			}
		}

		private void InitStream()
		{
			_memoryStream = new MemoryStream();
			if (EnableCompression)
			{
				_zStdStream = new ZstandardStream(_memoryStream, 10);
			}
		}

		private void WriteData(byte[] data)
		{
			if (EnableCompression)
			{
				_zStdStream.Write(data, 0, data.Length);
			}
			else
			{
				_memoryStream.Write(data, 0, data.Length);
			}
		}

		private int FinalizeBlock(bool last=false)
		{
			
			if (EnableCompression)
			{
				_zStdStream.Flush();
			}

			var data = _memoryStream.ToArray();
			_dataOut.Write(data);

			if (EnableCompression)
			{
				_zStdStream.Dispose();
			}

			_memoryStream.Dispose();

			if (!last)
			{
				_memoryStream = new MemoryStream();

				if (EnableCompression)
				{
					_zStdStream = new ZstandardStream(_memoryStream, 10);
				}
			}
#if DEBUG
			//if (Compress)
			//{
			//	Decompress(data);
			//}
#endif
			return data.Length;
		}
		/// <inheritdoc />
		public override void WriteNextPeriodData(List<byte> periodData)
		{
			if (_currentBlock < 1 && _currentFrameInBlock >= 10)
			{
				ChangeBlock();
			}
			else if(_currentFrameInBlock >= _framesPerBlock)
			{
				ChangeBlock();
			}

			_currentFrameInBlock++;
			_currentFrame++;
			WriteData(periodData.ToArray());
		}

		private void ChangeBlock(bool last = false)
		{
			var length = FinalizeBlock(last);
#if DEBUG
			Logging.Info($"Block {_currentBlock}, Start Frame {_blockStartFrame}, Bytes {length}, Frames {_currentFrameInBlock}, Processed Frame count {_currentFrame}");
#endif
			_compressBlockMap.Add(_blockStartFrame, (uint) length);
			_blockStartFrame = _currentFrame;
			_currentBlock++;
			_currentFrameInBlock = 0;
		}

		/// <inheritdoc />
		public override void CloseSession()
		{
			ChangeBlock();
			try
			{
				_dataOut.Seek(0, SeekOrigin.Begin);
				WriteFileHeader(_dataOut);
				_dataOut.Flush();
				_dataOut.Close();
				_dataOut = null;
				_outfs.Close();
				_outfs.Close();
				_outfs = null;

			}
			catch (Exception e)
			{
				_dataOut = null;
				_outfs = null;
				Logging.Error(e, "An error occuring closing the export session.");
				throw e;
			}

		}

		private void Decompress(byte[] data)
		{
			try
			{
				using (var memoryStream = new MemoryStream(data))
				using (var compressionStream = new ZstandardStream(memoryStream, CompressionMode.Decompress))
				using (var temp = new MemoryStream())
				{
					compressionStream.CopyTo(temp);
					var output = temp.ToArray();
					StringBuilder sb = new StringBuilder(output.Length);
					foreach (var b in output)
					{
						sb.Append($"{b} ");
					}

					Logging.Info($"Frame data: {sb}");
				}
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occurred decompressing block.");
			}
		}

		private uint ComputeMaxBlockCount()
		{
			ulong size = (ulong) (_channelsPerFrame * _numberFrames);
			var numberBlocks = size;
			numberBlocks /= (64 * 2014);
			if (numberBlocks > 255)
			{
				numberBlocks = 255;
			}
			else if(numberBlocks < 1)
			{
				numberBlocks = 1;
			}

			_framesPerBlock = (uint) (_numberFrames / numberBlocks);

			if (_framesPerBlock < 10)
			{
				_framesPerBlock = 10;
			}

			var frameCount = _numberFrames - 10;  //peel off ten frames that we will put in the first block
			numberBlocks = frameCount / _framesPerBlock + 1;

			while (numberBlocks > 254)
			{
				_framesPerBlock++;
				numberBlocks = frameCount / _framesPerBlock + 1;
			}

			// first block is going to be smaller and special so add one for it
			if(numberBlocks < 255)
			{
				numberBlocks++;
			}

			return (uint)numberBlocks;
		}
	}
}
