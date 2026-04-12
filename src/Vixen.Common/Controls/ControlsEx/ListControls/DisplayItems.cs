using System.ComponentModel;

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

		public DisplayItem() : this(null, null)
		{
		}

		public DisplayItem(string text) : this(text, null)
		{
		}

		public DisplayItem(string text, object tag)
		{
			_text = text;
			_tag = tag;
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
			OnDraw(gr, rct);
		}

		protected abstract void OnDraw(Graphics gr, Rectangle rct);

		public void DrawUnscaled(Graphics gr, int x, int y)
		{
			if (gr == null) return;
			OnDrawUnscaled(gr, x, y);
		}

		protected abstract void OnDrawUnscaled(Graphics gr, int x, int y);

		#endregion

		#region properties

		/// <summary>
		/// gets or retrieves the text
		/// </summary>
		public virtual string Text
		{
			get { return _text; }
			set
			{
				if (value == _text)
					return;
				_text = value;
				RaiseRefresh();
			}
		}

		/// <summary>
		/// gets or sets the tag
		/// </summary>
		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		public abstract Size Size { get; }

		public int Width
		{
			get { return Size.Width; }
		}

		public int Height
		{
			get { return Size.Height; }
		}

		#endregion

		#region events

		/// <summary>
		/// Raises the refresh event
		/// </summary>
		public void RaiseRefresh()
		{
			if (Refresh != null)
				Refresh(this, EventArgs.Empty);
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
			_img = img;
		}

		public ImageDisplayItem(Image img, string text) : this(img, text, null)
		{
		}

		public ImageDisplayItem(Image img) : this(img, null, null)
		{
		}

		public ImageDisplayItem() : this(null, null, null)
		{
		}

		public override void Dispose()
		{
			if (_img != null)
				_img.Dispose();
		}

		#endregion

		protected override void OnDraw(Graphics gr, Rectangle rct)
		{
			if (_img != null)
				gr.DrawImage(_img, rct);
		}

		protected override void OnDrawUnscaled(Graphics gr, int x, int y)
		{
			if (_img != null)
				gr.DrawImage(_img, x, y, _img.Width, _img.Height);
		}

		#region properties

		[DefaultValue(null)]
		public Image Image
		{
			get { return _img; }
			set
			{
				if (value == _img)
					return;
				_img = value;
				RaiseRefresh();
			}
		}

		public override Size Size
		{
			get
			{
				if (_img == null)
					return Size.Empty;
				return _img.Size;
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
			_icn = icn;
		}

		public IconDisplayItem(Icon icn, string text) : this(icn, text, null)
		{
		}

		public IconDisplayItem(Icon icn) : this(icn, null, null)
		{
		}

		public IconDisplayItem() : this(null, null, null)
		{
		}

		public override void Dispose()
		{
			if (_icn != null)
				_icn.Dispose();
		}

		#endregion

		protected override void OnDraw(Graphics gr, Rectangle rct)
		{
			if (_icn != null)
				gr.DrawIcon(_icn, rct);
		}

		protected override void OnDrawUnscaled(Graphics gr, int x, int y)
		{
			//here is some cheating necessary to
			//draw the icon with the right size
			if (_icn != null)
				gr.DrawIcon(_icn, GetTransformedBounds(gr.Transform,
				                                            new Rectangle(x, y, _icn.Width, _icn.Height)));
		}

		private Rectangle GetTransformedBounds(System.Drawing.Drawing2D.Matrix transform, Rectangle rct)
		{
			return new Rectangle(rct.X, rct.Y,
			                     (int) (rct.Width*transform.Elements[0]),
			                     (int) (rct.Height*transform.Elements[3]));
		}

		#region properties

		public Icon Icon
		{
			get { return _icn; }
			set
			{
				if (value == _icn)
					return;
				_icn = value;
				RaiseRefresh();
			}
		}

		public override Size Size
		{
			get
			{
				if (_icn == null)
					return Size.Empty;
				return _icn.Size;
			}
		}

		#endregion
	}
}