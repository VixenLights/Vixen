using System;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls;
using Vixen.Module.Controller;
using Vixen.Commands;

namespace VixenModules.Controller.OpenDMX
{
	public class VixenOpenDMXInstance : ControllerModuleInstanceBase
	{
		private FTDI _dmxPort = new FTDI();
		private int _outputCount;

		public VixenOpenDMXInstance()
		{
			DataPolicyFactory = new DataPolicyFactory();
		}

		public override int OutputCount
		{
			get { return _outputCount; }

			set
			{
				if (value > 512) {
					throw new Exception("Output count greater than 512");
				}
				else {
					_outputCount = value;
				}
			}
		}

		public override void UpdateState(int chainInex, ICommand[] outputStates)
		{
			//Pass the lighting data onto the hardware controller class
			_dmxPort.updateData(outputStates);
		}

		public override bool HasSetup
		{
			get { return true; }
		}

		public override bool Setup()
		{
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("Nothing to Setup", "", false, false);
			messageBox.ShowDialog();
			return base.Setup();
		}

		public override void Start()
		{
			base.Start();
			//Open up FTDI interface
			_dmxPort.start();
		}

		public override void Stop()
		{
			base.Stop();

			//Close FTDI interface
			_dmxPort.stop();
		}
	}
}