using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;


namespace Vixen.Export
{
	public sealed class HelixWriter : ExportWriterBase
    {
        private Vix2XMLData _xmlData;
        private Byte[] _periodData;
        private int _curPeriod;
        SequenceSessionData _sessionData;
        private FileStream _outfs = null;
        private int _adder;

        public HelixWriter()
        {
	        FileTypeDescr = "Helix File";
	        FileType = "vix";
        }
        public override void OpenSession(SequenceSessionData sessionData)
        {

            _curPeriod = 0;
            _sessionData = sessionData; 
            try
            {
                _outfs = File.Create(_sessionData.OutFileName, _sessionData.ChannelNames.Count * 2, FileOptions.None);
            }
            catch (Exception e)
            {
                _outfs = null;
                throw e;
            }
			_xmlData = new Vix2XMLData();
            _xmlData.Channels = new List<Vix2Channel>();

            _xmlData.Time = _sessionData.TimeMS.ToString();

            _xmlData.EventPeriodInMilliseconds = _sessionData.PeriodMS.ToString();

            _adder = 0;
            if (_sessionData.TimeMS % _sessionData.PeriodMS != 0)
            {
                _adder = 1;
            }

            _periodData = new Byte[sessionData.ChannelNames.Count * (_sessionData.NumPeriods + _adder)];

            _xmlData.MinimumLevel = "0";
            _xmlData.MaximumLevel = "255";
            _xmlData.AudioDevice = "-1";
            _xmlData.AudioVolume = "0";
            

        }

        public override void WriteNextPeriodData(List<Byte> periodData)
        {
            int numPeriods =  _sessionData.NumPeriods + _adder;

            for (int j = 0; j < periodData.Count; j++)
            {
                _periodData[(j * numPeriods) + _curPeriod] = periodData[j];
            }

            _curPeriod++;
        }

        public override void CloseSession()
        {
            int count = 0;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.IndentChars = "    ";
            settings.Indent = true;
            
            XmlWriter writer =  XmlWriter.Create(_outfs, settings);


            XmlSerializerNamespaces n = new XmlSerializerNamespaces();
            n.Add("", "");

            _xmlData.EventValues = Convert.ToBase64String(_periodData);

            Vix2Channel tempChannel;
 
            foreach (string channelName in _sessionData.ChannelNames)
            {
                tempChannel = new Vix2Channel() 
                { 
                    name = channelName, 
                    id = count, 
                    output = count, 
                    enabled = true, 
                    color = -1
                };
                count++;
                _xmlData.Channels.Add(tempChannel);
            }

            if (_sessionData.AudioFileName.Length > 0)
            {
                _xmlData.Audio = new Vix2Audio();
                _xmlData.Audio.filename = Path.GetFileName(_sessionData.AudioFileName);
                _xmlData.Audio.duration = _sessionData.PeriodMS.ToString();
                _xmlData.Audio.Value = _xmlData.Audio.filename;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Vix2XMLData));
            serializer.Serialize(writer, _xmlData, n);
            

            try
            {
                _outfs.Close();
                _outfs = null;

            }
            catch (Exception e)
            {
                _outfs = null;
                throw e;
            }

        }
        
    }

}
