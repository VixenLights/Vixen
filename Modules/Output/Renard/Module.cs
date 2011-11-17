using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;

namespace VixenModules.Output.Renard
{
	public class Module : OutputModuleInstanceBase {
		private Data _moduleData;

		private byte[] _p1Packet;
		private byte[] _p2Packet;
		private byte[] _p1Zeroes;
		private byte[] _p2Zeroes;
		private Action<Command[]> _updateAction;

		private SerialPort _port;

		// Assume clocks are accurate to 1%, so insert a pad byte every 100 bytes.     
		private const int PAD_DISTANCE = 100;
		private const int DEFAULT_WRITE_TIMEOUT = 500;

		public Module() {
			_SetupP1Packet();
			_SetupP2Packet();
		}

		private void _SetupP1Packet() {
			// 2 possible bytes per channels * 8 channels = 16 + 2 control bytes
			_p1Packet = new byte[18];
			_p1Packet[0] = 0x7e;
			_p1Zeroes = new byte[16];
		}

		private void _SetupP2Packet() {
			// 1 byte per channel * 8 channels = 8 + 3 control bytes = 11
			_p2Packet = new byte[11];
			_p2Packet[0] = 0x00;
			_p2Zeroes = new byte[8];
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override bool Setup() {
			using(CommonElements.SerialPortConfig serialPortConfig = new CommonElements.SerialPortConfig(_port)) {
				if(serialPortConfig.ShowDialog() == DialogResult.OK) {
					SerialPort port = serialPortConfig.SelectedPort;
					_moduleData.PortName = port.PortName;
					_moduleData.BaudRate = port.BaudRate;
					_moduleData.DataBits = port.DataBits;
					_moduleData.Parity = port.Parity;
					_moduleData.StopBits = port.StopBits;
					_UpdateFromData();
					return true;
				}
			}
			return false;
		}

		public override IModuleDataModel ModuleData {
			get { return _moduleData; }
			set {
				_moduleData = value as Data;
				if(_moduleData.WriteTimeout == 0) _moduleData.WriteTimeout = DEFAULT_WRITE_TIMEOUT;
				if(_moduleData.ProtocolVersion == 0) _moduleData.ProtocolVersion = 1;
				_UpdateFromData();
			}
		}

		public override void Start() {
			base.Start();
			if(_port != null) {
				if(!_port.IsOpen) {
					_port.Open();
				}
			}
		}

		public override void Stop() {
			if(_port != null) {
				if(_port.IsOpen) {
					_port.Close();
				}
			}
			base.Stop();
		}


		protected override void _SetOutputCount(int outputCount) { }

		protected override void _UpdateState(Command[] outputStates) {
			if(_port != null) {
				if(_port.IsOpen) {
					_updateAction(outputStates);
				}
			}
		}

		private void _UpdateFromData() {
			_CreatePort();
			_SetUpdateAction();
		}

		private void _CreatePort() {
			if(_port != null) {
				_port.Dispose();
				_port = null;
			}

			if(_moduleData.IsValid) {
				_port = new SerialPort(_moduleData.PortName, _moduleData.BaudRate, _moduleData.Parity, _moduleData.DataBits, _moduleData.StopBits);
				_port.WriteTimeout = _moduleData.WriteTimeout;
				_port.Handshake = Handshake.None;
				_port.Encoding = Encoding.UTF8;
				_port.RtsEnable = true;
				_port.DtrEnable = true;
			} else {
				_port = null;
			}
		}

		private void _SetUpdateAction() {
			_updateAction = (_moduleData.ProtocolVersion == 1) ? (Action<Command[]>)_Protocol1Event : _Protocol2Event;
		}

		private void _Protocol1Event(Command[] outputStates) {
			int src_size = outputStates.Length;
			int dst_index = 2;
			int dst_size = 2 + 2 * src_size + (2 + 2 * src_size) / PAD_DISTANCE;

			// check destination size
			if(_p1Packet.Length < dst_size) {
				_p1Packet = new byte[dst_size];
			}

			_p1Packet[0] = 0x7E;
			_p1Packet[1] = 0x80;

			foreach(Command command in outputStates) {
				if(command == null) {
					// State reset
					_p1Packet[dst_index] = 0;
				}
				// Casting is fasting than comparing strings.
				Lighting.Monochrome.SetLevel setLevelCommand = command as Lighting.Monochrome.SetLevel;
				if(command != null) {
					// Good command
					byte level = (byte)(0xFF * setLevelCommand.Level / 100);
					if(level == 0x7d) {
						_p1Packet[dst_index] = 124;
					} else if(level == 0x7e) {
						_p1Packet[dst_index] = 124;
					} else if(level == 0x7f) {
						_p1Packet[dst_index] = 128;
					} else {
						_p1Packet[dst_index] = level;
					}
				}
				// (Bad commmand - no effect)

				dst_index++;

				if(dst_index % PAD_DISTANCE == 0) {
					_p1Packet[dst_index++] = 0x7D;
				}
			}

			if(IsRunning && dst_index > 2) {
				while(_port.WriteBufferSize - _port.BytesToWrite <= dst_index) {
					System.Threading.Thread.Sleep(10);
				}
				_port.Write(this._p1Packet, 0, dst_index);
			}
		}

		private void _Protocol2Event(Command[] outputStates) {
			int startChannel, endChannel;
			byte picIndex = 0x80;
			int arrayIndex, arrayIndex2;
			int channelCount = outputStates.Length;
			byte offsetByte;
			byte[] usedBytes = new byte[8];
			byte bottomValue, topValue;

			// One whole PIC at a time.
			for(startChannel = 0; startChannel < channelCount; startChannel += 8) {
				endChannel = Math.Min(startChannel + 7, channelCount - 1);
				_p2Packet[1] = picIndex++;

				// Not all pins of the last PIC may be used, so clear the data portion 
				// of the packet if it's the last PIC.
				if(endChannel >= channelCount - 1) {
					_p2Zeroes.CopyTo(_p2Packet, 3);
				}

				// Track the possible offset bytes that are used by packet values
				Array.Clear(usedBytes, 0, 8); // favoring size over speed for this

				// Scan the values for this PIC for the offset byte
				// Need to find a value between 1 and 8 (inclusive)
				// The offset needs to be a value that, when added to any channel value,
				// keeps it from being 0 (including wrap-around).
				for(arrayIndex = startChannel; arrayIndex <= endChannel; arrayIndex++) {
					Command command = outputStates[arrayIndex];
					if(!(command is Lighting.Monochrome.SetLevel)) continue;
					byte level = (byte)(command as Lighting.Monochrome.SetLevel).Level;

					bottomValue = level;
					topValue = (byte)(0 - bottomValue);
					if(bottomValue >= 1 && bottomValue <= 8) {
						usedBytes[bottomValue - 1] = 1;
					} else if(topValue >= 1 && topValue <= 8) {
						usedBytes[topValue - 1] = 1;
					}
				}
				offsetByte = (byte)(1 + Array.IndexOf(usedBytes, (byte)0));
				_p2Packet[2] = offsetByte;

				// Copy values to the packet, adding the offset byte
				for(arrayIndex = startChannel, arrayIndex2 = 3; arrayIndex <= endChannel; arrayIndex++, arrayIndex2++) {
					Command command = outputStates[arrayIndex];
					if(!(command is Lighting.Monochrome.SetLevel)) continue;
					byte level = (byte)(command as Lighting.Monochrome.SetLevel).Level;
					_p2Packet[arrayIndex2] = (byte)(level - offsetByte);
				}

				if(IsRunning) {
					_port.Write(_p2Packet, 0, arrayIndex2);
				}
			}
		}

	}
}
