using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Vixen.Common
{
	public class Graphics
	{
		static PrivateFontCollection private_fonts = null;
		[DllImport("gdi32.dll")]
		private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
		public static bool DisableEffectsEditorRendering
		{
			get
			{
				var setting=	System.Configuration.ConfigurationManager.AppSettings["DisableEffectsEditorRendering"];
				if (!string.IsNullOrWhiteSpace(setting)) {
					return setting.Equals("true", StringComparison.CurrentCultureIgnoreCase);
				}
				return false;
			}
		}
		static unsafe Font GetFontFromResx(string fontResourseName)
		{
			if (private_fonts == null) {

				// specify embedded resource name

				private_fonts = new PrivateFontCollection();
				// receive resource stream
				Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fontResourseName);
				fontStream.Position = 0;
				byte[] buffer = new byte[fontStream.Length];
				for (int totalBytesCopied = 0; totalBytesCopied < fontStream.Length; )
					totalBytesCopied += fontStream.Read(buffer, totalBytesCopied, Convert.ToInt32(fontStream.Length) - totalBytesCopied);

				fixed (byte* pFontData = buffer) {
					uint dummy = 0;
					private_fonts.AddMemoryFont((IntPtr)pFontData, buffer.Length);
					AddFontMemResourceEx((IntPtr)pFontData, (uint)buffer.Length, IntPtr.Zero, ref dummy);
				}


			}
			return new Font(private_fonts.Families[0], 22);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="graphicRef"></param>
		/// <param name="graphicString"></param>
		/// <param name="clipRectangle"></param>
		/// <param name="fontResourceName">Either Font Family Name or the full path to the Font Resourse (RESX)</param>
		/// <param name="maxFontSize"></param>
		/// <param name="minFontSize"></param>
		/// <returns></returns>
		public static Font GetAdjustedFont(System.Drawing.Graphics graphicRef, string graphicString,
			System.Drawing.Rectangle clipRectangle, string fontResourceName, int maxFontSize = 100, int minFontSize = 10)
		{
			bool privateFont = fontResourceName.ToLower().EndsWith("ttf");
			Font  originalFont = privateFont ? new Font(GetFontFromResx(fontResourceName).Name, 100) : new Font(fontResourceName, 100);
			SizeF adjustedSizeNew = graphicRef.MeasureString(graphicString, originalFont);

			double minRatio = Math.Min(clipRectangle.Height / adjustedSizeNew.Height, (clipRectangle.Width + 15) / adjustedSizeNew.Width);
			float newFontSize = (float)(originalFont.Size * minRatio) - 2;

			if (newFontSize < minFontSize) newFontSize = minFontSize;
			else if (newFontSize > maxFontSize) newFontSize = maxFontSize;

			return new Font(originalFont.Name, newFontSize);
		}
	}
}
