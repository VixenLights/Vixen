using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	internal class DoubleBufferedPanel: Panel
	{
		public DoubleBufferedPanel() {
			this.DoubleBuffered = true;
		}
	}
}
