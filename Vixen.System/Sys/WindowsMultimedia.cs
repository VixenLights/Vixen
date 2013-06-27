using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Vixen.Sys
{
	internal class WindowsMultimedia
	{
		private IntPtr _avHandle;
		private int _index;

		[DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
		private static extern uint WinMM_BeginPeriod(uint uMilliseconds);

		[DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
		private static extern uint WinMM_EndPeriod(uint uMilliseconds);

		[DllImport("Avrt.dll"), SuppressUnmanagedCodeSecurity]
		private static extern IntPtr AvSetMmThreadCharacteristics(string task, ref int index);

		[DllImport("Avrt.dll"), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AvRevertMmThreadCharacteristics(IntPtr handle);

		private const int WINMM_RESOLUTION = 10;

		public void BeginEnhancedResolution()
		{
			if (!_IsNtKernel()) return;

			if (Environment.OSVersion.Version >= WindowsVersion.XP) {
				if (Environment.OSVersion.Version >= WindowsVersion.Vista) {
					//HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks
					_avHandle = AvSetMmThreadCharacteristics("Pro Audio", ref _index);
				}
				else {
					WinMM_BeginPeriod(WINMM_RESOLUTION);
				}
			}
		}

		public void EndEnhancedResolution()
		{
			if (!_IsNtKernel()) return;

			if (Environment.OSVersion.Version >= WindowsVersion.XP) {
				if (Environment.OSVersion.Version >= WindowsVersion.Vista) {
					AvRevertMmThreadCharacteristics(_avHandle);
				}
				else {
					WinMM_EndPeriod(WINMM_RESOLUTION);
				}
			}
		}

		private bool _IsNtKernel()
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT;
		}

		private static class WindowsVersion
		{
			//+------------+------------+-------+-------+---------+
			//| Version    | PlatformId | Major | Minor | Release |
			//+------------+------------+-------+-------+---------+
			//| Win32s     |      0     |   ?   |   ?   |         |
			//| Win95      |      1     |   4   |   0   | 1995 08 |
			//| Win98      |      1     |   4   |  10   | 1998 06 |
			//| WinME      |      1     |   4   |  90   | 2000 09 |
			//| WinNT351   |      2     |   3   |  51   | 1995 04 |
			//| WinNT4     |      2     |   4   |   0   | 1996 07 |
			//| Win2000    |      2     |   5   |   0   | 2000 02 |
			//| WinXP      |      2     |   5   |   1   | 2001 10 |
			//| Win2003    |      2     |   5   |   2   | 2003 04 |
			//| WinXPx64   |      2     |   5   |   2   | 2003 03 |
			//| WinCE      |      3     |   ?   |   ?   |         |
			//| Vista      |      2     |   6   |   0   | 2007 01 |
			//| Win2008    |      2     |   6   |   0   | 2008 02 |
			//| Win2008R2  |      2     |   6   |   1   | 2009 10 |
			//| Win7       |      2     |   6   |   1   | 2009 10 |
			//+------------+------------+-------+-------+---------+
			public static Version XP = new Version(5, 1);
			public static Version Vista = new Version(6, 0);
			//public static Version Win7 = new Version(6, 1);
		}
	}
}