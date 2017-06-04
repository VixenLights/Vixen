using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace VixenModules.Output.LauncherController
{
	public class Module : ControllerModuleInstanceBase
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private Data _Data;
		public Module()
		{
			DataPolicyFactory = new DataPolicyFactory();
		}

		internal static bool Launch(Data launcherData, string executable, string arguments)
		{
			if (File.Exists(executable))
			{
				Logging.Info("Launching Executable: {0} with arguments [{1}]", executable, arguments);
				Task.Factory.StartNew(() => {
					try {
						Stopwatch w = Stopwatch.StartNew();
						Process proc = new Process();

						proc.StartInfo.FileName = executable;
						if (!string.IsNullOrWhiteSpace(arguments))
							proc.StartInfo.Arguments = arguments;

						proc.StartInfo.CreateNoWindow = launcherData.HideLaunchedWindows;

						proc.Start();
						proc.WaitForExit();
						w.Stop();
						Logging.Info("Process {0} Completed After {1} with Exit code {2}", executable, w.Elapsed, proc.ExitCode);

					} catch (Exception e) {

						Logging.Error(e, e.Message);
					}

				});
			} else
				Logging.Error("File Not found to Launch: [{0}]", executable);
			return false;
		}

		private Dictionary<int, string> _lastCommandValues = new Dictionary<int, string>();

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			for( int idx=0; idx<outputStates.Length; idx++) {
				var item = outputStates[idx];
				var cmd = item as StringCommand;
				if( cmd == null) {
					_lastCommandValues[idx] = null;
					continue;
				}
				String lastVal;
				_lastCommandValues.TryGetValue(idx, out lastVal);
				_lastCommandValues[idx] = cmd.CommandValue;

				var cmdType = cmd.CommandValue.Split('|')[0];
				switch (cmdType.ToUpper()) {
					case "LAUNCHER":
						if (lastVal != null && cmd.CommandValue.Equals(lastVal))
						{
							continue;
						}
						var args = cmd.CommandValue.Split('|')[1].Split(',');
						Launch(_Data, args[0], args[1]);							 
						break;
				}
				Logging.Info("Launcher Value Sent: " + cmd.CommandValue);
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

	}
}