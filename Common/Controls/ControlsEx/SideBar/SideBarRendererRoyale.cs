using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// SideBarRendererRoyale renders a SideBar in EnergyBliss style
	/// </summary>
	[ToolboxItem(true)]
	public class SideBarRendererRoyale:SideBarRendererBase
	{
		#region variables
		private Color[][] _colors=new Color[][]
			{
				new Color[]{
							   Color.FromArgb(81, 116, 168),//special header
							   Color.FromArgb(29, 75, 143),
							   Color.FromArgb(16, 54, 122),
							   Color.FromArgb(12, 51, 123)
						   },
				new Color[]{
							   Color.FromArgb(151, 199, 240),//normal header
							   Color.FromArgb(100, 154, 221),
							   Color.FromArgb(57, 128, 210),
							   Color.FromArgb(49, 95, 183)
						   },
				new Color[]{
							   Color.FromArgb(98,160,232),//sidebar background
							   Color.FromArgb(83,114,190)
						   },
				new Color[]{
							   Color.FromArgb(16,55,124),//special border
							   Color.FromArgb(47,77,171),//normal border
							   Color.FromArgb(217,225,246),//text low
							   Color.FromArgb(255,255,255)//text bright
						   }
			};
		private Font _fnt=new Font("Tahoma",8f,FontStyle.Bold);
		private StringFormat _fmt;
		private LinearGradientBrush _lnbrs=new LinearGradientBrush(
			new Point(0,0),new Point(1,0),Color.White,Color.Black);
		private ColorBlend _blnd=new ColorBlend();
		private Pen _pn=new Pen(Color.Black);
		private SolidBrush _slbrs=new SolidBrush(Color.White);
		private float[][] _positions=new float[][]
			{
				new float[]{0f,0.45f,0.4501f,1f},
				new float[]{0f,1f}
			};
		#endregion
		public SideBarRendererRoyale()
		{
			this._fmt=new StringFormat(StringFormatFlags.NoWrap);
			this._fmt.LineAlignment=StringAlignment.Center;
			this._fmt.Trimming=StringTrimming.EllipsisCharacter;
		}
		public override Color ItemBackColor
		{
			get
			{
				return Color.White;
			}
		}

		protected override void OnDrawItemBorder(DrawItemArgs e)
		{
			if (e.Collapsed) return;
			if (e.Special)
				this._pn.Color=this._colors[3][0];
			else
				this._pn.Color=this._colors[3][1];
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
			{
				this._blnd.Colors=this._colors[0];
				this._pn.Color=this._colors[3][0];
			}
			else
			{
				this._blnd.Colors=this._colors[1];
				this._pn.Color=this._colors[3][1];
			}
			this._blnd.Positions=this._positions[0];
			this._lnbrs.InterpolationColors=this._blnd;
			this._lnbrs.Transform=new Matrix(0f,(float)e.TitleBarHeight,1f,0f,1f,(float)y);
			e.Graphics.FillRectangle(this._lnbrs,1,y+1,e.ItemBounds.Width-1,e.TitleBarHeight-1);
			#endregion
			#region draw outline
			if (e.HasParent)
			{
				e.Graphics.DrawLines(this._pn,new Point[]
					{
						new Point(0,y+e.TitleBarHeight),
						new Point(0,y+1),
						new Point(1,y),
						new Point(e.ItemBounds.Right-2,y),
						new Point(e.ItemBounds.Right-1,y+1),
						new Point(e.ItemBounds.Right-1,y+e.TitleBarHeight)
					});
			}
			else
			{
				e.Graphics.DrawLines(this._pn,new Point[]
					{
						new Point(0,y+e.TitleBarHeight),
						new Point(0,y),
						new Point(e.ItemBounds.Right-1,y),
						new Point(e.ItemBounds.Right-1,y+e.TitleBarHeight)
					});
			}
			#endregion
			int stringoffx=16;
			if (e.TitleImage!=null)
			{
				e.Graphics.DrawImageUnscaled(e.TitleImage,1,0);
				stringoffx=Math.Max(16,e.TitleImage.Width);
			}

			#region draw string / button
			if (e.MouseCapture)
				this._slbrs.Color=this._colors[3][3];
			else
				this._slbrs.Color=this._colors[3][2];

			e.Graphics.DrawString(e.Caption,this._fnt,this._slbrs,
				new Rectangle(stringoffx,y+1,e.ItemBounds.Width-(stringoffx+26),e.TitleBarHeight),this._fmt);
			if (e.Collapsed)
			{
				DrawArrowDown(e.Graphics,e.ItemBounds.Right-18,y+e.TitleBarHeight/2-4);
				DrawArrowDown(e.Graphics,e.ItemBounds.Right-18,y+e.TitleBarHeight/2);
			}
			else
			{
				DrawArrowUp(e.Graphics,e.ItemBounds.Right-18,y+e.TitleBarHeight/2-5);
				DrawArrowUp(e.Graphics,e.ItemBounds.Right-18,y+e.TitleBarHeight/2-1);
			}
			#endregion
		}
		private void DrawArrowDown(Graphics gr, int x, int y)
		{
			gr.FillPolygon(this._slbrs,new Point[]
					{
						new Point(x,y+1),
						new Point(x+3,y+5),
						new Point(x+7,y+1),
						new Point(x+5,y),
						new Point(x+3,y+3),
						new Point(x+1,y)
					});
		}
		private void DrawArrowUp(Graphics gr, int x, int y)
		{
			gr.FillPolygon(this._slbrs,new Point[]
					{
						new Point(x-1,y+5),
						new Point(x+3,y),
						new Point(x+7,y+5),
						new Point(x+5,y+5),
						new Point(x+3,y+2),
						new Point(x+1,y+5)
					});
		}
		protected override void OnDrawSideBarBackground(DrawElementArgs e)
		{
			this.DrawBarBackground(e.Graphics,e.Bounds,e.Bounds);
		}
		private void DrawBarBackground(Graphics gr, Rectangle bounds, Rectangle rct)
		{
			if(bounds.Height<1) return;
			this._blnd.Colors=this._colors[2];
			this._blnd.Positions=this._positions[1];
			this._lnbrs.InterpolationColors=this._blnd;
			this._lnbrs.Transform=new Matrix(0f,(float)bounds.Height,1f,0f,
				(float)bounds.X,(float)bounds.Y);
			gr.FillRectangle(this._lnbrs,rct);
		}

	}
}
