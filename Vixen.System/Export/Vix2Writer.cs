using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Execution;
using Vixen.Commands;


namespace Vixen.Export
{
    public class Vix2Writer : IExportWriter
    {
        private Vix2XMLData _xmlData;
        private Byte[] _periodData;
        private int _curPeriod;
        SequenceSessionData _sessionData;
        private FileStream _outfs = null;


        public int SeqPeriodTime { get; set; }
        
        public void WriteFileHeader()
        {

        }
        
        public void WriteFileFooter()
        {

        }

        public void OpenSession(SequenceSessionData sessionData)
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
            _xmlData.Channels = new List<Channel>();

            _xmlData.Time = _sessionData.TimeMS.ToString();

            _xmlData.EventPeriodInMilliseconds = _sessionData.PeriodMS.ToString();

            _periodData = new Byte[sessionData.ChannelNames.Count * _sessionData.NumPeriods];

            _xmlData.MinimumLevel = "0";
            _xmlData.MaximumLevel = "255";
            _xmlData.AudioDevice = "-1";
            _xmlData.AudioVolume = "0";

        }

        public void WriteNextPeriodData(List<Byte> periodData)
        {
            int numPeriods =  _sessionData.NumPeriods;

            for (int j = 0; j < periodData.Count; j++)
            {
                _periodData[(j * numPeriods) + _curPeriod] = periodData[j];
            }

            _curPeriod++;
        }

        public void CloseSession()
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

            Channel tempChannel;
 
            foreach (string channelName in _sessionData.ChannelNames)
            {
                count++;
                tempChannel = new Channel() 
                { 
                    name = channelName, 
                    id = count, 
                    output = count, 
                    enabled = true, 
                    color = -1
                };
                _xmlData.Channels.Add(tempChannel);
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
        public string FileType
        {
            get
            {
                return "vix";
            }
        }

        public string FileTypeDescr
        {
            get
            {
                return "Vixen 2.1 Sequence";
            }
        }
    }

    [Serializable()]
    [XmlRoot("Program")]
    public class Vix2XMLData
    {
        public Vix2XMLData()
        {
            EngineType = "Standard";
        }

        public string Time { get; set; }
        public string EventPeriodInMilliseconds { get; set; }
        public string MinimumLevel { get; set; }
        public string MaximumLevel { get; set; }
        public string AudioDevice { get; set; }
        public string  AudioVolume { get; set; }
        
        [XmlArrayItem("Channel")]
        public List<Channel> Channels { get; set; }

        public string EngineType { get; set; }
        public string EventValues { get; set; }
        
    }

    public class Channel
    {
        
        [XmlText]
        public string name;

        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public int output;

        [XmlAttribute]
        public bool enabled;

        [XmlAttribute]
        public int color;
    }
}
