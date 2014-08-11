using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Module.Timing;
using Vixen.Module.App;
using Vixen.Services;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Factory;
using Vixen.Sys;
using Vixen.Sys.Output;
using NLog;

namespace Vixen.Export
{
    public class Export
    {
        private ISequenceContext _context = null;
        private OutputController _outputController = null;
        Guid _controllerTypeId = new Guid("{F79764D7-5153-41C6-913C-2321BC2E1819}");
		List<OutputController> _nonExportControllers = null;
		private const string EXPORT_CONTROLLER_NAME = "ExportGateway";

        private IExportWriter _output;
        private Dictionary<string, IExportWriter> _writers = null;
        private Dictionary<string, string> _exportFileTypes = null;


        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();


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

		public Dictionary<string, string> ExportFileTypes
		{
			get
			{
                return _exportFileTypes;
			}
		}

        private OutputController FindExportControler()
        {
            return
                VixenSystem.OutputControllers.ToList().Find(x => x.ModuleId.Equals(_controllerTypeId));
        }

        private List<OutputController> NonExportControllers
        {
			get
			{
				if (_nonExportControllers == null)
				{
					_nonExportControllers = VixenSystem.OutputControllers.ToList().FindAll(x => x.ModuleId != _controllerTypeId);
				}
				return _nonExportControllers;
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

			using (XmlWriter writer = XmlWriter.Create(xmlOutName,settings))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("Vixen3_Export");
				writer.WriteElementString("Resolution", UpdateInterval.ToString());
				writer.WriteElementString("OutFile", Path.GetFileName(OutFileName));
				writer.WriteElementString("Duration", sequence.Length.ToString());

				writer.WriteStartElement("Network");
				foreach (ControllerExportInfo exportInfo in ControllerExportData)
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

        public void DoExport(ISequence sequence)
        {
/*

            if (sequence != null)
            {
				PopulateControllerCommands();

                string[] timingSources;
                TimingProviders timingProviders = new TimingProviders(sequence);
                timingSources = timingProviders.GetAvailableTimingSources("Export");

                if (timingSources.Length > 0)
                {
                    SelectedTimingProvider exportTimingProvider = new SelectedTimingProvider("Export", timingSources.First());
                    sequence.SelectedTimingProvider = exportTimingProvider;
                }

                _context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), sequence);
                if (_context == null)
                {
                   // Logging.Error(@"Null context when attempting to play sequence.");
                    MessageBox.Show(@"Unable to play this sequence.  See error log for details.");
                    return;
                }

				WriteControllerInfo(_context.Sequence);

				_context.Sequence.ClearMedia();
				
                _context.Play(TimeSpan.Zero, TimeSpan.MaxValue);

            }
 */
        }

        public double SequenceLength
        {
            get
            {
                double retVal = 0;
                if (_context != null)
                {
                    retVal = _context.Sequence.Length.TotalMilliseconds;
                }
                return retVal;
            }
        }

        public ITiming SequenceTiming
        {
            get
            {
                ITiming retVal = null;
                if (_context != null)
                {
                    retVal = _context.Sequence.GetTiming();
                }
                return retVal;
            }
        }

        public List<ControllerExportInfo> ControllerExportData
        {
            get
            {
                int index = 0;
                List<ControllerExportInfo> retVal = new List<ControllerExportInfo>();
                NonExportControllers.ForEach(x => retVal.Add(new ControllerExportInfo(x, index++)));
                return retVal;
            }
        }
    }

    public class ControllerExportInfo
    {
        public ControllerExportInfo(OutputController controller, int index)
        {
            Name = controller.Name;
            Index = index;
            Channels = controller.OutputCount;
        }

        public int Index { get; set; }
        public int Channels { get; set; }
        public string Name { get; set; }
    }
}