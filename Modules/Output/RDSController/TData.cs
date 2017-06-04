using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VixenModules.Output.CommandController
{
	[StructLayout(LayoutKind.Explicit, Size = 256)]
	unsafe struct TData
	{
		[FieldOffset(0)]
		fixed byte m_buffer[8];

		public TData(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (buffer.Length != 8)
				throw new ArgumentException("must be 8 bytes", "buffer");

			fixed (byte* p = m_buffer) {
				IntPtr ptr = new IntPtr(p);
				Marshal.Copy(buffer, 0, ptr, buffer.Length);
			}
		}

		public byte Address
		{
			get
			{
				fixed (byte* p = m_buffer) {
					return p[0];
				}
			}
		}

		public byte BytesRead
		{
			get
			{
				fixed (byte* p = m_buffer) {
					return p[1];
				}
			}
		}

		public bool HasData
		{
			get { return (BytesRead > 0); }
		}

		public string TextData
		{
			get
			{
				if (HasData) {
					byte[] target = new byte[BytesRead];
					fixed (byte* src = m_buffer, dst = target) {
						byte* ps = src + 2;
						byte* pt = dst + 0;
						for (int i = 0; i < target.Length; i++) {
							*pt = *ps;
							pt++;
							ps++;
						}
						return Encoding.Default.GetString(target);
					}
				} else {
					return string.Empty;
				}
			}
		}
	}
}
