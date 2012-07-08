using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// Zusammenfassung für SideBarRendererBase.
	/// </summary>
	public abstract class SideBarRendererBase:Component
	{
		#region drawitemheader
		public void DrawItemHeader(DrawHeaderArgs e)
		{
			if (e.IsEmpty()) return;
			this.OnDrawItemHeader(e);
		}
		protected abstract void OnDrawItemHeader(DrawHeaderArgs e);
		#endregion
		#region itembackcolor
		public abstract Color ItemBackColor{get;}
		#endregion
		#region drawborder
		public void DrawItemBorder(DrawItemArgs e)
		{
			if (e.IsEmpty()) return;
			this.OnDrawItemBorder(e);
		}
		protected abstract void OnDrawItemBorder(DrawItemArgs e);
		#endregion
		#region drawsidebarbackground
		public void DrawSideBarBackground(DrawElementArgs e)
		{
			if (e.IsEmpty()) return;
			this.OnDrawSideBarBackground(e);
		}
		protected abstract void OnDrawSideBarBackground(DrawElementArgs e);
		#endregion
	}
	public struct DrawElementArgs
	{
		public Graphics Graphics;
		public Rectangle Bounds;
		public DrawElementArgs(Graphics graphics, Rectangle bounds)
		{
			this.Graphics=graphics;
			this.Bounds=bounds;
		}
		public DrawElementArgs(Graphics graphics, int x, int y, int width, int height):
			this(graphics,new Rectangle(x,y,width,height)){}
		public bool IsEmpty()
		{
			return this.Graphics==null;
		}
	}
	public struct DrawItemArgs
	{
		public Graphics Graphics;
		public Rectangle ItemBounds;
		public bool Special, Collapsed;
		public int HeaderHeight;
		public DrawItemArgs(Graphics graphics, Rectangle itembounds, int headerheight, bool special, bool collapsed)
		{
			this.Graphics=graphics;
			this.ItemBounds=itembounds;
			this.HeaderHeight=headerheight;
			this.Special=special;
			this.Collapsed=collapsed;
		}
		public bool IsEmpty()
		{
			return this.Graphics==null;
		}
	}
	public struct DrawHeaderArgs
	{
		public Graphics Graphics;
		public Rectangle ItemBounds, ParentRectangle;
		public string Caption;
		public bool Special, Collapsed, HasParent, MouseCapture;
		public Point Location;
		public int HeaderHeight, TitleBarHeight;
		public Bitmap TitleImage;
		public DrawHeaderArgs(Graphics graphics, Rectangle itembounds, int headerheight,
			Rectangle parentrectangle, string caption, bool special, bool collapsed, Point location,
			bool mousecapture, Bitmap titleimage, int titlebarheight)
		{
			this.Graphics=graphics;
			this.ItemBounds=itembounds;
			this.HeaderHeight=headerheight;
			this.ParentRectangle=parentrectangle;
			this.Caption=caption;
			this.Special=special;
			this.Collapsed=collapsed;
			this.HasParent=true;
			this.Location=location;
			this.MouseCapture=mousecapture;
			this.TitleImage=titleimage;
			this.TitleBarHeight=titlebarheight;
		}
		public DrawHeaderArgs(Graphics graphics, Rectangle itembounds, int headerheight,
			string caption, bool special, bool collapsed, Point location, bool mousecapture,
			Bitmap titleimage, int titlebarheight)
		{
			this.Graphics=graphics;
			this.ItemBounds=itembounds;
			this.HeaderHeight=headerheight;
			this.ParentRectangle=Rectangle.Empty;
			this.Caption=caption;
			this.Special=special;
			this.Collapsed=collapsed;
			this.HasParent=false;
			this.Location=location;
			this.MouseCapture=mousecapture;
			this.TitleImage=titleimage;
			this.TitleBarHeight=titlebarheight;
		}
		public bool IsEmpty()
		{
			return this.Graphics==null;
		}
	}
}
