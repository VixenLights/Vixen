using System;
using System.Windows.Interop;
using D3D10 = Microsoft.WindowsAPICodePack.DirectX.Direct3D10;
using DirectX = Microsoft.WindowsAPICodePack.DirectX;

namespace Common.Controls.Direct2D
{
    /// <summary>
    /// A <see cref="D3DImage"/> class that displays Direct3D 10 content.
    /// </summary>
    public class D3D10Image : D3DImage
    {
        private Interop.Direct3DSurface9 surface;

        /// <summary>
        /// Assigns a <see cref="D3D10.Texture2D"/> as the source of the back buffer.
        /// </summary>
        /// <param name="texture">
        /// The <c>Texture2D</c> to assign as the back buffer. Value can be null.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The D3D10Image has not been locked by a call to the
        /// <see cref="D3DImage.Lock" /> or <see cref="D3DImage.TryLock" /> methods.
        /// </exception>
        public void SetBackBuffer(D3D10.Texture2D texture)
        {
            // Free the old surface
            if (this.surface != null)
            {
                this.surface.Dispose();
                this.surface = null;
            }

            if (texture == null)
            {
                this.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            }
            else
            {
                using (var device = CreateDevice(Interop.NativeMethods.GetDesktopWindow()))
                using (var texture9 = GetSharedSurface(device, texture))
                {
                    this.surface = texture9.GetSurfaceLevel(0);
                }

                this.SetBackBuffer(D3DResourceType.IDirect3DSurface9, this.surface.NativeInterface);
            }
        }

        private static Interop.Direct3DTexture9 GetSharedSurface(Interop.Direct3DDevice9Ex device, D3D10.Texture2D texture)
        {
            // First get a shared handle to the D3D10 texture
            using (var surface = texture.QueryInterface<DirectX.Graphics.Resource>())
            {
                IntPtr handle = surface.SharedHandle;

                // Then create a D3D9 texture using the D3D10 shared handle.
                // The D3D10 texture must be in the DXGI_FORMAT_B8G8R8A8_UNORM
                // format (direct 9 version is D3DFMT_A8R8G8B8).
                return device.CreateTexture(
                    texture.Description.Width,
                    texture.Description.Height,
                    1,
                    1,  // D3DUSAGE_RENDERTARGET
                    21, // D3DFMT_A8R8G8B8
                    0,  // D3DPOOL_DEFAULT
                    ref handle);
            }
        }

        private static Interop.Direct3DDevice9Ex CreateDevice(IntPtr handle)
        {
            const int D3D_SDK_VERSION = 32;
            using (var d3d9 = Interop.Direct3D9Ex.Create(D3D_SDK_VERSION))
            {
                var present = new Interop.NativeStructs.D3DPRESENT_PARAMETERS();
                present.Windowed = 1; // TRUE
                present.SwapEffect = 1; // D3DSWAPEFFECT_DISCARD
                present.hDeviceWindow = handle;
                present.PresentationInterval = unchecked((int)0x80000000); // D3DPRESENT_INTERVAL_IMMEDIATE;

                return d3d9.CreateDeviceEx(
                    0, // D3DADAPTER_DEFAULT
                    1, // D3DDEVTYPE_HAL
                    handle,
                    70, // D3DCREATE_HARDWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE
                    present,
                    null);
            }
        }
    }
}
