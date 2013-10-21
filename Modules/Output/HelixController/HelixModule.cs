using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Execution;
using Vixen.Commands;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace VixenModules.Output.HelixController
{
	public class HelixModule : ControllerModuleInstanceBase
	{
		private bool _sequenceStarted = false;
		private HelixData _helixData;
		private HelixCommandHandler _helixCommandHandler;
		private VixenXmlOutput _output;
		private List<byte> _eventData;
		private Stopwatch _timer;
		private long _lastUpdateMs;

		

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public HelixModule()
		{
			_helixCommandHandler = new HelixCommandHandler();
			DataPolicyFactory = new HelixDataPolicyFactory();
						

			VixenSystem.Contexts.ContextCreated += Contexts_ContextCreated;
			VixenSystem.Contexts.ContextReleased += Contexts_ContextReleased;

		}

		void Contexts_ContextCreated(object sender, ContextEventArgs e)
		{
			IContext sequenceContext = e.Context as IContext;

			if (sequenceContext != null)
			{
				sequenceContext.ContextStarted += sequenceContext_ContextStarted;
				sequenceContext.ContextEnded += sequenceContext_ContextEnded;

			}
		}


		void Contexts_ContextReleased(object sender, ContextEventArgs e)
		{

			IContext sequenceContext = e.Context as IContext;
			if (sequenceContext != null)
			{
				sequenceContext.ContextStarted -= sequenceContext_ContextStarted;
				sequenceContext.ContextEnded -= sequenceContext_ContextEnded;

			}

		}

		void sequenceContext_ContextEnded(object sender, EventArgs e)
		{
			_sequenceStarted = false;
			BuildEventData();
			BuildChannelList();
			string outputData = BuildVixenOutputFile();
			WriteOutputFile(outputData);
		}

		void sequenceContext_ContextStarted(object sender, EventArgs e)
		{

			Vixen.Execution.Context.ISequenceContext sequenceContext = (Vixen.Execution.Context.ISequenceContext)sender;

			_timer = new Stopwatch();
			_eventData = new List<byte>();
			_output = new VixenXmlOutput() { Audio = new Audio(), Channels = new List<string>() };

			_output.Audio.filename = sequenceContext.Sequence.SequenceData.SelectedTimingProvider.SourceName;
			string audioname = _output.Audio.filename.Substring(_output.Audio.filename.LastIndexOf("\\") + 1);

			_output.Audio.Value = audioname.Substring(0, audioname.LastIndexOf("."));

			_output.Time = sequenceContext.Sequence.Length.TotalMilliseconds.ToString();

			_output.EventPeriodInMilliseconds = _helixData.EventPeriod.ToString();

			_timer.Start();
			_sequenceStarted = true;

		}

		public override bool Setup()
		{
			using (HelixSetup setup = new HelixSetup())
			{
				if (setup.ShowDialog() == DialogResult.OK)
				{
					_helixData.EventPeriod = setup.EventData;
					return true;
				}
			}

			return false;
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			if (_sequenceStarted)
			{
				if (_timer.ElapsedMilliseconds < _lastUpdateMs + MsPerUpdate)
				{
					return;
				}

				_lastUpdateMs = _timer.ElapsedMilliseconds;

				for (int i = 0; i < outputStates.Length; i++)
				{
					_helixCommandHandler.Reset();
					ICommand command = outputStates[i];
					if (command != null)
					{
						command.Dispatch(_helixCommandHandler);
					}
					_eventData.Add(_helixCommandHandler.Value);
				}
			}
		}

		public override void Start()
		{
			base.Start();
		}
		public override void Stop()
		{
			base.Stop();
		}

		public override Vixen.Module.IModuleDataModel ModuleData
		{
			get
			{
				return _helixData;
			}
			set
			{
				_helixData = (HelixData)value;
				initModule();
			}
		}

		private void initModule()
		{
			MsPerUpdate = _helixData.EventPeriod;
			_lastUpdateMs = int.MinValue;
		}

		private void BuildEventData()
		{
			byte[] eventdata = _eventData.ToArray<byte>();
			_output.EventValues = Convert.ToBase64String(eventdata);

		}

		private void BuildChannelList()
		{
			for (int i = 0; i < OutputCount; i++)
			{
				_output.Channels.Add(i.ToString());
			}

		}

		private string BuildVixenOutputFile()
		{
			StringBuilder sb = new StringBuilder();
			// Remove the <?xml... /> tag from the start of the xml
			XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true };

			XmlWriter writer = XmlWriter.Create(sb, settings);

			// Remove namespace references
			XmlSerializerNamespaces n = new XmlSerializerNamespaces();
			n.Add("", "");

			XmlSerializer serializer = new XmlSerializer(typeof(VixenXmlOutput));
			serializer.Serialize(writer, _output, n);

			return sb.ToString();
		}

		private void WriteOutputFile(string outputData)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\vixen";

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			if (File.Exists(string.Format("{0}{1}.vix", path + "\\", _output.Audio.Value)))
			{
				File.Move(string.Format("{0}{1}.vix", path + "\\", _output.Audio.Value), string.Format(path + "\\" + "{0}_{1:hhmmss}.vix", _output.Audio.Value, DateTime.Now));
			}

			File.AppendAllText(string.Format("{0}{1}.vix", path + "\\", _output.Audio.Value), outputData);
		}

		public int MsPerUpdate { get; set; }
	}

}
