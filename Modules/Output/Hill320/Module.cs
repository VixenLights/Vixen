using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Vixen.Module;
using Vixen.Module.Output;
using Vixen.Commands;

namespace VixenModules.Output.Hill320
{
    public class Module : OutputModuleInstanceBase
    {
        [DllImport("inpout32", EntryPoint = "Inp32")]
        //[DllImport("inpoutx64", EntryPoint = "Inp32")]
        private static extern short In(ushort port);

        [DllImport("inpout32", EntryPoint = "Out32")]
        //[DllImport("inpoutx64", EntryPoint = "Out32")]
        private static extern void Out(ushort port, short data);

        private Data _moduleData;
        private ushort _portAddress;

        public override bool Setup()
        {
            using (CommonElements.ParallelPortConfig parallelPortConfig = new CommonElements.ParallelPortConfig(_portAddress))
            {
                if (parallelPortConfig.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _portAddress = parallelPortConfig.PortAddress;
                    _moduleData.PortAddress = _portAddress;
                    _moduleData.StatusPort = (ushort)(_portAddress + 1);
                    _moduleData.ControlPort = (ushort)(_portAddress + 2);
                    return true;
                }
            }
            return false;
        }

        public override IModuleDataModel ModuleData
        {
            get { return _moduleData; }
            set { _moduleData = value as Data; }
        }

        protected override void _SetOutputCount(int outputCount) { }

        protected override void _UpdateState(Command[] outputStates)
        {

            int valueIndex = 0;
            int bitCount;
            byte value;
            byte bankBox, bank;
            int loopCount = outputStates.Length >> 3;

            for (int box = 0; box < loopCount; box++)
            {
                value = 0;
                for (bitCount = 0; bitCount < 8; bitCount++)
                {
                    Lighting.Monochrome.SetLevel setLevelCommand = outputStates[valueIndex++] as Lighting.Monochrome.SetLevel;

                        value >>= 1;
                        if (setLevelCommand == null)
                        {
                            value |= (byte)0;
                        }
                        else
                        {
                            if (setLevelCommand.Level > 0)
                                value |= (byte)0x80;
                            else
                                value |= (byte)0;
                        }
                }
                bank = (byte)(8 << (box >> 3));
                bankBox = (byte)(box % 8);

                //Write #1
                //Outputs data byte	(D0-D7)	on pins	2-9 of parallel port.  This is the data
                //we are trying to send	to box XX.
                Out(_moduleData.PortAddress, value);
                
                //Write #2:
                //Outputs a 1 (high) on C0 and C1 (pins 1 and 14) since they are inverted
                //without changing any states on the data pins.  This opens the 
                //"data buffer" flip-flop so that it can read the data on D0-D7.  
                //It also opens up the decoders for each bank solely to	avoid the need for a 7th write.
                Out(_moduleData.ControlPort, 0);

                //Write #3
                //Outputs a 0 (low) on C0 and a 1 (high) on C1 since they are inverted. Again, not
                //changing any states on the data pins.  This "locks" the data presently on	D0-D7
                //into the data buffer (C0) but leaves the state of the decoders (C1) unchanged.
                Out(_moduleData.ControlPort, 1);

                // Write #4
                // Outputs the steering (addressing) data on the data pins
                Out(_moduleData.ControlPort, (short)(bank | bankBox));
                //Console.Out.WriteLine(String.Format("{0} {1}", value, (short)(bank | bankBox)));
                
                //Write	#5
                //Outputs a 0 (low) on both	C0 and C1 since	they are inverted.  This locks
                //the data into	the	correct decoder which sends a "low" single to the clock
                //port of one of the 40	flip flops allowing	the	data from the "buffer" flip	flop
                //to flow in.
                Out(_moduleData.ControlPort, 3);

                //Write #6
                //Outputs a 0 (low) on C0 and a 1 (high) on C1 since they are inverted. Again, not
                //changing any states on the data pins.  This locks	the	data into the correct
                //flip flop and will remain transmitting this data to the relays until the next time
                //this box needs to	be modified.
                Out(_moduleData.ControlPort, 1);

            }
        }
    }
}
