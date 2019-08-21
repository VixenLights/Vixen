using System;
using System.Collections.Generic;
using System.IO;


namespace Vixen.Export
{
    public sealed class Vir2Writer : ExportWriterBase
    {
	    private Byte[] _periodData;
        private int _curPeriod;
        private SequenceSessionData _sessionData;
        private FileStream _outfs = null;
        private BinaryWriter _dataOut = null;

        public Vir2Writer()
        {
	        FileType = "vir";
	        FileTypeDescr = "Vixen 2 Routine";
        }

        public override void OpenSession(SequenceSessionData sessionData)
        {

            _curPeriod = 0;
            _sessionData = sessionData;
            try
            {
                _outfs = File.Create(_sessionData.OutFileName, _sessionData.ChannelNames.Count * 2, FileOptions.None);
                _dataOut = new BinaryWriter(_outfs);
            }
            catch (Exception e)
            {
                _outfs = null;
                _dataOut = null;
                throw e;
            }

            _periodData = new Byte[sessionData.ChannelNames.Count * _sessionData.NumPeriods];
        }

        public override void WriteNextPeriodData(List<Byte> periodData)
        {
            for (int j = 0; j < periodData.Count; j++)
            {
                _periodData[(j * _sessionData.NumPeriods) + _curPeriod] = periodData[j];
            }

            _curPeriod++;
        }

        public override void CloseSession()
        {

            try
            {
                for (int channel = 0; channel < _sessionData.ChannelNames.Count; channel++)
                {
                    for (int period = 0; period < _sessionData.NumPeriods; period++)
                    {
                        _dataOut.Write(_periodData[channel * _sessionData.NumPeriods + period].ToString().ToCharArray());
                        _dataOut.Write(' ');
                    }
                    _dataOut.Write(Environment.NewLine.ToCharArray());
                }
            }
            catch (Exception e)
            {
                _dataOut.Flush();
                _dataOut.Close();
                _dataOut = null;
                _outfs.Close();
                _outfs = null;

                throw e;
            }

            if (_dataOut != null)
            {
                try
                {
                    _dataOut.Flush();
                    _dataOut.Close();
                    _dataOut = null;
                    _outfs.Close();
                    _outfs = null;

                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
