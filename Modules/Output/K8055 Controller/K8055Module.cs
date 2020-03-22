using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;

namespace VixenModules.Output.K8055_Controller
{
	public class K8055Module : ControllerModuleInstanceBase
	{
		private K8055Data _Data;
		private K8055CommandHandler _commandHandler;
		private K8055ControlModule[] _modules;
		private int _Offset;

		public K8055Module()
		{
			_commandHandler = new K8055CommandHandler();
			DataPolicyFactory = new K8055DataPolicyFactory();

			_modules = new K8055ControlModule[4];
			for (int i = 0; i < 4; i++)
			{
				_modules[i] = new K8055ControlModule(i + 1);
			}
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			for (int i = 0; i < 4; i++)
			{
				if (_Data.Modules[i].Enabled)
				{
					_commandHandler.Reset();
					int start = _Data.Modules[i].StartChannel - _Offset;
					int end = Math.Min(start + 8, outputStates.Length);
					byte data = 0;
					ICommand command = outputStates[start++];
					if (command != null)
					{
						while (start < end)
						{
							data >>= 1;
							if (_commandHandler.Value > 0)
							{
								data |= (byte)0x80;
							}
							else
							{
								data |= (byte)0;
							}
						}
					}
					else
					{
						data >>= 1;
						data |= (byte)0;
					}
					K8055DLLWrapper.Write(i, (long)data);
				}
			}
		}

		public override bool Setup()
		{
			using (Setup setup = new Setup(OutputCount, _Data))
			{
				if (setup.ShowDialog() == DialogResult.OK)
				{
					for (int i = 0; i < 4; i++)
					{
						_modules[i] = setup.Modules[i];
					}
					_Data.Modules = _modules;
					return true;
				}
			}

			return false;
		}

		public override void Start()
		{
			long numdevices = K8055DLLWrapper.SearchDevices();
			for (int i = 0; i < 4; i++)
			{
				_Data.Modules[i].Enabled = (numdevices & (((int)1) << i)) != 0L;
				if (_Data.Modules[i].Enabled)
				{
					K8055DLLWrapper.Open(i);
				}
			}
			base.Start();
		}

		public override void Stop()
		{
			for (int i = 0; i < 4; i++)
			{
				if (_Data.Modules[i].Enabled)
				{
					K8055DLLWrapper.Close(i);
				}
			}
			base.Stop();
		}

		private void initModule()
		{
			if (_Data.Modules != null)
			{
				for (int i = 0; i < 4; i++)
				{
					if (_Data.Modules[i] != null)
					{
						_modules[i].Enabled = _Data.Modules[i].Enabled;
						if (_modules[i].Enabled)
						{
							_modules[i].StartChannel = _Data.Modules[i].StartChannel;
						}
					}
				}
			}
			else
			{
				//hasn't been configured yet so lets set some defaults.
				_Data.Modules = _modules;
			}
		}

		public override IModuleDataModel ModuleData
		{
			get
			{
				return _Data;
			}
			set
			{
				_Data = (K8055Data)value;
				initModule();
			}
		}
	}
}
