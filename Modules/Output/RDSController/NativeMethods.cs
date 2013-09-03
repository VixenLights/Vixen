using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace VixenModules.Output.RDSController
{
	internal static class NativeMethods
	{
		[DllImport("inpout32.dll", EntryPoint = "Inp32")]
		public static extern void Input(int adress);
		[DllImport("inpout32.dll", EntryPoint = "Out32")]
		public static extern void Output(int adress, int Len);
		[DllImport("mrdsio.dll")]
		public static extern void ConnectionSetup(int ConMode, int PortNum, bool Bidirectional, bool Slow);
		[DllImport("mrdsio.dll")]
		public static extern string Version();
		[DllImport("mrdsio.dll")]
		public static extern bool Connect();
		[DllImport("mrdsio.dll")]
		public static extern void Disconnect();
		[DllImport("mrdsio.dll")]
		public static extern bool Send(int Len, byte[] Data);
		[return: MarshalAs(UnmanagedType.Struct)]
		[DllImport("mrdsio.dll")]
		static extern TData Receive(int Adr, int Len);
        
	}
}
