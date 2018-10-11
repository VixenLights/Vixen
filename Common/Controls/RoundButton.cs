using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Common.Controls
{
	public class RoundButton : Button
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			GraphicsPath grPath = new GraphicsPath();
			grPath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
			Region = new Region(grPath);
			base.OnPaint(e);
		}
	}
}
