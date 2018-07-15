using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace Common.Controls
{
	public partial class BaseForm : Form
	{
		public BaseForm()
		{
			//need to work on the broken layouts so we can just use the system font size.
			//Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 9);
			Font = ThemeUpdateControls.StandardFont;
		}
	}
}
