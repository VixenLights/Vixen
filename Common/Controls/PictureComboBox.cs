using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls
{
	public partial class PictureComboBox : ComboBox
	{
		public PictureComboBox()
		{
			DrawMode = DrawMode.OwnerDrawFixed;
			DropDownStyle = ComboBoxStyle.DropDownList;
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			e.DrawBackground();

			e.DrawFocusRectangle();

			if (e.Index >= 0 && e.Index < Items.Count) {
				PictureComboBoxItem item = (PictureComboBoxItem) Items[e.Index];

				e.Graphics.DrawImage(item.Image, e.Bounds.Left, e.Bounds.Top);

				e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + item.Image.Width,
				                      e.Bounds.Top + 2);
			}

			base.OnDrawItem(e);
		}
	}
}