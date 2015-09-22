using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Vixen.Cache.Sequence;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Export
{
    public enum ExportNotifyType
    {
        NETSAVE,
        LOADING,
        EXPORTING,
        SAVING,
        COMPLETE
    };

    public class Export
    {
        private IExportWriter _output;
        private readonly Dictionary<string, IExportWriter> _writers;
        private readonly Dictionary<string, string> _exportFileTypes;

        private bool _exporting;
        private bool _cancelling;
        private string _exportDir;
		private SequenceIntervalGenerator _generator;
        private readonly ExportCommandHandler _exporterCommandHandler;
        private readonly List<byte> _eventData;
	    private List<ControllerExportInfo> _controllerExportInfos; 

        public delegate void SequenceEventHandler(ExportNotifyType notify);
        public event SequenceEventHandler SequenceNotify;

        #region Contructor
        public Export()
        {
            _exportFileTypes = new Dictionary<string, string>();
            _writers = new Dictionary<string, IExportWriter>();
            var type = typeof(IExportWriter);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.Equals(type));

            IExportWriter exportWriter;
            foreach (Type theType in types.ToArray())
            {
                exportWriter = (IExportWriter)Activator.CreateInstance(theType);
                _writers[exportWriter.FileType] = exportWriter;
                _exportFileTypes[exportWriter.FileTypeDescr] = exportWriter.FileType;
            }

            UpdateInterval = VixenSystem.DefaultUpdateInterval;  //Default the UpdateInterval to the global interval

            _eventData = new List<byte>();
            _exporterCommandHandler = new ExportCommandHandler();

            _exporting = false;
            _cancelling = false;

            SavePosition = 0;

			InitializeControllerInfo();
        }

	    #endregion

        #region Properties
        public string ExportDir
        {
            get
            {
                if (_exportDir == null)
                {
                    ExportDir = Path.Combine(Paths.DataRootPath, "Export");
                }
                return _exportDir;
            }

            set
            {
                _exportDir = value;
                CheckExportdir();
            }
        }

        public string[] FormatTypes
        {
            get
            {
                return _exportFileTypes.Keys.ToArray();
            }
        }

        public string OutFileName { get; set; }

        public int UpdateInterval { get; set; }

        public string AudioFilename { get; set; }

        public Dictionary<string, string> ExportFileTypes
        {
            get
            {
                return _exportFileTypes;
            }
        }

        private List<OutputController> SystemControllers
        {
            get
            {
                return VixenSystem.OutputControllers.ToList();//.FindAll(x => x.ModuleId != _controllerTypeId); 
            }
        }

		public List<Guid> ExportControllerList { get; set; } 

        public decimal SavePosition { get; set; }

        public List<ControllerExportInfo> ControllerExportInfo
        {
            get { return _controllerExportInfos; }
        }

        #endregion

        #region Operational

		private void InitializeControllerInfo()
		{
			int index = 0;
			_controllerExportInfos = new List<ControllerExportInfo>();
			SystemControllers.ForEach(x => _controllerExportInfos.Add(new ControllerExportInfo(x, index++)));
		}

        private bool CheckExportdir()
        {
            if (!Directory.Exists(_exportDir))
            {
                try
                {
                    Directory.CreateDirectory(_exportDir);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public void WriteControllerInfo(ISequence sequence)
        {
            int chanStart = 1;

            string xmlOutName =
                Path.GetDirectoryName(OutFileName) +
                Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(OutFileName) +
                "_Network.xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            using (XmlWriter writer = XmlWriter.Create(xmlOutName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Vixen3_Export");
                writer.WriteElementString("Resolution", UpdateInterval.ToString());
                writer.WriteElementString("OutFile", Path.GetFileName(OutFileName));
                writer.WriteElementString("Duration", sequence.Length.ToString());

                writer.WriteStartElement("Network");
                foreach (ControllerExportInfo exportInfo in ControllerExportInfo.OrderBy(x => x.Index))
                {
                    writer.WriteStartElement("Controller");
                    writer.WriteElementString("Index", exportInfo.Index.ToString());
                    writer.WriteElementString("Name", exportInfo.Name);
                    writer.WriteElementString("StartChan", chanStart.ToString());
                    writer.WriteElementString("Channels", exportInfo.Channels.ToString());
                    writer.WriteEndElement();

                    chanStart += exportInfo.Channels;
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

        }

        public void DoExport(ISequence sequence, string outFormat)
        {
            string fileType;

            _exporting = true;
            _cancelling = false;

            if ((sequence != null) && (_exportFileTypes.TryGetValue(outFormat,out fileType)))
            {
                if (_writers.TryGetValue(fileType, out _output))
                {
					_generator = new SequenceIntervalGenerator(UpdateInterval, sequence);
                    WriteControllerInfo(sequence);
					Task.Factory.StartNew(ProcessExport);
                }
            }
        }
        
        public void Cancel()
        {
            _cancelling = true;
			_generator.EndGeneration();
        }

        private void UpdateState(ICommand[] outputStates)
        {
            _eventData.Clear();

            for (int i = 0; i < outputStates.Length; i++)
            {
                _exporterCommandHandler.Reset();
                ICommand command = outputStates[i];
                if (command != null)
                {
                    command.Dispatch(_exporterCommandHandler);
                }
                _eventData.Add(_exporterCommandHandler.Value);
            }
            
            _output.WriteNextPeriodData(_eventData);
           
        }
        #endregion

        #region Events
       
        string ReverseBuildChannelName(IDataFlowComponent component, int outIndex)
        {
            string nameVal = component.Outputs[outIndex].Name ?? "";

            if (component.Source != null)
            {
                if (component.Name.Equals("Color Breakdown"))
                {
                    nameVal =
                        ReverseBuildChannelName(component.Source.Component, component.Source.OutputIndex) + 
                        " " + 
                        component.Outputs[outIndex].Name;
                }
                else
                {
                    nameVal =
                        ReverseBuildChannelName(component.Source.Component, component.Source.OutputIndex);
                }
            }
            return nameVal;
        }
        List<string> BuildChannelNames(IEnumerable<Guid> outIds)
        {
            List<string> retVal = new List<string>();
	        IEnumerable<OutputController> outControllers = VixenSystem.OutputControllers.GetAll();
            foreach (OutputController oc in outControllers)
            {
                for (int j = 0; j < oc.OutputCount; j++)
                {
	                string chanName;
	                if (oc.Outputs[j].Source != null)
                    {
                        chanName = ReverseBuildChannelName(oc.Outputs[j].Source.Component, oc.Outputs[j].Source.OutputIndex);
                    }
                    else
                    {
                        chanName = oc.Name + "_" + (j + 1);
                    }
                    retVal.Add(chanName);
                }
            }

            return retVal;
        }

        private void ProcessExport()
        {
            SequenceSessionData sessionData = new SequenceSessionData();
			
            if (_exporting)
            {
				SavePosition = 0;
				SequenceNotify(ExportNotifyType.SAVING);
             	_generator.BeginGeneration();
                List<ICommand> commandList = new List<ICommand>();
	            
	            IEnumerable<Guid> outIds = _generator.State.GetOutputIds();
	            int periods = (int)_generator.Sequence.Length.TotalMilliseconds / _generator.Interval;//outAggregator.GetCommandsForOutput(outIds.First()).Count() - 1;

				//Get a list of controller ids by index order
				IEnumerable<Guid> controllers = ControllerExportInfo.OrderBy(x => x.Index).Select(i => i.Id);

				//Now assemble a all their outputs by controller order.
				List<List<Guid>> controllerOutputs = controllers.Select(controller => VixenSystem.OutputControllers.GetController(controller).Outputs.Select(x => x.Id).ToList()).ToList();

	            if (_cancelling == false)
                {
                    SequenceNotify(ExportNotifyType.SAVING);
                    sessionData.OutFileName = OutFileName;
                    sessionData.NumPeriods = periods;
                    sessionData.PeriodMS = UpdateInterval;
                    sessionData.ChannelNames = BuildChannelNames(outIds);
                    sessionData.TimeMS = _generator.Sequence.Length.TotalMilliseconds;
                    sessionData.AudioFileName = AudioFilename;
	                try
	                {
		                _output.OpenSession(sessionData);
		                int j = 0;
		                while (_generator.HasNextInterval())
		                {
			                SavePosition = Decimal.Round(((Decimal) j/periods)*100, 2);
			                commandList.Clear();
			                //Iterate the controller output groups.
			                foreach (var controller in controllerOutputs)
			                {
				                //Grab commands for each output
				                commandList.AddRange(controller.Select(guid => _generator.State.GetCommandForOutput(guid)));
			                }

			                UpdateState(commandList.ToArray());
			                _generator.NextInterval();
			                j++;
		                }

		                _output.CloseSession();
	                }
	                catch (Exception ex)
	                {
		                MessageBox.Show(ex.Message, @"Save Error!");
		                throw ex;
	                }
	                finally
	                {
		                _generator.EndGeneration();
	                }

                }

                if (SequenceNotify != null)
                {
                    SequenceNotify(ExportNotifyType.COMPLETE);
                }
            }
        }
        
        #endregion

    }

    public class ControllerExportInfo
    {
        public ControllerExportInfo(OutputController controller, int index)
        {
            Name = controller.Name;
            Index = index;
            Channels = controller.OutputCount;
	        Id = controller.Id;
        }

        public int Index { get; set; }
        public int Channels { get; set; }
        public string Name { get; set; }
		public Guid Id { get; private set; }
    }

    public class SequenceSessionData
    {
        public SequenceSessionData()
        {
            PeriodMS = 50;
            TimeMS = 0;
            AudioFileName = "";
            ChannelNames = new List<string>();
        }

        public int PeriodMS { get; set; }
        public double TimeMS { get; set; }
        public string AudioFileName { get; set; }
        public List<string> ChannelNames { get; set; }
        public string OutFileName { get; set; }
        public int NumPeriods { get; set; }
    }
}