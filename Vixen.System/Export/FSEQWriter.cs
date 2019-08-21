using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vixen.Export.FPP;

namespace Vixen.Export
{
    public sealed class FSEQWriter : ExportWriterBase
    {
        private const Byte _vMinor = 0;
        private const Byte _vMajor = 1;
        private const UInt16 _fixedHeaderLength = 28;
        private UInt32 _dataOffset = _fixedHeaderLength;
        private Int32 _seqNumChannels = 0;
        private UInt32 _seqNumPeriods = 0;
        private UInt16 _numUniverses = 0;    //Ignored by Pi Player
        private UInt16 _universeSize = 0;    //Ignored by Pi Player
        private Byte _gamma = 1;             //0=encoded, 1=linear, 2=RGB Ignored by FPP
		private Byte _colorEncoding = 2;
        private readonly List<VariableHeader> _variableHeaders;

		private FileStream _outfs = null;
        private BinaryWriter _dataOut = null;

        private Byte[] _padding;

        //step size is number of channels in output
        //num steps is number of 25,50,100ms intervals

        public FSEQWriter()
        {
            SeqPeriodTime = 50;  //Default to 50ms
            FileTypeDescr = "Falcon Player Sequence";
            FileType = "fseq";
			_variableHeaders = new List<VariableHeader>(1);
        }

		public void WriteFileHeader()
        {
            if (_dataOut != null)
            {

                // Header Information
                // Format Identifier
                _dataOut.Write('P');
                _dataOut.Write('S');
                _dataOut.Write('E');
                _dataOut.Write('Q');
                
                // Data offset  (Initially write to the fixed header length)
                _dataOut.Write((Byte)(_dataOffset % 256));
                _dataOut.Write((Byte)(_dataOffset / 256));

                // Data header
                _dataOut.Write(_vMinor);
                _dataOut.Write(_vMajor);

                // Fixed header length
                _dataOut.Write((Byte)(_fixedHeaderLength % 256));
                _dataOut.Write((Byte)(_fixedHeaderLength / 256));

                // Step Size
                _dataOut.Write((Byte)(_seqNumChannels & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 8) & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 16) & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 24) & 0xFF));

                // Number of Steps
                _dataOut.Write((Byte)(_seqNumPeriods & 0xFF));
                _dataOut.Write((Byte)((_seqNumPeriods >> 8) & 0xFF));
                _dataOut.Write((Byte)((_seqNumPeriods >> 16) & 0xFF));
                _dataOut.Write((Byte)((_seqNumPeriods >> 24) & 0xFF));

                // Step time in ms
                _dataOut.Write((Byte)(SeqPeriodTime & 0xFF));

				// 19 bit flags/reserved should be zero
                _dataOut.Write(Byte.MinValue);

				// 20-21 universe count, ignored by FPP
				_dataOut.Write((Byte)(_numUniverses & 0xFF));
                _dataOut.Write((Byte)((_numUniverses >> 8) & 0xFF));

				// 22-23 universe Size, ignored by FPP
				_dataOut.Write((Byte)(_universeSize & 0xFF));
                _dataOut.Write((Byte)((_universeSize >> 8) & 0xFF));

                // 24 gamma, should be 1, ignored by FPP
                _dataOut.Write(_gamma);

                // 25 color encoding 2 for RGB, ignored by FPP
                _dataOut.Write(_colorEncoding);

				//26-27 reserved, should be 0
                _dataOut.Write(Byte.MinValue);
                _dataOut.Write(Byte.MinValue);

				//Write the variable headers
				foreach (var variableHeader in _variableHeaders)
				{
					_dataOut.Write(variableHeader.GetHeaderBytes());
				}
			}
        }

        public override void OpenSession(SequenceSessionData data)
        {
			_variableHeaders.Clear();
	        _dataOffset = _fixedHeaderLength;
			SeqPeriodTime = data.PeriodMS;
			
			if (!string.IsNullOrEmpty(data.OutputAudioFileName))
			{
				_variableHeaders.Add(new VariableHeader(data.OutputAudioFileName));
			}
			_dataOffset += (uint)_variableHeaders.Sum(x => x.HeaderLength); //Account for variable header length
																	
			OpenSession(data.OutFileName, data.NumPeriods, data.ChannelNames.Count());
        }

        private void OpenSession(string fileName, Int32 numPeriods, Int32 numChannels)
        {
            try
            {
                _outfs = File.Create(fileName, numChannels * 2, FileOptions.None);
                _dataOut = new BinaryWriter(_outfs);
                _dataOut.Write(new Byte[_dataOffset]);
                _seqNumChannels = numChannels;
                _seqNumPeriods = 0;
                if ((_seqNumChannels % 4) != 0)
                {
                    _padding = Enumerable.Repeat((Byte)0, 4 - (_seqNumChannels % 4)).ToArray();
                    _seqNumChannels += _padding.Length;
                }
                else
                {
                    _padding = null;
                }
            }
            catch (Exception e)
            {
                _outfs = null;
                _dataOut = null;
                throw e;
            }
        }

        public override void WriteNextPeriodData(List<Byte> periodData)
        {
            if (_dataOut != null)
            {
                try
                {
                    _dataOut.Write(periodData.ToArray());
                    if (_padding != null)
                    {
                        _dataOut.Write(_padding);
                    }
                    
                    _seqNumPeriods++;

                }
                catch (Exception e)
                {
                    _dataOut = null;
                    _outfs = null;
                    throw e;
                }
            }

        }

        public override void CloseSession()
        {
            if (_dataOut != null)
            {
                try
                {
                    _dataOut.Seek(0, SeekOrigin.Begin);
                    WriteFileHeader();
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
                    throw e;
                }
            }
        }

    }
}
