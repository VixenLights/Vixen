using System.IO.Ports;
using System.Text;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;
using System.Timers;
using System;
using System.IO;
using System.Linq;

namespace VixenModules.Output.RDSController
{
	public class Module : ControllerModuleInstanceBase
	{
		private Data _Data;
		private CommandHandler _commandHandler;
		public Module()
		{
			DataPolicyFactory = new DataPolicyFactory();
			_commandHandler = new CommandHandler();
		}

		internal static bool Send(Data RdsData, string rdsText)
		{
			switch (RdsData.HardwareID) {
				case Hardware.MRDS192:
				case Hardware.MRDS1322:
					NativeMethods.ConnectionSetup(RdsData.ConnectionMode, RdsData.PortNumber, RdsData.BiDirectional, RdsData.Slow);
					if (NativeMethods.Connect()) {
						try {


							byte[] Data = new byte[9];
							int i, Len;
							Data[0] = 0x02;             // buffer address
							Len = 8;
							for (i = 1; i <= Len; i++)
								Data[i] = 0x20; // fill 8 blank spaces first
							//Data[0] = 0x02;
							//Len = 8;
							for (i = 0; i < rdsText.Length; i++) {
								byte vOut = Convert.ToByte(rdsText[i]);
								Data[i + 1] = vOut;
							}

							if (NativeMethods.Send(Len, Data))
								return true;
							else
								return false;

						} finally {
							NativeMethods.Disconnect();
						}
					}
					return false;
				case Hardware.VFMT212R:
					throw new NotImplementedException();
				default:
					return false;
			}


		}



		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			foreach (var item in outputStates.Where(i => i != null)) {
				var cmd = item as StringCommand;
				if (cmd != null) {
					Console.WriteLine("RDS Value Sent: " + cmd.CommandValue);
					Module.Send(_Data, cmd.CommandValue);
				}
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