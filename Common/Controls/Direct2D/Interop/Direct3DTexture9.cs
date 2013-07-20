using System;
using System.Runtime.InteropServices;

namespace Common.Controls.Direct2D.Interop
{
    public sealed class Direct3DTexture9 : IDisposable
    {
        private ComInterface.IDirect3DTexture9 comObject;
        private ComInterface.GetSurfaceLevel getSurfaceLevel;

        internal Direct3DTexture9(ComInterface.IDirect3DTexture9 obj)
        {
            this.comObject = obj;
            ComInterface.GetComMethod(this.comObject, 18, out this.getSurfaceLevel);
        }

        ~Direct3DTexture9()
        {
            this.Release();
        }

        public void Dispose()
        {
            this.Release();
            GC.SuppressFinalize(this);
        }

        public Direct3DSurface9 GetSurfaceLevel(uint Level)
        {
            ComInterface.IDirect3DSurface9 surface = null;
            Marshal.ThrowExceptionForHR(this.getSurfaceLevel(this.comObject, Level, out surface));

            return new Direct3DSurface9(surface);
        }

        private void Release()
        {
            if (this.comObject != null)
            {
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
                this.getSurfaceLevel = null;
            }
        }
    }
}
