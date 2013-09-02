using System;
using System.Runtime.InteropServices;

namespace Common.Controls.Direct2D.Interop
{
    public sealed class Direct3DDevice9Ex : IDisposable
    {
        private ComInterface.IDirect3DDevice9Ex comObject;
        private ComInterface.CreateTexture createTexture;

        internal Direct3DDevice9Ex(ComInterface.IDirect3DDevice9Ex obj)
        {
            this.comObject = obj;
            ComInterface.GetComMethod(this.comObject, 23, out this.createTexture);
        }

        ~Direct3DDevice9Ex()
        {
            this.Release();
        }

        public void Dispose()
        {
            this.Release();
            GC.SuppressFinalize(this);
        }

        public Direct3DTexture9 CreateTexture(uint Width, uint Height, uint Levels, int Usage, int Format, int Pool, ref IntPtr pSharedHandle)
        {
            ComInterface.IDirect3DTexture9 obj = null;
            int result = this.createTexture(this.comObject, Width, Height, Levels, Usage, Format, Pool, out obj, ref pSharedHandle);
            Marshal.ThrowExceptionForHR(result);

            return new Direct3DTexture9(obj);
        }

        private void Release()
        {
            if (this.comObject != null)
            {
                Marshal.ReleaseComObject(this.comObject);
                this.comObject = null;
                this.createTexture = null;
            }
        }
    }
}
