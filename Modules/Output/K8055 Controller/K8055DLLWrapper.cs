using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VixenModules.Output.K8055_Controller
{
	public static class K8055DLLWrapper
	{
		private static bool _busy = false;
		private static int[] _refCounts = new int[4];


		public static void Close(int device)
		{
			if (device < 0 || device > 3)
			{
				throw new Exception("Invalid device");
			}
			if (_refCounts[device] == 1)
			{
				K8055.SetCurrentDevice((long)device);
				K8055.CloseDevice();
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
				return (K8055.OpenDevice((long)device)==device);
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
			K8055.SetCurrentDevice((long)device);
			return K8055.ReadAllDigital();
		}

		public static long SearchDevices()
		{
			int num;
			_busy = true;
			for (num = 0; num < _refCounts.Length; num++)
			{
				K8055.SetCurrentDevice((long)num);
				K8055.CloseDevice();
			}
			long numdevices = K8055.SearchDevices();
			for (num = 0; num < _refCounts.Length; num++)
			{
				if (_refCounts[num] > 0)
				{
					K8055.OpenDevice((long)num);
				}
			}
			_busy = false;
			return numdevices;
		}

		public static void Version()
		{
			K8055.Version();
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
				K8055.SetCurrentDevice((long)device);
				K8055.WriteAllDigital(data);
			}
		}
	}
	public class VERSION
	{
#if WIN64
        public const string dll = "Common\\K8055Dx64";
#else
		public const string dll = "Common\\K8055D";
#endif
	}

	internal static class K8055
	{
		[DllImport(VERSION.dll)]
		public static extern void ClearAnalogChannel(long channel);

		[DllImport(VERSION.dll)]
		public static extern void ClearDigitalChannel(long channel);

		[DllImport(VERSION.dll)]
		public static extern void CloseDevice();

		[DllImport(VERSION.dll)]
		public static extern long OpenDevice(long address);

		[DllImport(VERSION.dll)]
		public static extern void OutputAnalogChannel(long channel, long data);

		[DllImport(VERSION.dll)]
		public static extern long ReadAllDigital();

		[DllImport(VERSION.dll)]
		public static extern long SearchDevices();

		[DllImport(VERSION.dll)]
		public static extern void SetAnalogChannel(long channel);

		[DllImport(VERSION.dll)]
		public static extern long SetCurrentDevice(long address);

		[DllImport(VERSION.dll)]
		public static extern void SetDigitalChannel(long channel);

		[DllImport(VERSION.dll)]
		public static extern void Version();

		[DllImport(VERSION.dll)]
		public static extern void WriteAllDigital(long data);
	}
}
