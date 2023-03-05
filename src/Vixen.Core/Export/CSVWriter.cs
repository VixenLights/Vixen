namespace Vixen.Export
{
    public sealed class CSVWriter : ExportWriterBase
    {
        private Int32 _seqNumChannels = 0;

        private FileStream _outfs = null;
        private BinaryWriter _dataOut = null;

        public CSVWriter()
        {
            SeqPeriodTime = 50;  //Default to 50ms
            FileType = "csv";
            FileTypeDescr = "CSV File";
        }

        public override void OpenSession(SequenceSessionData data)
        {
            OpenSession(data.OutFileName, data.ChannelNames.Count);
        }

        private void OpenSession(string fileName, Int32 numChannels)
        {
            _seqNumChannels = numChannels;

            try
            {
                _outfs = File.Create(fileName, numChannels * 2, FileOptions.None);
                _dataOut = new BinaryWriter(_outfs);
            }
            catch (Exception)
            {
                _dataOut = null;
                throw;
            }
        }

        public void ResetStreamPtr()
        {
            _dataOut.Seek(0, SeekOrigin.Begin);
        }

        public override void WriteNextPeriodData(List<Byte> periodData)
        {
            if (_dataOut != null)
            {
                try
                {
                    _dataOut.Write(periodData[0].ToString("000").ToCharArray()); 
                    for (int j = 1; j < _seqNumChannels; j++)
                    {
                        _dataOut.Write(',');
                        _dataOut.Write(periodData[j].ToString("000").ToCharArray());
                    }
                    _dataOut.Write(System.Environment.NewLine.ToCharArray());
                }
                catch (Exception)
                {
                    _dataOut = null;
                    throw;
                }
            }
        }


        public override void CloseSession()
        {
            if (_dataOut != null)
            {
                try
                {
                    _dataOut.Flush();
                    _dataOut.Close();
                    _dataOut = null;
                    _outfs.Close();
                    _outfs.Close();
                    _outfs = null;

                }
                catch (Exception)
                {
                    _dataOut = null;
                    _outfs = null;
                    throw;
                }
            }
        }

    }
}
