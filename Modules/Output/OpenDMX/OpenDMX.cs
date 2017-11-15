using System;
using System.Runtime.InteropServices;
using System.Threading;
using FTD2XX_NET;
using Vixen.Commands;

namespace VixenModules.Controller.OpenDMX
{
	public class OpenDMX    
	{
		public static FTDI OpenDmxConnection= new FTDI();
        public static byte[] buffer = new byte[513];
		//public static uint handle = 0;
		public static bool done = false;
		public static int bytesWritten = 0;
		//public static FT_STATUS status;
        public static FTDI.FT_STATUS status; 

		private IntPtr transmitPtr = IntPtr.Zero;

	    public const uint BAUDRATE = 250000;
        public const byte BITS_8 = 8;
		public const byte STOP_BITS_2 = 2;
		public const byte PARITY_NONE = 0;
		public const UInt16 FLOW_NONE = 0;
		public const byte PURGE_RX = 1;
		public const byte PURGE_TX = 2;

		/*
        [DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_Open(UInt32 uiPort, ref uint ftHandle);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_Close(uint ftHandle);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_Read(uint ftHandle, IntPtr lpBuffer, UInt32 dwBytesToRead,
		                                       ref UInt32 lpdwBytesReturned);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_Write(uint ftHandle, IntPtr lpBuffer, UInt32 dwBytesToRead,
		                                        ref UInt32 lpdwBytesWritten);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_SetDataCharacteristics(uint ftHandle, byte uWordLength, byte uStopBits, byte uParity);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_SetFlowControl(uint ftHandle, char usFlowControl, byte uXon, byte uXoff);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_GetModemStatus(uint ftHandle, ref UInt32 lpdwModemStatus);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_Purge(uint ftHandle, UInt32 dwMask);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_ClrRts(uint ftHandle);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_SetBreakOn(uint ftHandle);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_SetBreakOff(uint ftHandle);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_GetStatus(uint ftHandle, ref UInt32 lpdwAmountInRxQueue,
		                                            ref UInt32 lpdwAmountInTxQueue, ref UInt32 lpdwEventStatus);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_ResetDevice(uint ftHandle);

		[DllImport("FTD2XX.dll")]
		public static extern FT_STATUS FT_SetDivisor(uint ftHandle, char usDivisor);
       
         */
		public void start()
		{
			
            //handle = 0;
			//status = FT_Open(0, ref handle);
            status = OpenDmxConnection.OpenByIndex(0);

			if (status != FTDI.FT_STATUS.FT_OK) 
            {
				string message = "Failed to open FTDI device" + status.ToString();
				//failure
				throw new Exception(message);
			}

			else {
				//worked

				//Configure the device for DMX data
				initOpenDMX();

				//Initialize the universe and start code to all 0s
				for (int i = 0; i < 513; i++)
					setDmxValue(i, 0);

				//Create and start the thread that sends the output data to the driver
				Thread thread = new Thread(new ThreadStart(writeData));
				thread.Start();
			}
		}

		//Added stop()
		public void stop()
		{
			done = true;
            status = OpenDmxConnection.Close();// FT_Close(handle);
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
			var channelValues = new byte[outputStates.Length];

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
				//FT_GetStatus(handle, ref rxBuf, ref txBuf, ref status);
                OpenDmxConnection.GetTxBytesWaiting(ref txBuf);
                while (txBuf != 0) {
					//Not ready yet, wait for a bit
					Thread.Sleep(2);

					//Check the transmit buffer again
					//FT_GetStatus(handle, ref rxBuf, ref txBuf, ref status);
                    OpenDmxConnection.GetTxBytesWaiting(ref txBuf);
				}

				//Keep buffer from channging while being copied to the output.
				lock (buffer) {
					//Create a break signal in the output before the DMX data.
					//FT_SetBreakOn(handle);
				    OpenDmxConnection.SetBreak(true);
					//FT_SetBreakOff(handle);
				    OpenDmxConnection.SetBreak(false);

					//Send the next frame to the driver
					//bytesWritten = write(handle, buffer, buffer.Length);
				    bytesWritten = write(buffer, buffer.Length);
				}

				//Goto sleep while data is transmitting
				Thread.Sleep(25);
			}

			//Free any used memory from write()
			//Marshal.FreeHGlobal(transmitPtr);
		}

		//public int write(uint handle, byte[] data, int length)
	    public int write(byte[] data, int length)
		{
			//Free the memory from the last call to write()
			//Marshal.FreeHGlobal(transmitPtr);

			//Copy the buffer to a stream of bytes
			//transmitPtr = Marshal.AllocHGlobal((int) length);
			//Marshal.Copy(data, 0, transmitPtr, (int) length);

			uint bytesWritten = 0;

			//Write the data to the serial buffer
			//status = FT_Write(handle, transmitPtr, (uint) length, ref bytesWritten);
		    status = OpenDmxConnection.Write(data, length, ref bytesWritten);
			return (int) bytesWritten;
		}

		public void initOpenDMX()
		{
		    //status = FT_ResetDevice(handle);
            status = OpenDmxConnection.ResetDevice();
            //status = FT_SetDivisor(handle, (char) 12); // set baud rate
		    status = OpenDmxConnection.SetBaudRate(BAUDRATE);
		    //status = FT_SetDataCharacteristics(handle, BITS_8, STOP_BITS_2, PARITY_NONE);
		    status = OpenDmxConnection.SetDataCharacteristics(BITS_8, STOP_BITS_2, PARITY_NONE);
			//status = FT_SetFlowControl(handle, (char) FLOW_NONE, 0, 0);
		    status = OpenDmxConnection.SetFlowControl(FLOW_NONE, 0, 0);
			//status = FT_ClrRts(handle);
		    status = OpenDmxConnection.SetRTS(false);
            //status = FT_Purge(handle, PURGE_TX);
            status = OpenDmxConnection.Purge(PURGE_TX);
            //status = FT_Purge(handle, PURGE_RX);
		    status = OpenDmxConnection.Purge(PURGE_RX);
		}
	}

/*	/// <summary>
	/// Enumaration containing the varios return status for the DLL functions.
	/// </summary>
	public enum FT_STATUS
	{
		FT_OK = 0,
		FT_INVALID_HANDLE,
		FT_DEVICE_NOT_FOUND,
		FT_DEVICE_NOT_OPENED,
		FT_IO_ERROR,
		FT_INSUFFICIENT_RESOURCES,
		FT_INVALID_PARAMETER,
		FT_INVALID_BAUD_RATE,
		FT_DEVICE_NOT_OPENED_FOR_ERASE,
		FT_DEVICE_NOT_OPENED_FOR_WRITE,
		FT_FAILED_TO_WRITE_DEVICE,
		FT_EEPROM_READ_FAILED,
		FT_EEPROM_WRITE_FAILED,
		FT_EEPROM_ERASE_FAILED,
		FT_EEPROM_NOT_PRESENT,
		FT_EEPROM_NOT_PROGRAMMED,
		FT_INVALID_ARGS,
		FT_OTHER_ERROR
	};
 */
}