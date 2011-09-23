using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CommonElements.ControlsEx.ListControls
{
	/// <summary>
	/// list with fixed height items and custom rendering
	/// </summary>
	[Designer(typeof(ActionListDesigner)),
	DefaultEvent("ItemClicked")]
	public class ActionList : VDisplayList
	{
		#region types
		private class ActionCollection : DisplayItemCollection
		{
			public ActionCollection(DisplayList owner)
				: base(owner) { }
			protected override void OnValidate(DisplayItem value)
			{
				base.OnValidate(value as Action);
			}
		}
		#endregion
		#region variables
		private int _itemheight = 72;
		#endregion
		public ActionList()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor,
				true);
			base.AutoSelect = false;
			this.ImageAlignment = HorizontalAlignment.Left;
			this.TextAlignment = ContentAlignment.BottomLeft;
		}
		#region controller
		//only accept action items
		protected override DisplayItemCollection CreateItemCollection()
		{
			return new ActionCollection(this);
		}
		//rounded path
		private static GraphicsPath CreateRoundedRect(int radius, Rectangle rct)
		{
			radius = Math.Max(1, Math.Min(Math.Min(rct.Width, rct.Height), radius * 2));

			GraphicsPath pth = new GraphicsPath();
			pth.AddArc(rct.X, rct.Y, radius, radius, 180f, 90f);
			pth.AddArc(rct.Right - radius, rct.Y, radius, radius, 270f, 90f);
			pth.AddArc(rct.Right - radius, rct.Bottom - radius, radius, radius, 0f, 90f);
			pth.AddArc(rct.X, rct.Bottom - radius, radius, radius, 90f, 90f);
			pth.CloseFigure();
			return pth;
		}
		//render using new style
		protected override void DrawSelectionFrame(PaintEventArgs e, Rectangle rct, ButtonState state)
		{
			if (state == ButtonState.Checked ||
				state == ButtonState.Pushed)
			{
				e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				rct.Width--; rct.Height--;
				using (GraphicsPath pth = CreateRoundedRect(2, rct))
				{
					using (LinearGradientBrush brs = new LinearGradientBrush(
						new Point(0, rct.Bottom), new Point(0, rct.Y),
						Color.Black, Color.White))
					{
						ColorBlend colorblnd = new ColorBlend();
						colorblnd.Positions = new float[] {
							0F, 0.5F, 0.50F, 1F};
						colorblnd.Colors = new Color[] {
							Color.FromArgb(255, 236, 181),
							Color.FromArgb(255, 236, 181),
							Color.FromArgb(255, 243, 207),
							Color.FromArgb(255, 252, 242)};
						//
						brs.InterpolationColors = colorblnd;
						e.Graphics.FillPath(brs, pth);
					}
					using (Pen pn = new Pen(Color.FromArgb(229, 195, 101)))
						e.Graphics.DrawPath(pn, pth);
				}
			}
		}
		//ise using itemheight
		protected override Size GetTotalSize(Size clientsize, int count)
		{
			_cacheindex = int.MinValue;
			_fieldsize = new Size(clientsize.Width - 2, _itemheight);
			return new Size(0, count * (_fieldsize.Height + 1) + 1);
		}
		//raise clicked on action
		protected override void OnItemClicked(int index)
		{
			base.OnItemClicked(index);
			Action act;
			if (index != -1 && (act = Items[index] as Action) != null)
				act.RaiseClicked();
		}
		protected override void OnHoverChanged(int index)
		{
			base.OnHoverChanged(index);
			if (index == -1)
				base.Cursor = Cursors.Default;
			else
				base.Cursor = Cursors.Hand;
		}
		#endregion
		#region properties
		/// <summary>
		/// gets or sets the height of the items
		/// </summary>
		[DefaultValue(72)]
		public int ItemHeight
		{
			get { return _itemheight; }
			set
			{
				if (value == _itemheight)
					return;
				_itemheight = value;
				Reload();
			}
		}
		#endregion
		#region designer overrides
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Cursor Cursor
		{
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		/// <summary>
		/// gets the collection of display elements
		/// </summary>
		[Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(typeof(ActionCollectionEditor),
			typeof(System.Drawing.Design.UITypeEditor))]
		public new DisplayItemCollection Items
		{
			get { return base.Items; }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool AutoSelect
		{
			get { return base.AutoSelect; }
			set { base.AutoSelect = value; }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ContentAlignment TextAlignment
		{
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int Aspect
		{
			get { return 1; }
			set { }
		}
		#endregion
	}
	/// <summary>
	/// action item allowing designer support
	/// </summary>
	[ToolboxItem(false),
	DesignTimeVisible(false)]
	public class Action : ImageDisplayItem, IComponent
	{
		private ISite _site;
		public Action()
			: base() { }
		public Action(Image img, string text, object tag, EventHandler clicked)
			: base(img, text, tag)
		{
			if (clicked != null)
				Clicked += clicked;
		}
		public override void Dispose()
		{
			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
			base.Dispose();
		}
		#region properties
		/// <summary>
		/// image of the element
		/// </summary>
		[DefaultValue(null)]
		public new Image Image
		{
			get { return base.Image; }
			set { base.Image = value; }
		}
		/// <summary>
		/// text of the element
		/// </summary>
		[Localizable(true)]
		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
		#endregion
		//implemented this to make use of designer support
		#region IComponent Member

		public event EventHandler Disposed;

		public ISite Site
		{
			get { return _site; }
			set { _site = value; }
		}

		#endregion
		public void RaiseClicked()
		{
			if (Clicked != null)
				Clicked(this, EventArgs.Empty);
		}
		public event EventHandler Clicked;
	}
	/// <summary>
	/// editor for toolbarcontrols collection
	/// </summary>
	public class ActionCollectionEditor : CollectionEditor
	{
		public ActionCollectionEditor(Type t)
			: base(t) { }
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(Action) };
		}
	}
	/// <summary>
	/// Zusammenfassung für DisplayListDesigner.
	/// </summary>
	public class ActionListDesigner : ControlDesigner
	{
		public override System.Collections.ICollection AssociatedComponents
		{
			get
			{
				return ((ActionList)Component).Items;
			}
		}
	}
}
