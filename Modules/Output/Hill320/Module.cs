using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Sys;

namespace VixenModules.Output.Hill320 {
	internal class LoadInpOutDLL {
		[DllImport("inpout32", EntryPoint = "Out32")]
		private static extern short Out32(ushort port, short data);

		[DllImport("inpoutx64", EntryPoint = "Out32")]
		private static extern short Out64(ushort port, short data);

		public static short outputData(ushort port, short data) {
			if(Environment.Is64BitOperatingSystem) {
				return Out64(port, data);
			} else {
				return Out32(port, data);
			}
		}
	}
	public class Module : ControllerModuleInstanceBase {
		private Data _moduleData;
		private ushort _portAddress;
		private IDataPolicy _dataPolicy;
		private CommandHandler _commandHandler;

		public Module() {
			_dataPolicy = new DataPolicy();
			_commandHandler = new CommandHandler();
		}

		public override bool Setup() {
			using(Common.Controls.ParallelPortConfig parallelPortConfig = new Common.Controls.ParallelPortConfig(_portAddress)) {
				if(parallelPortConfig.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					_portAddress = parallelPortConfig.PortAddress;
					_moduleData.PortAddress = _portAddress;
					_moduleData.StatusPort = (ushort)(_portAddress + 1);
					_moduleData.ControlPort = (ushort)(_portAddress + 2);
					return true;
				}
			}
			return false;
		}

		public override IModuleDataModel ModuleData {
			get { return _moduleData; }
			set { _moduleData = value as Data; }
		}

		protected override void _SetOutputCount(int outputCount) { }

		public override void UpdateState(ICommand[] outputStates) {
			int valueIndex = 0;
			int bitCount;
			byte value;
			byte bankBox, bank;

			int loopCount = outputStates.Length >> 3;

			for(int box = 0; box < loopCount; box++) {
				value = 0;
				for(bitCount = 0; bitCount < 8; bitCount++) {
					ICommand command = outputStates[valueIndex++];
					_commandHandler.Reset();
					if(command != null) {
						command.Dispatch(_commandHandler);
					}

					value >>= 1;
					if(_commandHandler.Value > 0) {
						value |= 0x80;
					}
				}

				bank = (byte)(8 << (box >> 3));
				bankBox = (byte)(box % 8);


				//Write #1
				//Outputs data byte	(D0-D7)	on pins	2-9 of parallel port.  This is the data
				//we are trying to send	to box XX.
				LoadInpOutDLL.outputData(_moduleData.PortAddress, value);


				//Write #2:
				//Outputs a 1 (high) on C0 and C1 (pins 1 and 14) since they are inverted
				//without changing any states on the data pins.  This opens the 
				//"data buffer" flip-flop so that it can read the data on D0-D7.  
				//It also opens up the decoders for each bank solely to	avoid the need for a 7th write.
				LoadInpOutDLL.outputData(_moduleData.ControlPort, 0);


				//Write #3
				//Outputs a 0 (low) on C0 and a 1 (high) on C1 since they are inverted. Again, not
				//changing any states on the data pins.  This "locks" the data presently on	D0-D7
				//into the data buffer (C0) but leaves the state of the decoders (C1) unchanged.
				LoadInpOutDLL.outputData(_moduleData.ControlPort, 1);

				// Write #4
				// Outputs the steering (addressing) data on the data pins
				LoadInpOutDLL.outputData(_moduleData.ControlPort, (short)(bank | bankBox));
				if(value > 0) {
					Console.Out.WriteLine((short)(bank | bankBox) + "," + value);
				}

				//Write	#5
				//Outputs a 0 (low) on both	C0 and C1 since	they are inverted.  This locks
				//the data into	the	correct decoder which sends a "low" single to the clock
				//port of one of the 40	flip flops allowing	the	data from the "buffer" flip	flop
				//to flow in.
				LoadInpOutDLL.outputData(_moduleData.ControlPort, 3);

				//Write #6
				//Outputs a 0 (low) on C0 and a 1 (high) on C1 since they are inverted. Again, not
				//changing any states on the data pins.  This locks	the	data into the correct
				//flip flop and will remain transmitting this data to the relays until the next time
				//this box needs to	be modified.
				LoadInpOutDLL.outputData(_moduleData.ControlPort, 1);
			}
		}

		public override IDataPolicy DataPolicy {
			get { return _dataPolicy; }
		}
	}
}
