using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace VixenModules.Output.CommandController
{
	public class Module : ControllerModuleInstanceBase
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private Data _Data;
		public Module()
		{
			DataPolicyFactory = new DataPolicyFactory();
		}
		static string lastRdsText= string.Empty;
		internal static bool Send(Data RdsData, string rdsText, string rdsArtist = null, bool sendps=false)
		{
			// Darren... is this still needed?
			//We dont want to keep hammering the RDS device with updates if they havent changed.
			if (lastRdsText.Equals(rdsText))
				return true;
			else
				lastRdsText=rdsText;

			Console.WriteLine("Sending {0}: {1}", rdsText, DateTime.Now);
			switch (RdsData.HardwareID) {
				case Hardware.MRDS192:
				case Hardware.MRDS1322:
					var portName = string.Format("COM{0}", RdsData.PortNumber);
					using (var rdsPort = new RdsSerialPort(portName, RdsData.Slow ? 2400 : 19200))
					{
						rdsPort.Send(rdsText);
					}
					return false;
				case Hardware.VFMT212R:
				case Hardware.HTTP:
					Task.Factory.StartNew(() => {
						try {
							//string url = RdsData.HttpUrl.ToLower().Replace("{text}",HttpUtility.UrlEncode(rdsText)).Replace("{time}", HttpUtility.UrlEncode(DateTime.Now.ToLocalTime().ToShortTimeString()));
							//JC 11/27/16- replaced with line below to remove lowercase force
							string url = RdsData.HttpUrl.Replace("{text}", HttpUtility.UrlEncode(rdsText)).Replace("{time}", HttpUtility.UrlEncode(DateTime.Now.ToLocalTime().ToShortTimeString()));
							if (sendps) {
								//TODO: Enable the sendps code here.
							}
							WebRequest request = WebRequest.Create(url);
							if (RdsData.RequireHTTPAuthentication) {
								request.Credentials= new NetworkCredential(RdsData.HttpUsername, RdsData.HttpPassword);
							}
						} catch (Exception e) {
							Logging.Error(e, e.Message);
							lastRdsText=string.Empty;
						}
					});
					return true;
				default:
					return false;
			}
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
					case "RDS":
						Module.Send(_Data, cmd.CommandValue.Split('|')[1]);							 
						break;
					case "LAUNCHER":
						if (lastVal != null && cmd.CommandValue.Equals(lastVal))
						{
							// no repeats for us
							continue;
						}
						var args = cmd.CommandValue.Split('|')[1].Split(',');

						Launch(_Data, args[0], args[1]);
							 
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
				if (setupForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					_Data= setupForm.RdsData;
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