// =====================================================================
//  Help.cs - all the help clicks implemented as statics
//  Version 1.0.0.0 - 1 june 2010
// =====================================================================

// =====================================================================
// Copyright (c) 2010 Joshua 1 Systems Inc. All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//    1. Redistributions of source code must retain the above copyright notice, this list of
//       conditions and the following disclaimer.
//    2. Redistributions in binary form must reproduce the above copyright notice, this list
//       of conditions and the following disclaimer in the documentation and/or other materials
//       provided with the distribution.
// THIS SOFTWARE IS PROVIDED BY JOSHUA 1 SYSTEMS INC. "AS IS" AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// The views and conclusions contained in the software and documentation are those of the
// authors and should not be interpreted as representing official policies, either expressed
// or implied, of Joshua 1 Systems Inc.
// =====================================================================

namespace VixenModules.Output.E131
{
    using System;
    using System.Diagnostics;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Text;
    using System.Windows.Forms;
    using VixenModules.Output.E131.J1Sys;

    internal static class Help
    {
        // --------------------------------------------------------------------
        // ShowSysClick() - command to show system info
        // --------------------------------------------------------------------
        public static void ShowSysClick(object sender, EventArgs e)
        {
            var txt = new StringBuilder();
            var p = Process.GetCurrentProcess();

            txt.AppendLine("Process: " + p.ProcessName);
            txt.Append("Threads: " + p.Threads.Count);
            txt.AppendLine("  Handles: " + p.HandleCount);

            if (Environment.OSVersion.Platform
                == PlatformID.Win32NT)
            {
                txt.Append("PMemory: " + p.PrivateMemorySize64);
                txt.AppendLine("  WMemory: " + p.WorkingSet64);
            }

            txt.AppendLine();

            var computerProperties = IPGlobalProperties.GetIPGlobalProperties();

            txt.Append("HostName: " + computerProperties.HostName);
            txt.AppendLine("  DomainName: " + computerProperties.DomainName);

            var nics = NetworkInterface.GetAllNetworkInterfaces();
            if (nics.Length > 0)
            {
                foreach (var nic in nics)
                {
                    if (nic.NetworkInterfaceType.CompareTo(NetworkInterfaceType.Tunnel) != 0)
                    {
                        txt.AppendLine();
                        txt.AppendLine(nic.Description + ":");
                        txt.AppendLine("  ID: " + nic.Id);
                        txt.AppendLine("  Name: " + nic.Name);
                        txt.Append("  Interface Type: " + nic.NetworkInterfaceType);
                        txt.Append("  Physical Address: " + nic.GetPhysicalAddress());
                        txt.AppendLine();
                        txt.Append("  Operational Status: " + nic.OperationalStatus);
                        txt.Append("  Supports Multicast: " + nic.SupportsMulticast);
                        txt.AppendLine();

                        var uniCasts = nic.GetIPProperties().UnicastAddresses;

                        bool prefix = false;

                        foreach (var uniCast in uniCasts)
                        {
                            if (uniCast.Address.AddressFamily.CompareTo(AddressFamily.InterNetwork) == 0)
                            {
                                if (!prefix)
                                {
                                    txt.Append("  IP Address: ");
                                    prefix = true;
                                }

                                txt.Append(" " + uniCast.Address);
                            }
                        }

                        if (prefix)
                        {
                            txt.AppendLine();
                        }
                    }
                }
            }

            txt.AppendLine();
            txt.AppendLine("That's All Folks!!!");
            J1MsgBox.ShowMsg(string.Empty, txt.ToString(), "Show System Info", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}
