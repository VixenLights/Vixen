using System;
using System.Runtime.InteropServices;

namespace Common.Controls.Direct2D.Interop
{
    public static class NativeMethods
    {
        [DllImport("d3d9.dll")]
        public static extern int Direct3DCreate9Ex(int SDKVersion, out ComInterface.IDirect3D9Ex directX);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();
    }
}
