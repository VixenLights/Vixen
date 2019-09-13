using System;
using System.Windows.Forms;
using Vixen.Module.Controller;
using Vixen.Commands;
using Vixen.Module;

namespace VixenModules.Controller.OpenDMX
{
	public class VixenOpenDMXInstance : ControllerModuleInstanceBase
	{
		private OpenDmx _dmxPort;
		private int _outputCount;
		private OpenDMXData _moduleData;

		public VixenOpenDMXInstance()
		{
			_moduleData = new OpenDMXData();
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

		public override IModuleDataModel ModuleData
		{
			get => _moduleData;
			set => _moduleData = value as OpenDMXData;
		}

		public override void UpdateState(int chainInex, ICommand[] outputStates)
		{
			//Pass the lighting data onto the hardware controller class
			_dmxPort.UpdateData(outputStates);
		}

		public override bool HasSetup => true;

		public override bool Setup()
		{
			SetupDialog dialog = new SetupDialog(_moduleData);
			var dr = dialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				_dmxPort.Stop();
				_dmxPort.Start();
			}

			return true;
		}

		public override void Start()
		{
			base.Start();
			if (_dmxPort == null)
			{
				_dmxPort = new OpenDmx(_moduleData);
			}
			//Open up FTDI interface
			_dmxPort.Start();
		}

		public override void Stop()
		{
			base.Stop();

			//Close FTDI interface
			_dmxPort.Stop();
		}
	}
}