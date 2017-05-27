using System.Drawing.Printing;
using System.IO.Ports;
using System.Text;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;
using System.Timers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace VixenModules.Output.LauncherController
{
	public class Module : ControllerModuleInstanceBase
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private Data _Data;
		private CommandHandler _commandHandler;
		public Module()
		{
			DataPolicyFactory = new DataPolicyFactory();
			_commandHandler = new CommandHandler();
		}

        internal static bool Launch(Data RdsData, string Executable, string Arguments)
		{
			if (File.Exists(Executable)) {
				Logging.Info(string.Format("Launching Executable: {0} with arguments [{1}]", Executable, Arguments));
				Task.Factory.StartNew(() => {
					try {
						Stopwatch w = Stopwatch.StartNew();
						Process proc = new Process();

						proc.StartInfo.FileName   = Executable;
						if (!string.IsNullOrWhiteSpace(Arguments))
							proc.StartInfo.Arguments = Arguments;

						proc.StartInfo.CreateNoWindow= RdsData.HideLaunchedWindows;

						proc.Start();
						proc.WaitForExit();
						w.Stop();
						Logging.Info(string.Format("Process {0} Completed After {1} with Exit code {2}", Executable, w.Elapsed, proc.ExitCode));

					} catch (Exception e) {

						Logging.Error(e.Message, e);
					}

				});
			} else
				Logging.Error(string.Format("File Not found to Launch: [{0}]", Executable));
			return false;
		}

		private Dictionary<int, string> lastCommandValues = new Dictionary<int, string>();

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			for( int idx=0; idx<outputStates.Length; idx++) {
				var item = outputStates[idx];
				var cmd = item as StringCommand;
				if( cmd == null) {
					lastCommandValues[idx] = null;
					continue;
				}
				String lastVal;
				lastCommandValues.TryGetValue(idx, out lastVal);				
				lastCommandValues[idx] = cmd.CommandValue;

				var cmdType = cmd.CommandValue.Split('|')[0];
				switch (cmdType.ToUpper()) {
                    case "LAUNCHER":
						if (lastVal != null && cmd.CommandValue.Equals(lastVal))
						{
							continue;
						}
						var args = cmd.CommandValue.Split('|')[1].Split(',');
						Module.Launch(_Data, args[0], args[1]);							 
						break;
				}
				Logging.Info("RDS Value Sent: " + cmd.CommandValue);
			}
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			using (var setupForm = new SetupForm(_Data)) {
				if (setupForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
                {
                    _Data = setupForm.LauncherData;
					return true;
				}
				return false;
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _Data; }
			set
			{
				_Data = (Data)value;
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
	}
}