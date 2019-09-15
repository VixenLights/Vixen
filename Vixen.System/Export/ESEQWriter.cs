using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vixen.Export
{
    public sealed class ESEQWriter : ExportWriterBase
    {
        private const UInt32 _dataOffset = 20;
        private const UInt16 _fixedHeaderLength = 20;
        private Int32 _seqNumChannels = 0;
        private Int32 _seqNumPeriods = 0;
        private Int32 _startAddress = 0;
        private Int32 _numModels = 1;
        private Int32 _modelSize = 0;
        
        private FileStream _outfs = null;
        private BinaryWriter _dataOut = null;

        private Byte[] _padding;

        //step size is number of channels in output
        //num steps is number of 25,50,100ms intervals

        public ESEQWriter()
        {
            SeqPeriodTime = 50;  //Default to 50ms
            IsFalconFormat = true;
			FileTypeDescr = "Falcon Player Effect";
			FileType = "eseq";
        }

		public void WriteFileHeader()
        {
            if (_dataOut != null)
            {

                // Header Information
                // Format Identifier
                _dataOut.Write('E');
                _dataOut.Write('S');
                _dataOut.Write('E');
                _dataOut.Write('Q');

                //Number of Models 
                _dataOut.Write((Byte)(_numModels & 0xFF));
                _dataOut.Write((Byte)((_numModels >> 8) & 0xFF));
                _dataOut.Write((Byte)((_numModels >> 16) & 0xFF));
                _dataOut.Write((Byte)((_numModels >> 24) & 0xFF));

                // Step Size
                _dataOut.Write((Byte)(_seqNumChannels & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 8) & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 16) & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 24) & 0xFF));

                //Model Start address
                _dataOut.Write((Byte)(_startAddress & 0xFF));
                _dataOut.Write((Byte)((_startAddress >> 8) & 0xFF));
                _dataOut.Write((Byte)((_startAddress >> 16) & 0xFF));
                _dataOut.Write((Byte)((_startAddress >> 24) & 0xFF));

                // Model Size
                _modelSize = _seqNumPeriods * _seqNumChannels;
                
                _dataOut.Write((Byte)(_seqNumChannels & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 8) & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 16) & 0xFF));
                _dataOut.Write((Byte)((_seqNumChannels >> 24) & 0xFF));
                
            }
        }

        public override void OpenSession(SequenceSessionData data)
        {
            this.SeqPeriodTime = data.PeriodMS;
            OpenSession(data.OutFileName, data.NumPeriods, data.ChannelNames.Count());
        }

        private void OpenSession(string fileName, Int32 numPeriods, Int32 numChannels)
        {
            try
            {
                _outfs = File.Create(fileName, numChannels * 2, FileOptions.None);
                _dataOut = new BinaryWriter(_outfs);
                _dataOut.Write(new Byte[_fixedHeaderLength]);
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
