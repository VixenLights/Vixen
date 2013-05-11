using System;
using System.ComponentModel;
using System.Drawing;

namespace Common.Controls.ControlsEx.ListControls
{
	/// <summary>
	/// DisplayElements can be displayed in DisplayEdit
	/// or DisplayList controls
	/// </summary>
	public abstract class DisplayItem : IDisposable
	{
		#region variables
		protected string _text;
		protected object _tag;
		#endregion
		public DisplayItem() : this(null, null) { }
		public DisplayItem(string text) : this(text, null) { }
		public DisplayItem(string text, object tag)
		{
			this._text = text;
			this._tag = tag;
		}
		public virtual void Dispose()
		{
		}
		#region controller
		/// <summary>
		/// renders the element on the specified surface
		/// </summary>
		public void Draw(Graphics gr, Rectangle rct)
		{
			if (gr == null) return;
			this.OnDraw(gr, rct);
		}
		protected abstract void OnDraw(Graphics gr, Rectangle rct);
		public void DrawUnscaled(Graphics gr, int x, int y)
		{
			if (gr == null) return;
			this.OnDrawUnscaled(gr, x, y);
		}
		protected abstract void OnDrawUnscaled(Graphics gr, int x, int y);
		#endregion
		#region properties
		/// <summary>
		/// gets or retrieves the text
		/// </summary>
		public virtual string Text
		{
			get { return this._text; }
			set
			{
				if (value == this._text)
					return;
				this._text = value;
				this.RaiseRefresh();
			}
		}
		/// <summary>
		/// gets or sets the tag
		/// </summary>
		public object Tag
		{
			get { return this._tag; }
			set { this._tag = value; }
		}
		public abstract Size Size { get; }
		public int Width { get { return Size.Width; } }
		public int Height { get { return Size.Height; } }
		#endregion
		#region events
		/// <summary>
		/// Raises the refresh event
		/// </summary>
		public void RaiseRefresh()
		{
			if (this.Refresh != null)
				this.Refresh(this, EventArgs.Empty);
		}
		internal event EventHandler Refresh;
		#endregion
	}
	/// <summary>
	/// displayitem for an image
	/// </summary>
	public class ImageDisplayItem : DisplayItem
	{
		#region variables
		private Image _img;
		#endregion
		#region ctor
		public ImageDisplayItem(Image img, string text, object tag)
			: base(text, tag)
		{
			this._img = img;
		}
		public ImageDisplayItem(Image img, string text) : this(img, text, null) { }
		public ImageDisplayItem(Image img) : this(img, null, null) { }
		public ImageDisplayItem() : this(null, null, null) { }
		public override void Dispose()
		{
			if (_img != null)
				_img.Dispose();
		}
		#endregion
		protected override void OnDraw(Graphics gr, Rectangle rct)
		{
			if (_img != null)
				gr.DrawImage(this._img, rct);
		}
		protected override void OnDrawUnscaled(Graphics gr, int x, int y)
		{
			if (_img != null)
				gr.DrawImage(this._img, x, y, this._img.Width, this._img.Height);
		}

		#region properties
		[DefaultValue(null)]
		public Image Image
		{
			get { return this._img; }
			set
			{
				if (value == this._img)
					return;
				this._img = value;
				this.RaiseRefresh();
			}
		}
		public override Size Size
		{
			get
			{
				if (_img == null)
					return Size.Empty;
				return this._img.Size;
			}
		}
		#endregion
	}
	/// <summary>
	/// displayitem for an icon
	/// </summary>
	public class IconDisplayItem : DisplayItem
	{
		#region variables
		private Icon _icn;
		#endregion
		#region ctor
		public IconDisplayItem(Icon icn, string text, object tag)
			: base(text, tag)
		{
			if (icn == null)
				throw new ArgumentNullException("icn");
			this._icn = icn;
		}
		public IconDisplayItem(Icon icn, string text) : this(icn, text, null) { }
		public IconDisplayItem(Icon icn) : this(icn, null, null) { }
		public IconDisplayItem() : this(null, null, null) { }
		public override void Dispose()
		{
			if (_icn != null)
				_icn.Dispose();
		}
		#endregion
		protected override void OnDraw(Graphics gr, Rectangle rct)
		{
			if (_icn != null)
				gr.DrawIcon(this._icn, rct);
		}
		protected override void OnDrawUnscaled(Graphics gr, int x, int y)
		{
			//here is some cheating necessary to
			//draw the icon with the right size
			if (_icn != null)
				gr.DrawIcon(this._icn, GetTransformedBounds(gr.Transform,
					new Rectangle(x, y, this._icn.Width, this._icn.Height)));
		}
		private Rectangle GetTransformedBounds(System.Drawing.Drawing2D.Matrix transform, Rectangle rct)
		{
			return new Rectangle(rct.X, rct.Y,
				(int)((float)rct.Width * transform.Elements[0]),
				(int)((float)rct.Height * transform.Elements[3]));
		}
		#region properties
		public Icon Icon
		{
			get { return this._icn; }
			set
			{
				if (value == this._icn)
					return;
				this._icn = value;
				this.RaiseRefresh();
			}
		}
		public override Size Size
		{
			get
			{
				if (_icn == null)
					return Size.Empty;
				return this._icn.Size;
			}
		}
		#endregion
	}
}
