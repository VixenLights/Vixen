using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vixen.Cache.Sequence;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Export.FPP;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Sys.Output;
using Formatting = Newtonsoft.Json.Formatting;

namespace Vixen.Export
{
    public enum ExportNotifyType
    {
        NONE,
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
        private List<byte> _eventData;
	    private List<Controller> _controllerExportInfos; 

        public delegate void SequenceEventHandler(ExportNotifyType notify);
        public event SequenceEventHandler SequenceNotify;

	    public bool AllSelectedControllersSupportUniverses => ControllerExportInfo.Where(x => x.IsActive).All(x => x.HasNetworkSupport);

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

            _exporterCommandHandler = new ExportCommandHandler();

            _exporting = false;
            _cancelling = false;

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

	    public string DefaultFormatType()
	    {
			//This is sketchy at best
			return ExportFileTypes.ContainsKey("Falcon Player Sequence") ? "Falcon Player Sequence" : FormatTypes[0];
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

        private static List<OutputController> SystemControllers
        {
            get
            {
                return VixenSystem.OutputControllers.ToList();//.FindAll(x => x.ModuleId != _controllerTypeId); 
            }
        }

		public List<Guid> ExportControllerList { get; set; } 

        public decimal SavePosition { get; set; }

        public List<Controller> ControllerExportInfo
        {
            get { return _controllerExportInfos; }
			set { _controllerExportInfos = value; }
        }

        #endregion

        #region Operational

		private void InitializeControllerInfo()
		{
			_controllerExportInfos = CreateControllerInfo(true);
		}

	    public static List<Controller> CreateControllerInfo(bool index)
	    {
			int i = 0;
		    var controllerExportInfos = new List<Controller>();
		    SystemControllers.ForEach(x => controllerExportInfos.Add(new Controller(x, index?i++:Int32.MaxValue)));
		    return controllerExportInfos;
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

		/// <summary>
		/// Writes FPP Universe file in 2.x json format
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
	    public async Task Write2xUniverseFile(string fileName)
	    {
		    var fppStartChannel = 1;
			FppOutputConfiguration config = new FppOutputConfiguration();
			ChannelOutputs channelOutputs = new ChannelOutputs();
			config.ChannelOutputs.Add(channelOutputs);
		    foreach (var controller in ControllerExportInfo.Where(x => x.IsActive).OrderBy(x => x.Index))
		    {
			    if (controller.HasNetworkSupport)
			    {
				    var universes = controller.ControllerNetworkConfiguration.Universes;
					foreach (var uc in universes)
				    {
						Universe u = new Universe();
						channelOutputs.Universes.Add(u);
					    string ip = uc.IpAddress?.Address.ToString();

						if (ip == null) ip = string.Empty;

						u.Address = ip;
					    u.Description = controller.Name;
					    u.UniverseType = uc.IsMultiCast?UniverseTypes.E131_Multicast:UniverseTypes.E131_Unicast;
					    u.Active = uc.Active;
					    u.ChannelCount = uc.Size;
					    u.StartChannel = fppStartChannel;
					    u.UniverseId = uc.Universe;
						fppStartChannel = fppStartChannel + uc.Size;
				    }
			    }
			    else
			    {
				    fppStartChannel = fppStartChannel + controller.Channels;
			    }
		    }

		    using (var writer = new StreamWriter(fileName))
		    {
			    DefaultContractResolver contractResolver = new DefaultContractResolver
			    {
				    NamingStrategy = new CamelCaseNamingStrategy()
			    };
				var s = JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings
				{
					ContractResolver = contractResolver
				});
			    await writer.WriteAsync(s);
				await writer.FlushAsync();
			}
	    }

		/// <summary>
		/// Writes FPP universe file in 1.x format
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
	    public async Task WriteUniverseFile(string fileName)
	    {
		    using (var writer = new StreamWriter(fileName))
		    {
			    var fppStartChannel = 1;
			    foreach (var controller in ControllerExportInfo.Where(x => x.IsActive).OrderBy(x => x.Index))
			    {
				    if (controller.HasNetworkSupport)
				    {
					    var universes = controller.ControllerNetworkConfiguration.Universes;
					    foreach (var uc in universes)
					    {
						    string ip = string.Empty;
						    if (!uc.IsMultiCast)
						    {
							    //Validate ip address
							    ip = uc.IpAddress?.Address.ToString();
							    if (ip == null)
							    {
								    ip = string.Empty;
							    }
						    }
						    var s =
							    $"{(uc.Active ? "1" : "0")},{uc.Universe},{fppStartChannel},{uc.Size},{(uc.IsMultiCast ? "0" : "1")},{ip},\n";
						    await writer.WriteAsync(s);
						    fppStartChannel = fppStartChannel + uc.Size;
					    }
					}
				    else
				    {
					    fppStartChannel = fppStartChannel + controller.Channels;
					}
				    
			    }

			    await writer.FlushAsync();
		    }
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
                foreach (Controller exportInfo in ControllerExportInfo.Where(x => x.IsActive).OrderBy(x => x.Index))
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

        public async Task DoExport(ISequence sequence, string outFormat, IProgress<ExportProgressStatus> progress = null)
        {
            string fileType;

            _exporting = true;
            _cancelling = false;

            if ((sequence != null) && (_exportFileTypes.TryGetValue(outFormat,out fileType)))
            {
                if (_writers.TryGetValue(fileType, out _output))
                {
					_generator = new SequenceIntervalGenerator(UpdateInterval, sequence);
                    //WriteControllerInfo(sequence);
	                await Task.Factory.StartNew(() => ProcessExport(progress));
                }
            }
        }
        
        public void Cancel()
        {
            _cancelling = true;
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
        List<string> BuildChannelNames(IEnumerable<OutputController> outControllers)
        {
            List<string> retVal = new List<string>();
	       // IEnumerable<OutputController> outControllers = VixenSystem.OutputControllers.GetAll();
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

        private void ProcessExport(IProgress<ExportProgressStatus> progress)
        {
            SequenceSessionData sessionData = new SequenceSessionData();
			
            if (_exporting)
            {
				if (SequenceNotify != null)
	            {
		            SequenceNotify(ExportNotifyType.SAVING);
	            }
             	_generator.BeginGeneration();
                
	            //IEnumerable<Guid> outIds = _generator.State.GetOutputIds();
	            int periods = (int)_generator.Sequence.Length.TotalMilliseconds / _generator.Interval;//outAggregator.GetCommandsForOutput(outIds.First()).Count() - 1;

				//Get a list of controller ids by index order
				IEnumerable<Guid> controllerIds = ControllerExportInfo.Where(x => x.IsActive).OrderBy(x => x.Index).Select(i => i.Id);

	            var controllers = controllerIds.Select(controller => VixenSystem.OutputControllers.GetController(controller));

				//Now assemble a all their outputs by controller order.
				List<List<Guid>> controllerOutputs = controllers.Select(controller => controller.Outputs.Select(x => x.Id).ToList()).ToList();

				List<ICommand> commandList = new List<ICommand>(controllerOutputs.Count);
	            _eventData = new List<byte>(controllerOutputs.Count);
				var progressData = new ExportProgressStatus(ExportProgressStatus.ProgressType.Task);
	            if (_cancelling == false)
                {
                    sessionData.OutFileName = OutFileName;
                    sessionData.NumPeriods = periods;
                    sessionData.PeriodMS = UpdateInterval;
                    sessionData.ChannelNames = BuildChannelNames(controllers);
                    sessionData.TimeMS = _generator.Sequence.Length.TotalMilliseconds;
                    sessionData.AudioFileName = AudioFilename;
	                try
	                {
		                _output.OpenSession(sessionData);
		                double j = 0;
		                while (_generator.HasNextInterval() && _cancelling == false)
		                {
			                if (progress != null)
			                {
				                progressData.TaskProgressValue = (int)(j / periods * 100);
								progressData.TaskProgressMessage = string.Format("Exporting {0}", _generator.Sequence.Name);
				                progress.Report(progressData);
			                }
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

	[DataContract]
    public class Controller:ICloneable, IEqualityComparer<Controller>
    {
        public Controller(OutputController controller, int index)
        {
            Name = controller.Name;
            Index = index;
            Channels = controller.OutputCount;
	        Id = controller.Id;
	        IsActive = true;
	        if (controller.ControllerModule.SupportsNetwork)
	        {
		        var config = controller.ControllerModule.GetNetworkConfiguration();
		        if (config.SupportsUniverses)
		        {
			        HasNetworkSupport = true;
			        ControllerNetworkConfiguration = config;
				}
	        }
        }

		[DataMember]
        public int Index { get; set; }
		[DataMember]
	    public bool IsActive { get; set; }
	   
		public int Channels { get; set; }
	   
		public string Name { get; set; }
	    [DataMember]
		public Guid Id { get; private set; }

	    public bool HasNetworkSupport { get; set; }

	    public ControllerNetworkConfiguration ControllerNetworkConfiguration { get; private set; }

	    public object Clone()
	    {
			//All my members are value types. If that changes so must this!
		    return MemberwiseClone();
	    }

	    public bool Equals(Controller x, Controller y)
	    {
		    return x.Id == y.Id; //Controller ids determine equality in this case.
	    }

	    public int GetHashCode(Controller obj)
	    {
		    return Id.GetHashCode();
	    }
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