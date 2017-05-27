using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace VixenModules.Output.LauncherController
{
	internal static class NativeMethods
	{
		#region MRDSIO
		[DllImport("Common\\mrdsio.dll")]
		public static extern void ConnectionSetup(int ConMode, int PortNum, bool Bidirectional, bool Slow);
		[DllImport("Common\\mrdsio.dll")]
		public static extern string Version();
		[DllImport("Common\\mrdsio.dll")]
		public static extern bool Connect();
		[DllImport("Common\\mrdsio.dll")]
		public static extern void Disconnect();
		[DllImport("Common\\mrdsio.dll")]
		public static extern bool Send(int Len, byte[] Data);
		[return: MarshalAs(UnmanagedType.Struct)]
		[DllImport("Common\\mrdsio.dll")]
		static extern TData Receive(int Adr, int Len);
		#endregion
	
	}
}
