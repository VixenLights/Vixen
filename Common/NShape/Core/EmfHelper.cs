/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace Dataweb.NShape.Advanced
{
	// Based on code from http://www.dotnet247.com/247reference/msgs/23/118514.aspx
	/// <summary>
	/// Helper class for copying and deleting EMF files.
	/// </summary>
	internal static class EmfHelper
	{
		[DllImport("user32.dll")]
		private static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport("user32.dll")]
		private static extern bool EmptyClipboard();

		[DllImport("user32.dll")]
		private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

		[DllImport("user32.dll")]
		private static extern bool CloseClipboard();

		[DllImport("gdi32.dll")]
		private static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, string fileName);

		[DllImport("gdi32.dll")]
		private static extern bool DeleteEnhMetaFile(IntPtr hemf);


		/// <summary>
		/// Copies the given <see cref="T:System.Drawing.Imaging.MetaFile" /> to the clipboard.
		/// The given <see cref="T:System.Drawing.Imaging.MetaFile" /> is set to an invalid state inside this function.
		/// </summary>
		public static bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile metafile)
		{
			return PutEnhMetafileOnClipboard(hWnd, metafile, true);
		}


		/// <summary>
		/// Copies the given <see cref="T:System.Drawing.Imaging.MetaFile" /> to the clipboard.
		/// The given <see cref="T:System.Drawing.Imaging.MetaFile" /> is set to an invalid state inside this function.
		/// </summary>
		public static bool PutEnhMetafileOnClipboard(IntPtr hWnd, Metafile metafile, bool clearClipboard)
		{
			if (metafile == null) throw new ArgumentNullException("metafile");
			bool bResult = false;
			IntPtr hEMF, hEMF2;
			hEMF = metafile.GetHenhmetafile(); // invalidates mf
			if (!hEMF.Equals(IntPtr.Zero)) {
				try {
					hEMF2 = CopyEnhMetaFile(hEMF, null);
					if (!hEMF2.Equals(IntPtr.Zero)) {
						if (OpenClipboard(hWnd)) {
							try {
								if (clearClipboard) {
									if (!EmptyClipboard())
										return false;
								}
								IntPtr hRes = SetClipboardData(14 /*CF_ENHMETAFILE*/, hEMF2);
								bResult = hRes.Equals(hEMF2);
							}
							finally {
								CloseClipboard();
							}
						}
					}
				}
				finally {
					DeleteEnhMetaFile(hEMF);
				}
			}
			return bResult;
		}


		/// <summary>
		/// Copies the given <see cref="T:System.Drawing.Imaging.MetaFile" /> to the specified file. If the file does not exist, it will be created.
		/// The given <see cref="T:System.Drawing.Imaging.MetaFile" /> is set to an invalid state inside this function.
		/// </summary>
		public static bool SaveEnhMetaFile(string fileName, Metafile metafile)
		{
			if (metafile == null) throw new ArgumentNullException("metafile");
			bool result = false;
			IntPtr hEmf = metafile.GetHenhmetafile();
			if (hEmf != IntPtr.Zero) {
				IntPtr resHEnh = CopyEnhMetaFile(hEmf, fileName);
				if (resHEnh != IntPtr.Zero) {
					DeleteEnhMetaFile(resHEnh);
					result = true;
				}
				DeleteEnhMetaFile(hEmf);
				metafile.Dispose();
			}
			return result;
		}
	}
}