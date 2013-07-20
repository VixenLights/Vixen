using System;
using System.Runtime.InteropServices;

namespace Common.Controls.Direct2D.Interop
{
    public sealed class Direct3D9Ex : IDisposable
    {
        private ComInterface.IDirect3D9Ex comObject;
        private ComInterface.CreateDeviceEx createDeviceEx;

        private Direct3D9Ex(ComInterface.IDirect3D9Ex obj)
        {
            this.comObject = obj;
            ComInterface.GetComMethod(this.comObject, 20, out this.createDeviceEx);
        }

        ~Direct3D9Ex()
        {
            this.Release();
        }

        public void Dispose()
        {
            this.Release();
            GC.SuppressFinalize(this);
        }

        public static Direct3D9Ex Create(int SDKVersion)
        {
            ComInterface.IDirect3D9Ex obj;
            Marshal.ThrowExceptionForHR(NativeMethods.Direct3DCreate9Ex(SDKVersion, out obj));

            return new Direct3D9Ex(obj);
        }

        public Direct3DDevice9Ex CreateDeviceEx(uint Adapter, int DeviceType, IntPtr hFocusWindow, int BehaviorFlags,
                                                NativeStructs.D3DPRESENT_PARAMETERS pPresentationParameters, NativeStructs.D3DDISPLAYMODEEX pFullscreenDisplayMode)
        {
            ComInterface.IDirect3DDevice9Ex obj = null;
            int result = this.createDeviceEx(this.comObject, Adapter, DeviceType, hFocusWindow, BehaviorFlags, pPresentationParameters, pFullscreenDisplayMode, out obj);
            Marshal.ThrowExceptionForHR(result);

            return new Direct3DDevice9Ex(obj);
        }

        private void Release()
        {
            if (this.comObject != null)
            {
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
                this.createDeviceEx = null;
            }
        }
    }
}
