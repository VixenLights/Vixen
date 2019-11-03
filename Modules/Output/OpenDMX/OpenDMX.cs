using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FTD2XX_NET;
using NLog;
using Vixen.Commands;

namespace VixenModules.Controller.OpenDMX
{
	public class OpenDmx    
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
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
	    private OpenDMXData _data;

	    public OpenDmx(OpenDMXData data)
	    {
		    _data = data;
	    }

		public void Start()
		{
			Logging.Info("Starting OpenDmx");
			Device device = _data.Device;
			var deviceFound = false;
			if (device != null)
			{
				var i = FindDeviceIndex(device);
				if (i >= 0)
				{
					deviceFound = true;
					Logging.Info($"Specified OpenDMX device {device} found at index: {i}");
				}
			}
			
			if(device == null || !deviceFound)
			{
				device = GetDefaultDevice();
				if (device != null)
				{
					deviceFound = true;
					_data.Device = device;
					Logging.Info($"Using default OpenDMX device {device}");
				}
			}
			
			
			//try to open the device
			if (deviceFound)
			{
				Logging.Info($"Attempting to open OpenDMX device {device}");
				Logging.Info($"Attempting to open by serial number {device.SerialNumber}");
				_status = _openDmxConnection.OpenBySerialNumber(device.SerialNumber);
			}
			else
			{
				Logging.Error("No devices found to open.");
				var message = "No devices found to open.";
				throw new Exception(message);
			}
			
			if (_status != FTDI.FT_STATUS.FT_OK) //failure
			{
				Logging.Error($"Error opening the OpenDMX device {device} : {_status}");
				var message = "Failed to open OpenDMX device. Error from Driver: " + _status;
				throw new Exception(message);
			}
			
			//Initialize the universe and start code to all 0s
		    InitOpenDmx();
            for (var i = 0; i < 513; i++)
				SetDmxValue(i, 0);

			//Create and start the thread that sends the output data to the driver
			var thread = new Thread(WriteData);
			thread.Start();
			Logging.Info($"Open OpenDMX device {device} successful");
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

				//Keep buffer from changing while being copied to the output.
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
            Logging.Info($"Reset device: {_status}");
		    _status = _openDmxConnection.SetBaudRate(Baudrate);
		    Logging.Info($"Set device baudrate: {_status}");
		    _status = _openDmxConnection.SetDataCharacteristics(Bits8, StopBits2, ParityNone);
		    Logging.Info($"Set device data characteristics: {_status}");
		    _status = _openDmxConnection.SetFlowControl(FlowNone, 0, 0);
		    Logging.Info($"Set device flow control: {_status}");
		    _status = _openDmxConnection.SetRTS(false);
		    Logging.Info($"Set device RTS: {_status}");
            _status = _openDmxConnection.Purge(PurgeTx);
            Logging.Info($"Purge TX: {_status}");
		    _status = _openDmxConnection.Purge(PurgeRx);
		    Logging.Info($"Purge RX: {_status}");
		}

	    public int FindDeviceIndex(Device device)
	    {
		    var devices = GetDeviceList();
		    return devices.FindIndex(x => x.Id == device.Id && x.SerialNumber == device.SerialNumber && x.Description == device.Description);
	    }

	    public Device GetDefaultDevice()
	    {
		    var devices = GetDeviceList();
		    if (devices.Any())
		    {
			    return devices[0];
		    }

		    return null;
	    }

	    public static List<Device> GetDeviceList()
	    {
			List<Device> devices = new List<Device>();
		    UInt32 ftdiDeviceCount = 0;
		    // Create new instance of the FTDI device class
		    FTDI tempFtdiDevice = new FTDI();
		    Logging.Info("Interogating FTDI for devices.");
		    // Determine the number of FTDI devices connected to the machine
		    FTDI.FT_STATUS ftStatus = tempFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
		    // Check status
		    if (ftStatus != FTDI.FT_STATUS.FT_OK)
		    {
			    Logging.Error($"An error occured getting FTDI devices. {ftStatus.ToString()}");
			    return devices;
		    }

		    if (ftdiDeviceCount > 0)
		    {
			    // Allocate storage for device info list
			    FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
			    // Populate our device list
			    ftStatus = tempFtdiDevice.GetDeviceList(ftdiDeviceList);
			    //Show device properties
			    if (ftStatus == FTDI.FT_STATUS.FT_OK)
			    {
				    for (var i = 0; i < ftdiDeviceCount; i++)
				    {
					    Device d = new Device
					    {
						    Index = i,
						    Type = ftdiDeviceList[i].Type.ToString(),
						    Id = ftdiDeviceList[i].ID.ToString(),
						    Description = ftdiDeviceList[i].Description,
						    SerialNumber = ftdiDeviceList[i].SerialNumber
					    };
					    devices.Add(d);
				    }
			    }
			    else
			    {
				    Logging.Error($"Error getting FTDI device list {ftStatus.ToString()}");
			    }
		    }

		    //Close device
		    ftStatus = tempFtdiDevice.Close();
			Logging.Info("Closing FTDI device interogation.");

		    return devices;
	    }
	}

}