using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text;
using System.Globalization;

namespace ControlsEx.ListControls
{
	/// <summary>
	/// Zusammenfassung für HexEditor.
	/// </summary>
	public class HexEditor:BorderedScrollableControl
	{
		#region variables
		private byte[] _data;
		private Font _fnt=new Font("Courier New",9f);
		private StringFormat _fmt;
		private int _linecount=0;
		private SolidBrush sld=new SolidBrush(Color.Black);
		#endregion
		public HexEditor()
		{
			this.SetStyle(ControlStyles.UserPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserMouse, true);
			_fmt=new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
		}
		#region helpers
		private void AdjustScrollBars()
		{
			if (_data==null)
				this.AutoScrollMinSize=Size.Empty;
			else
			{
				this.AutoScrollMinSize=new Size(
					600,
					(int)((float)(_linecount)*_fnt.Height));
			}
		}
		private static bool CharIsPrintable(char c)
		{
			UnicodeCategory category = char.GetUnicodeCategory(c);
			if (((category == UnicodeCategory.Control) && (category != UnicodeCategory.Format)) && ((category != UnicodeCategory.LineSeparator) && (category != UnicodeCategory.ParagraphSeparator)))
			{
				return (category == UnicodeCategory.OtherNotAssigned);
			}
			return true;
		}
		private void DrawAddress(Graphics gr, int line, int pos, float ypos)
		{
			gr.DrawString(pos.ToString("X8")+"h:",
				_fnt,sld,4f,ypos,_fmt);
		}
		private void DrawHex(Graphics gr, int line, int pos, float ypos)
		{
			StringBuilder bld=new StringBuilder(50);
			int stop=Math.Min(_data.Length,pos+16);
			for(int i=pos; i<stop; i++)
			{
				bld.Append(_data[i].ToString("X2"));
				bld.Append(" ");
			}
			if (bld.Length>24)
				bld.Insert(24," ");
			if (bld.Length==49)
				bld.Append(";");
			gr.DrawString(bld.ToString(),
				_fnt,sld,86f,ypos,_fmt);

		}
		private void DrawDump(Graphics gr, int line, int pos, float ypos)
		{
			StringBuilder bld=new StringBuilder(16);
			int stop=Math.Min(_data.Length,pos+16);
			char buf;
			for(int i=pos; i<stop; i++)
			{
				buf=Convert.ToChar(_data[i]);
				if (CharIsPrintable(buf))
					bld.Append(buf);
				else
					bld.Append('.');
			}
			gr.DrawString(bld.ToString(),
				_fnt,sld,467f,ypos,_fmt);
		}
		#endregion
		#region controller
		protected override void OnForeColorChanged(EventArgs e)
		{
			sld.Color=this.ForeColor;
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout (levent);
			AdjustScrollBars();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(this.BackColor);
			if (_data==null) return;

			e.Graphics.Transform=new Matrix(1f,0f,0f,1f,
				(float)this.AutoScrollPosition.X,(float)this.AutoScrollPosition.Y);
			
			int startline=GetLineAtPosition(0),
				endline=GetLineAtPosition(this.Height),
				pos=startline*16;
			float ypos=_fnt.Height*(float)(startline);
			for (int line=startline; line<=endline; line++, pos+=16, ypos+=_fnt.Height)
			{
				DrawAddress(e.Graphics,line,pos,ypos);
				DrawHex(e.Graphics,line,pos,ypos);
				DrawDump(e.Graphics,line,pos,ypos);
			}
		}
		#endregion
		#region public members
		public int GetLineAtPosition(int y)
		{
			y-=this.AutoScrollPosition.Y;

			return Math.Min(_linecount-1,
				Math.Max(0,(int)((float)(y)/_fnt.Height)));
		}
		public void Reload()
		{
			if (_data==null) this._linecount=0;
			else this._linecount=(_data.Length+15)/16;
			this.AdjustScrollBars();
			this.Refresh();
		}
		#endregion
		#region properties
		[DefaultValue(null)]
		public byte[] Data
		{
			get{return _data;}
			set
			{
				if (_data==value) return;
				_data=value;
				this.Reload();
			}
		}
		#endregion
	}
}
