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
		/// <param name="GraphicRef"></param>
		/// <param name="GraphicString"></param>
		/// <param name="clipRectangle"></param>
		/// <param name="fontResourceName">Either Font Family Name or the full path to the Font Resourse (RESX)</param>
		/// <param name="MaxFontSize"></param>
		/// <param name="MinFontSize"></param>
		/// <param name="SmallestOnFail"></param>
		/// <returns></returns>
		static public Font GetAdjustedFont(System.Drawing.Graphics GraphicRef, string GraphicString, System.Drawing.Rectangle clipRectangle, string fontResourceName, int MaxFontSize=100, int MinFontSize=10, bool SmallestOnFail=true)
		{
			bool privateFont = fontResourceName.ToLower().EndsWith("ttf");
			Font OriginalFont=null;
			try {


				if (privateFont)
					OriginalFont = GetFontFromResx(fontResourceName);
				else
					OriginalFont= new Font(fontResourceName, 100);

				// We utilize MeasureString which we get via a control instance           
				for (int AdjustedSize = MaxFontSize; AdjustedSize >= MinFontSize; AdjustedSize--) {
					Font TestFont;

					if (privateFont)
						TestFont = new Font(private_fonts.Families[0], AdjustedSize);
					else
						TestFont= new Font(fontResourceName, AdjustedSize);

					// Test the string with the new size
					SizeF AdjustedSizeNew = GraphicRef.MeasureString(GraphicString, TestFont);

					if (clipRectangle.Width-4 > Convert.ToInt32(AdjustedSizeNew.Width) && clipRectangle.Height-4> Convert.ToInt32(AdjustedSizeNew.Height)) {
						// Good font, return it
						return TestFont;
					} else
						TestFont.Dispose();
				}

				// If you get here there was no fontsize that worked
				// return MinimumSize or Original?
				if (SmallestOnFail) {
					return new Font(OriginalFont.Name, MinFontSize, OriginalFont.Style);
				} else {
					return OriginalFont;
				}
			} finally {
				if (OriginalFont != null)
					OriginalFont.Dispose();
			}
		}
	}
}
