using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public class DataGridViewWithBackground : DataGridView
	{


		protected override void PaintBackground(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle gridBounds)
		{
			if (this.Parent.BackgroundImage == null)
			{
				base.PaintBackground(graphics, clipBounds, gridBounds);
			}
			else
			{
				Rectangle rectSource = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
				Rectangle rectDest = new Rectangle(0, 0, rectSource.Width, rectSource.Height);

				Bitmap b = new Bitmap(Parent.ClientRectangle.Width, Parent.ClientRectangle.Height);
				Graphics.FromImage(b).DrawImage(this.Parent.BackgroundImage, Parent.ClientRectangle);


				graphics.DrawImage(b, rectDest, rectSource, GraphicsUnit.Pixel);
				SetCellsTransparent();
			}
		}
		public void SetCellsTransparent()
		{
			this.EnableHeadersVisualStyles = false;
			this.ColumnHeadersDefaultCellStyle.BackColor = Color.Transparent;
			this.RowHeadersDefaultCellStyle.BackColor = Color.Transparent;


			foreach (DataGridViewColumn col in this.Columns)
			{
				col.DefaultCellStyle.BackColor = Color.Transparent;
				col.DefaultCellStyle.SelectionBackColor = Color.Transparent;
			}
		}
	}
}
