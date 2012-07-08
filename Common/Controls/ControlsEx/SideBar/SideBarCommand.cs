using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// Zusammenfassung für SideBarCommand.
	/// </summary>
	[DefaultEvent("Click")]
	public class SideBarCommand:Control
	{
		#region variables
		private string _text=null;
		private Image _image=null;
		private Color _hovercolor=Color.SteelBlue;
		private SolidBrush _sld=new SolidBrush(Color.MidnightBlue);
		private Font _underlinefnt;
		private StringFormat _fmt;
		#endregion
		public SideBarCommand()
		{
			base.ForeColor=Color.MidnightBlue;
			this._underlinefnt=new Font(this.Font,FontStyle.Underline | this.Font.Style);
			this._fmt=new StringFormat(StringFormatFlags.NoWrap);
			this._fmt.Trimming=StringTrimming.EllipsisCharacter;
			this._fmt.LineAlignment=StringAlignment.Center;
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.ResizeRedraw,true);
		}
		#region controller
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			height=Math.Max(16,height);
			base.SetBoundsCore (x, y, width, height, specified);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged (e);
			if (this._underlinefnt!=null)
				this._underlinefnt.Dispose();
			this._underlinefnt=new Font(this.Font,FontStyle.Underline | this.Font.Style);
			this.Refresh();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			int offx=0;
			if (this._image!=null)
			{
				offx+=this._image.Width;
				if (this.Enabled)
					e.Graphics.DrawImageUnscaled(this._image,
						0,(this.Height-this._image.Height)/2);
				else
					ControlPaint.DrawImageDisabled(e.Graphics,this._image,
						0,(this.Height-this._image.Height)/2,this.BackColor);
			}
			if (this._text!=null)
			{
				if (this.Enabled)
				{
					e.Graphics.DrawString(this._text,(this._sld.Color==this.ForeColor)?
						this.Font:this._underlinefnt,this._sld,
						new Rectangle(offx,0,this.Width-offx,this.Height),this._fmt);
				}
				else
				{
					e.Graphics.DrawString(this._text,this.Font,Brushes.DimGray,
						new Rectangle(offx,0,this.Width-offx,this.Height),this._fmt);
				}
			}
		}
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter (e);
			this._sld.Color=_hovercolor;
			this.Refresh();
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave (e);
			this._sld.Color=this.ForeColor;
			this.Refresh();
		}
		#endregion
		#region properties
		/// <summary>
		/// the hover color of the link
		/// </summary>
		[Description("the hover color of the link"),
		DefaultValue(typeof(Color),"SteelBlue")]
		public Color HoverColor
		{
			get{return _hovercolor;}
			set
			{
				if (_hovercolor==value) return;
				_hovercolor=value;
			}
		}
		/// <summary>
		/// the normal color of the link
		/// </summary>
		[Description("the normal color of the link"),
		DefaultValue(typeof(Color),"SteelBlue")]
		public override Color ForeColor
		{
			get{return base.ForeColor;}
			set{base.ForeColor = value;}
		}
		/// <summary>
		/// the text of the link
		/// </summary>
		[Description("the text of the link"),
		DefaultValue(null)]
		public new string Text
		{
			get{return this._text;}
			set
			{
				if (value==this._text) return;
				this._text=value;
				this.Refresh();
			}
		}
		/// <summary>
		/// the image displayed left to the text
		/// </summary>
		[Description("the image displayed left to the text"),
		DefaultValue(null)]
		public Image Image
		{
			get{return this._image;}
			set
			{
				if (value==this._image) return;
				this._image=value;
				this.Refresh();
			}
		}
		#endregion
	}
}

