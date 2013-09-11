using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.RDS
{
	public class RDSModule : EffectModuleInstanceBase
	{
		private EffectIntents _elementData = null;
		private RDSData _data;

		public RDSModule()
		{
			_data = new RDSData();

		}

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();

			CommandValue value = new CommandValue(new StringCommand(_data.Title));

			foreach (ElementNode node in TargetNodes) {
				IIntent i = new CommandIntent(value, TimeSpan);
				_elementData.AddIntentForElement(node.Element.Id, i, TimeSpan.Zero);
			}
		}

		[Value]
		public string Title
		{
			get { return _data.Title; }
			set
			{
				_data.Title=value;
				IsDirty=true;
			}
		}

		[Value]
		public string Artist
		{
			get
			{
				return _data.Artist;
			}
			set
			{
				_data.Artist=value;
				IsDirty=true;
			}
		}



		public override void GenerateVisualRepresentation(System.Drawing.Graphics g, System.Drawing.Rectangle clipRectangle)
		{
			string DisplayValue = string.Format("RDS - {0}", Title);

			Font AdjustedFont =  GetAdjustedFont(g, DisplayValue, clipRectangle);
			using (var StringBrush = new SolidBrush(Color.White)) {
				using (var backgroundBrush = new SolidBrush(Color.DarkGray)) {
					g.FillRectangle(backgroundBrush, clipRectangle);
				}
				g.DrawString(DisplayValue, AdjustedFont, StringBrush, 4, 4);
				//base.GenerateVisualRepresentation(g, clipRectangle);
			}

		}
		PrivateFontCollection private_fonts = null;
		[DllImport("gdi32.dll")]
		private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

		Font GetFontFromResx()
		{
			if (private_fonts == null) {

				// specify embedded resource name
				string resource = "RDS.DigitalDream.ttf";
				private_fonts = new PrivateFontCollection();
				// receive resource stream
				Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
				fontStream.Position = 0;
				byte[] buffer = new byte[fontStream.Length];
				for (int totalBytesCopied = 0; totalBytesCopied < fontStream.Length; )
					totalBytesCopied += fontStream.Read(buffer, totalBytesCopied, Convert.ToInt32(fontStream.Length) - totalBytesCopied);


				unsafe {
					fixed (byte* pFontData = buffer) {
						uint dummy = 0;
						private_fonts.AddMemoryFont((IntPtr)pFontData, buffer.Length);
						AddFontMemResourceEx((IntPtr)pFontData, (uint)buffer.Length, IntPtr.Zero, ref dummy);
					}
				}

			}
			return new Font(private_fonts.Families[0], 22);
		}
		Font GetAdjustedFont(Graphics GraphicRef, string GraphicString, System.Drawing.Rectangle clipRectangle, int MaxFontSize=100, int MinFontSize=10, bool SmallestOnFail=true)
		{
			Font OriginalFont = GetFontFromResx();
			// We utilize MeasureString which we get via a control instance           
			for (int AdjustedSize = MaxFontSize; AdjustedSize >= MinFontSize; AdjustedSize--) {
				Font TestFont = new Font(private_fonts.Families[0], AdjustedSize);

				// Test the string with the new size
				SizeF AdjustedSizeNew = GraphicRef.MeasureString(GraphicString, TestFont);

				if (clipRectangle.Width-4 > Convert.ToInt32(AdjustedSizeNew.Width) && clipRectangle.Height-4> Convert.ToInt32(AdjustedSizeNew.Height)) {
					// Good font, return it
					return TestFont;
				}
			}

			// If you get here there was no fontsize that worked
			// return MinimumSize or Original?
			if (SmallestOnFail) {
				return new Font(OriginalFont.Name, MinFontSize, OriginalFont.Style);
			} else {
				return OriginalFont;
			}
		}

		protected override Vixen.Sys.EffectIntents _Render()
		{
			return _elementData;
		}
		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as RDSData; }
		}
	}
}
