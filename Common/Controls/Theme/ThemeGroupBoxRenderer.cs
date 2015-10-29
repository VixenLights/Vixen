using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Common.Controls.Theme
{
	public sealed class ThemeGroupBoxRenderer
	{
		#region Draw GroupBox borders and Text

		public static void GroupBoxesDrawBorder(object sender, PaintEventArgs e, Font f)
		{
			//We have visual styles enabled so we can just use the built in stuff to draw with our own colors.
			GroupBox groupBox = sender as GroupBox;
			if (groupBox == null) return;

			GroupBoxState gbState = groupBox.Enabled ? GroupBoxState.Normal : GroupBoxState.Disabled; 
			TextFormatFlags textFlags = TextFormatFlags.Default | TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.PreserveGraphicsClipping;

			if (groupBox.RightToLeft == RightToLeft.Yes) {
				textFlags |= (TextFormatFlags.Right | TextFormatFlags.RightToLeft); 
			}

			Color textcolor = groupBox.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled; 
			GroupBoxRenderer.DrawGroupBox(e.Graphics, new Rectangle(0, 0, groupBox.Width, groupBox.Height), groupBox.Text, f, textcolor, textFlags, gbState);
			
		}

		#endregion
	}
}
