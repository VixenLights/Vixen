using System;
using System.Threading;
using FTD2XX_NET;
using Vixen.Commands;

namespace VixenModules.Controller.OpenDMX
{
	public class OpenDmx    
	{
	    private readonly FTDI _openDmxConnection= new FTDI();   
        private readonly byte[] _buffer = new byte[513];
	    private bool _done;
	    private FTDI.FT_STATUS _status;

	    private const uint Baudrate = 250000;
	    private const byte Bits8 = 8;
	    private const byte StopBits2 = 2;
	    private const byte ParityNone = 0;
	    private const ushort FlowNone = 0;
	    private const byte PurgeRx = 1;
	    private const byte PurgeTx = 2;

		public void Start()
		{
            _status = _openDmxConnection.OpenByIndex(0);

            if (_status != FTDI.FT_STATUS.FT_OK) //failure
            {
				var message = "Failed to open FTDI device.  Error from Driver: " + _status;
				throw new Exception(message);
			}
			//Initialize the universe and start code to all 0s
		    InitOpenDmx();
            for (var i = 0; i < 513; i++)
				SetDmxValue(i, 0);

			//Create and start the thread that sends the output data to the driver
			var thread = new Thread(WriteData);
			thread.Start();
		}

		public void Stop()
		{
			_done = true;
		    if (_openDmxConnection.IsOpen)
		    {
		        _status = _openDmxConnection.Close();
		    }
		}

	    public void UpdateData(ICommand[] outputStates)
	    {
	        //Make sure that editing the output buffer is thread safe
	        lock (_buffer)
	        {
	            //copy the lighting commands to the DMX Buffer
	            for (var i = 0; i < outputStates.Length; i++)
	            {
	                _8BitCommand command = outputStates[i] as _8BitCommand;

	                //Reset the channel if the command is null
	                if (command == null)
	                {
	                    // State reset
	                    _buffer[i + 1] = 0;
	                    continue;
	                }

	                //Copy the new intensity value to the output buffer
	                _buffer[i + 1] = command.CommandValue;
	            }
	        }
	    }
        
        private void SetDmxValue(int channel, byte value)
		{
			//Lock the buffer for thread safing
			lock (_buffer) {
				if (_buffer != null) {
					_buffer[channel] = value;
				}
			}
		}

	    private void WriteData()
		{
			uint txBuf = 0;
			while (!_done) {
				//Check if all the data has been written yet.
                _openDmxConnection.GetTxBytesWaiting(ref txBuf);
                while (txBuf != 0) {
                    //Not ready yet, wait for a bit and check again
					Thread.Sleep(2);
                    _openDmxConnection.GetTxBytesWaiting(ref txBuf);
				}

				//Keep buffer from channging while being copied to the output.
				lock (_buffer) {
					//Create a break signal in the output before the DMX data.
				    _openDmxConnection.SetBreak(true);
				    _openDmxConnection.SetBreak(false);

					//Send the next frame to the driver
				Write(_buffer, _buffer.Length);
				}

				//Goto sleep while data is transmitting
				Thread.Sleep(25);
			}
		}

	    private void Write(byte[] data, int length)
		{
			uint bytesWritten = 0;

			//Write the data to the serial buffer
		    _status = _openDmxConnection.Write(data, length, ref bytesWritten);
		}

	    private void InitOpenDmx()
		{
            _status = _openDmxConnection.ResetDevice();
		    _status = _openDmxConnection.SetBaudRate(Baudrate);
		    _status = _openDmxConnection.SetDataCharacteristics(Bits8, StopBits2, ParityNone);
		    _status = _openDmxConnection.SetFlowControl(FlowNone, 0, 0);
		    _status = _openDmxConnection.SetRTS(false);
            _status = _openDmxConnection.Purge(PurgeTx);
		    _status = _openDmxConnection.Purge(PurgeRx);
		}
	}

}