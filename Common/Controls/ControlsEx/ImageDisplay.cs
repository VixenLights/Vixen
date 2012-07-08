using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ControlsEx.ListControls
{
	/// <summary>
	/// ImageDisplay shows a DisplayElement
	/// </summary>
	[ToolboxItem(true),
	Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
	public class ImageDisplay:BorderedScrollableControl
	{
		#region variables
		#region painting
		private HatchBrush hbrs=new HatchBrush(HatchStyle.LargeCheckerBoard,
			Color.White,Color.Silver);
		private Pen lnpn=new Pen(Color.FromArgb(128,0,0,0));
		#endregion
		private bool _displayGrid=false;
		private DisplayMode _mode=DisplayMode.Normal;
		private float _zoom=1f;
		private int ZoomPercent=100;
		private DisplayItem _element;
		private GraphicsPath _grid;
		#endregion
		public ImageDisplay()
		{
			this.SetStyle(ControlStyles.UserPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserMouse, true);
		}
		#region helpers
		/// <summary>
		/// creates a grid according to the loaded image
		/// </summary>
		private void CreateGrid()
		{
			if (_grid!=null){_grid.Dispose(); _grid=null;}
			if (_element==null) return;
			//create graphicspath
			_grid=new GraphicsPath();
			//create columns
			for (int x=0; x<=_element.Width; x++)
			{
				_grid.StartFigure();
				_grid.AddLine(x,0,x,_element.Height);
			}
			//create rows
			for (int y=0; y<=_element.Height; y++)
			{
				_grid.StartFigure();
				_grid.AddLine(0,y,_element.Width,y);
			}
		}
		/// <summary>
		/// adjusts the control to content
		/// </summary>
		private void AdjustScrollbars()
		{
			if (CanSetZoom)
				this.AutoScrollMinSize=GetImageSize();
			else
			{
				this.AutoScrollMinSize=Size.Empty;
				//adapt zoom to image
				if (_element!=null && _element.Width>0 && _element.Height>0)
				{
					_zoom=Math.Max(0.01f,Math.Min(
						(float)(this.Width)/(float)(_element.Width+2),
						(float)(this.Height)/(float)(_element.Height+2)));
					if (_zoom>1f) _zoom=(float)Math.Floor(_zoom);
				}
			}
		}
		/// <summary>
		/// gets the matrix used to paint the displayelement
		/// </summary>
		private Matrix GetTransform()
		{
			if ((_mode & DisplayMode.Center)!=0)
			{
				Size sz=GetImageSize();
				return new Matrix(_zoom,0f,0f,_zoom,
					this.HScroll?//horizontal offset
					(float)this.AutoScrollPosition.X+1f:
					(float)((this.Width-sz.Width)/2),
					this.VScroll?//vertical offset
					(float)this.AutoScrollPosition.Y+1f:
					(float)((this.Height-sz.Height)/2));
			}
			else
			{
				return new Matrix(_zoom,0f,0f,_zoom,
					(float)this.AutoScrollPosition.X,
					(float)this.AutoScrollPosition.Y);
			}
		}
		/// <summary>
		/// returns if zoom can be set
		/// </summary>
		private bool CanSetZoom
		{
			get{return (_mode & DisplayMode.Stretch)==0;}
		}
		/// <summary>
		/// gets the image size after zoom applied
		/// </summary>
		private Size GetImageSize()
		{
			if (_element==null) return Size.Empty;
			return new Size(
				2+(int)Math.Ceiling((float)(_element.Width)*_zoom),
				2+(int)Math.Ceiling((float)(_element.Height)*_zoom));
		}
		#endregion
		#region controller
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_element==null) return;
			//prepare graphics
			e.Graphics.Transform=GetTransform();
			e.Graphics.RenderingOrigin=
				new Point((int)e.Graphics.Transform.OffsetX,
				(int)e.Graphics.Transform.OffsetY);
			if (_zoom>=1f)
				e.Graphics.InterpolationMode=InterpolationMode.NearestNeighbor;
			else
				e.Graphics.InterpolationMode=InterpolationMode.HighQualityBicubic;

			e.Graphics.PixelOffsetMode=PixelOffsetMode.HighQuality;
			e.Graphics.FillRectangle(hbrs,0,0,_element.Width,_element.Height);
			//draw element
			this._element.DrawUnscaled(e.Graphics,0,0);

			if (_grid!=null && _displayGrid && _zoom>=4f)
			{
				lnpn.Width=1f/_zoom;
				e.Graphics.DrawPath(lnpn,_grid);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if (!CanSetZoom)
				this.AdjustScrollbars();
			base.OnSizeChanged (e);
		}
		#endregion
		#region properties
		/// <summary>
		/// specifies the displayelement that is rendered
		/// </summary>
		[DefaultValue(null)]
		public DisplayItem Element
		{
			get{return this._element;}
			set
			{
				if (value==this._element) return;
				this._element=value;
				this.CreateGrid();
				this.AdjustScrollbars();
				this.Refresh();
			}
		}
		/// <summary>
		/// specifies the zoom in percent
		/// </summary>
		[Description("specifies the zoom in percent"),
		DefaultValue(100)]
		public int Zoom
		{
			get{return ZoomPercent;}
			set
			{
				value=Math.Min(40000,Math.Max(1,value));
				if (ZoomPercent==value) return;
				ZoomPercent=value;
				if (this.CanSetZoom)
				{
					_zoom=(float)(value)/100f;
					this.AdjustScrollbars();
					this.Refresh();
				}
			}
		}
		/// <summary>
		/// specifies if a pixel grid is displayed when zoom>400
		/// </summary>
		[Description("specifies if a pixel grid is displayed when zoom>400"),
		DefaultValue(false)]
		public bool DisplayGrid
		{
			get{return _displayGrid;}
			set
			{
				if (_displayGrid==value) return;
				_displayGrid=value;
				this.Refresh();
			}
		}
		/// <summary>
		/// specifies the displaymode
		/// </summary>
		[Description("specifies the displaymode"),
		DefaultValue(typeof(DisplayMode),"Normal")]
		public DisplayMode DisplayMode
		{
			get{return _mode;}
			set
			{
				if (_mode==value) return;
				_mode=value;
				if (CanSetZoom)
					_zoom=(float)(ZoomPercent)/100f;
				this.AdjustScrollbars();
				this.Refresh();
			}
		}
		#endregion
	}
	public enum DisplayMode
	{
		Normal=0, Center=1, Stretch=2,
		CenterStretch=3
	}
}
