using System;
using System.Collections.Generic;
using System.Linq;
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
		private int _channelCount = 0;
		private int[] _deviceStarts = new int[4];
		private int _Offset;
		private bool[] _validDevices = new bool[4];

		public K8055Module()
		{
			_commandHandler = new K8055CommandHandler();
			DataPolicyFactory = new K8055DataPolicyFactory();
		}

		public override void UpdateState(int chainIndex, ICommand[] outputStates)
		{
			for (int i = 0; i < 4; i++)
			{
				if (_validDevices[i])
				{
					_commandHandler.Reset();
					int start = _deviceStarts[i] - _Offset;
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
			using (Setup setup = new Setup(OutputCount,_deviceStarts))
			{
				if (setup.ShowDialog() == DialogResult.OK)
				{
					//_helixData.EventPeriod = setup.EventData;
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
				_validDevices[i] = (numdevices & (((int)1) << i)) != 0L;
				if (_validDevices[i])
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
				if (_validDevices[i])
				{
					K8055DLLWrapper.Close(i);
				}
			}
			base.Stop();
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
			}
		}
	}
}
