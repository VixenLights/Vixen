using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// SideBarRendererRoyale renders a SideBar in Aero style
	/// </summary>
	[ToolboxItem(true)]
	public class SideBarRendererAero:SideBarRendererBase
	{
		#region variables
		private Color[][] _colors=new Color[][]
			{
				new Color[]
				{
					Color.FromArgb(30, 64, 109),//normal item horizontal blend
					Color.FromArgb(92, 181, 221),
					Color.FromArgb(13, 76, 93)
				},
				new Color[]
				{
					Color.FromArgb(8, 78, 87),//special item horizontal blend
					Color.FromArgb(75, 197, 182),
					Color.FromArgb(8, 84, 65)
				},
				new Color[]
				{
					Color.FromArgb(120, 255, 255, 255),//item vertical blend
					Color.FromArgb(23, 255, 255, 255),
					Color.FromArgb(74, 0, 0, 0),
					Color.FromArgb(42, 0, 0, 0),
					Color.FromArgb(0, 0, 0, 0)
				},
				new Color[]
				{
					Color.FromArgb(149, 232, 248),//sidebar horizontal blend
					Color.FromArgb(92, 204, 221),
					Color.FromArgb(47, 133, 176),
					Color.FromArgb(50, 139, 178),
					Color.FromArgb(45, 130, 169),
					Color.FromArgb(57, 154, 181),
					Color.FromArgb(52, 142, 166)
				},
				new Color[]
				{
					Color.FromArgb(0,255,255,255),//sidebar vertical blend
					Color.White
				},
				new Color[]
				{
					Color.FromArgb(50,0,0,0),//header outer border
					Color.FromArgb(100,255,255,255),//header inner border
					Color.FromArgb(166,166,166),//item border
					Color.FromArgb(245,245,245)//item background
				}
			};
		private float[][] _positions=new float[][]
			{
				new float[]{0f,0.5f,1f},//horizontal item positions
				new float[]{0f,0.5f,0.5f,0.8f,1f},//vertial item positions
				new float[]{0f,0.21f,0.44f,0.53f,0.63f,0.83f,1f},//horizontal bar positions
				new float[]{0f,1f}//normal positions
			};
		private Font _fnt=new Font("Tahoma",8f,FontStyle.Bold);
		private StringFormat _fmt;
		private LinearGradientBrush _lnbrs=new LinearGradientBrush(
			new Point(0,0),new Point(1,0),Color.White,Color.Black);
		private ColorBlend _blnd=new ColorBlend();
		private Pen _pn=new Pen(Color.Black);
		private SolidBrush _slbrs=new SolidBrush(Color.White);
		#endregion
		public SideBarRendererAero()
		{
			this._fmt=new StringFormat(StringFormatFlags.NoWrap);
			this._fmt.LineAlignment=StringAlignment.Center;
			this._fmt.Trimming=StringTrimming.EllipsisCharacter;
		}

		public override Color ItemBackColor
		{
			get
			{
				return _colors[5][3];
			}
		}

		protected override void OnDrawItemBorder(DrawItemArgs e)
		{
			if (e.Collapsed) return;
			this._pn.Color=this._colors[5][2];
			e.Graphics.DrawLines(this._pn,
				new Point[]{
							   new Point(0,e.HeaderHeight),
							   new Point(0,e.ItemBounds.Bottom-1),
							   new Point(e.ItemBounds.Right-1,e.ItemBounds.Bottom-1),
							   new Point(e.ItemBounds.Right-1,e.HeaderHeight)
						   });
		}
		protected override void OnDrawItemHeader(DrawHeaderArgs e)
		{
			if(e.HeaderHeight<1) return;
			int y=e.HeaderHeight-e.TitleBarHeight;
			#region erase corners
			if (e.HasParent)
			{
				e.ParentRectangle.Offset(-e.Location.X,-e.Location.Y);
				this.DrawBarBackground(e.Graphics,
					e.ParentRectangle,
					new Rectangle(0,0,e.ItemBounds.Width,e.HeaderHeight));
			}
			else
			{
				e.Graphics.FillRectangle(SystemBrushes.Control,0,0,e.ItemBounds.Width,e.HeaderHeight);
			}
			#endregion
			#region draw backgrounds
			if (e.Special)
				this._blnd.Colors=this._colors[1];
			else
				this._blnd.Colors=this._colors[0];
			#region draw horizontal
			this._blnd.Positions=this._positions[0];
			this._lnbrs.InterpolationColors=this._blnd;
			this._lnbrs.Transform=new Matrix((float)e.ItemBounds.Width,0f,0f,1f,0f,(float)y);
			e.Graphics.FillRectangle(this._lnbrs,0,y,e.ItemBounds.Width,e.TitleBarHeight);
			#endregion
			#region draw vertical
			this._blnd.Colors=this._colors[2];
			this._blnd.Positions=_positions[1];
			this._lnbrs.InterpolationColors=this._blnd;
			this._lnbrs.Transform=new Matrix(0f,e.TitleBarHeight,1f,0f,0f,(float)y);
			e.Graphics.FillRectangle(this._lnbrs,0,y,e.ItemBounds.Width,e.TitleBarHeight);
			#endregion
			#endregion
			#region draw outline
			this._pn.Color=_colors[5][0];
			e.Graphics.DrawLines(this._pn,new Point[]
					{
						new Point(0,y+e.TitleBarHeight-1),
						new Point(0,y),
						new Point(e.ItemBounds.Right-1,y),
						new Point(e.ItemBounds.Right-1,y+e.TitleBarHeight-1)
					});
			this._pn.Color=_colors[5][1];
			e.Graphics.DrawRectangle(this._pn,1,y+1,e.ItemBounds.Width-3,e.TitleBarHeight-2);
			#endregion
			int stringoffx=16;
			if (e.TitleImage!=null)
			{
				e.Graphics.DrawImageUnscaled(e.TitleImage,1,0);
				stringoffx=Math.Max(16,e.TitleImage.Width);
			}

			#region draw string / button
			e.Graphics.DrawString(e.Caption,this._fnt,Brushes.White,
				new Rectangle(stringoffx,y+1,e.ItemBounds.Width-(stringoffx+26),e.TitleBarHeight),this._fmt);
			if (e.Collapsed)
				DrawArrowDown(e.Graphics,e.ItemBounds.Right-18,y+e.TitleBarHeight/2-1);
			else
				DrawArrowUp(e.Graphics,e.ItemBounds.Right-18,y+e.TitleBarHeight/2-3);
			#endregion
		}
		private void DrawArrowDown(Graphics gr, int x, int y)
		{
			gr.FillPolygon(Brushes.White,new Point[]
					{
						new Point(x+1,y),
						new Point(x+4,y+4),
						new Point(x+8,y)
					});
		}
		private void DrawArrowUp(Graphics gr, int x, int y)
		{
			gr.FillPolygon(Brushes.White,new Point[]
					{
						new Point(x,y+5),
						new Point(x+4,y),
						new Point(x+8,y+5)
					});
		}
		protected override void OnDrawSideBarBackground(DrawElementArgs e)
		{
			this.DrawBarBackground(e.Graphics,e.Bounds,e.Bounds);
		}
		private void DrawBarBackground(Graphics gr, Rectangle bounds, Rectangle rct)
		{
			if(bounds.Height<1 || bounds.Width<1) return;
			#region color-band
			this._blnd.Colors=this._colors[3];
			this._blnd.Positions=this._positions[2];
			this._lnbrs.InterpolationColors=this._blnd;
			this._lnbrs.Transform=new Matrix((float)bounds.Width,0f,0f,1f,
				(float)bounds.X,(float)bounds.Y);
			gr.FillRectangle(this._lnbrs,rct);
			#endregion
			#region white-blend
			this._blnd.Colors=_colors[4];
			this._blnd.Positions=_positions[3];
			this._lnbrs.InterpolationColors=this._blnd;
			this._lnbrs.Transform=new Matrix(0f,(float)bounds.Height,1f,0f,
				(float)bounds.X,(float)bounds.Y);
			gr.FillRectangle(this._lnbrs,rct);
			#endregion
		}
	}
}

