using System;
using System.Threading;
using FTD2XX_NET;
using Vixen.Commands;

namespace VixenModules.Controller.OpenDMX
{
	public class OpenDMX    
	{
		public static FTDI OpenDmxConnection= new FTDI();
        public static byte[] buffer = new byte[513];
		public static bool done = false;
		public static int bytesWritten = 0;
        public static FTDI.FT_STATUS status; 

	    public const uint BAUDRATE = 250000;
        public const byte BITS_8 = 8;
		public const byte STOP_BITS_2 = 2;
		public const byte PARITY_NONE = 0;
		public const UInt16 FLOW_NONE = 0;
		public const byte PURGE_RX = 1;
		public const byte PURGE_TX = 2;

		public void start()
		{
            status = OpenDmxConnection.OpenByIndex(0);

            if (status != FTDI.FT_STATUS.FT_OK) //failure
            {
				string message = "Failed to open FTDI device.  Error from Driver: " + status;//.ToString();
				throw new Exception(message);
			}

            else //Success
            {
				initOpenDMX();

				//Initialize the universe and start code to all 0s
				for (int i = 0; i < 513; i++)
					setDmxValue(i, 0);

				//Create and start the thread that sends the output data to the driver
				Thread thread = new Thread(new ThreadStart(writeData));
				thread.Start();
			}
		}

		public void stop()
		{
			done = true;
            status = OpenDmxConnection.Close();
		}

		public void setDmxValue(int channel, byte value)
		{
			//Lock the buffer for thread safing
			lock (buffer) {
				if (buffer != null) {
					buffer[channel] = value;
				}
			}
		}

		public void updateData(ICommand[] outputStates)
		{
			//Make sure that editing the output buffer is thread safe
			lock (buffer) {
				//copy the lighting commands to the DMX Buffer
				for (int i = 0; i < outputStates.Length; i++) {
					_8BitCommand command = outputStates[i] as _8BitCommand;

					//Reset the channel if the command is null
					if (command == null) {
						// State reset
						buffer[i + 1] = 0;
						continue;
					}

					//Copy the new intensity value to the output buffer
					buffer[i + 1] = command.CommandValue;
				}
			}
		}

		public void writeData()
		{
			uint txBuf = 0;
			while (!done) {
				//Check if all the data has been written yet.
                OpenDmxConnection.GetTxBytesWaiting(ref txBuf);
                while (txBuf != 0) {
                    //Not ready yet, wait for a bit and check again
					Thread.Sleep(2);
                    OpenDmxConnection.GetTxBytesWaiting(ref txBuf);
				}

				//Keep buffer from channging while being copied to the output.
				lock (buffer) {
					//Create a break signal in the output before the DMX data.
				    OpenDmxConnection.SetBreak(true);
				    OpenDmxConnection.SetBreak(false);

					//Send the next frame to the driver
				    bytesWritten = write(buffer, buffer.Length);
				}

				//Goto sleep while data is transmitting
				Thread.Sleep(25);
			}
		}

	    public int write(byte[] data, int length)
		{
			uint bytesWritten = 0;

			//Write the data to the serial buffer
		    status = OpenDmxConnection.Write(data, length, ref bytesWritten);
			return (int) bytesWritten;
		}

		public void initOpenDMX()
		{
            status = OpenDmxConnection.ResetDevice();
		    status = OpenDmxConnection.SetBaudRate(BAUDRATE);
		    status = OpenDmxConnection.SetDataCharacteristics(BITS_8, STOP_BITS_2, PARITY_NONE);
		    status = OpenDmxConnection.SetFlowControl(FLOW_NONE, 0, 0);
		    status = OpenDmxConnection.SetRTS(false);
            status = OpenDmxConnection.Purge(PURGE_TX);
		    status = OpenDmxConnection.Purge(PURGE_RX);
		}
	}

}