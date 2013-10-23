using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace VixenModules.Output.K8055_Controller
{
	public static class K8055DLLWrapper
	{
		private static bool _busy;
		private static int[] _refCounts;


		public static void Close(int device)
		{
			if (device < 0 || device > 3)
			{
				throw new Exception("Invalid device");
			}
			if (_refCounts[device] == 1)
			{
				K8055D.SetCurrentDevice((long)device);
				K8055D.CloseDevice();
				_refCounts[device] = 0;
			}
			else if (_refCounts[device] > 1)
			{
				_refCounts[device]--;
			}
		}

		public static bool Open(int device)
		{
			if (device < 0 || device > 3)
			{
				throw new Exception("Invalid device");
			}
			if (_refCounts[device]++ == 0)
			{
				return (K8055D.OpenDevice((long)device)==device);
			}
			return true;
		}

		public static long Read(int device)
		{
			if (_busy)
			{
				return 0L;
			}
			if (device < 0 || device > 3)
			{
				throw new Exception("Invalid device");
			}
			if (_refCounts[device] == 0)
			{
				throw new Exception("Device is not open");
			}
			K8055D.SetCurrentDevice((long)device);
			return K8055D.ReadAllDigital();
		}

		public static long SearchDevices()
		{
			int num;
			_busy = true;
			for (num = 0; num < _refCounts.Length; num++)
			{
				K8055D.SetCurrentDevice((long)num);
				K8055D.CloseDevice();
			}
			long numdevices = K8055D.SearchDevices();
			for (num = 0; num < _refCounts.Length; num++)
			{
				if (_refCounts[num] > 0)
				{
					K8055D.OpenDevice((long)num);
				}
			}
			_busy = false;
			return numdevices;
		}

		public static void Version()
		{
			K8055D.Version();
		}

		public static void Write(int device, long data)
		{
			if (!_busy)
			{
				if (device < 0 || device > 3)
				{
					throw new Exception("Invalid device");
				}
				if (_refCounts[device] == 0)
				{
					throw new Exception("Device is not open");
				}
				K8055D.SetCurrentDevice((long)device);
				K8055D.WriteAllDigital(data);
			}
		}
	}

	internal static class K8055D
	{
		[DllImport("Common\\K8055D")]
		public static extern void ClearAnalogChannel(long channel);

		[DllImport("Common\\K8055D")]
		public static extern void ClearDigitalChannel(long channel);

		[DllImport("Common\\K8055D")]
		public static extern void CloseDevice();

		[DllImport("Common\\K8055D")]
		public static extern long OpenDevice(long address);

		[DllImport("Common\\K8055D")]
		public static extern void OutputAnalogChannel(long channel, long data);

		[DllImport("Common\\K8055D")]
		public static extern long ReadAllDigital();

		[DllImport("Common\\K8055D")]
		public static extern long SearchDevices();

		[DllImport("Common\\K8055D")]
		public static extern void SetAnalogChannel(long channel);

		[DllImport("Common\\K8055D")]
		public static extern long SetCurrentDevice(long address);

		[DllImport("Common\\K8055D")]
		public static extern void SetDigitalChannel(long channel);

		[DllImport("Common\\K8055D")]
		public static extern void Version();

		[DllImport("Common\\K8055D")]
		public static extern void WriteAllDigital(long data);
	}
}
