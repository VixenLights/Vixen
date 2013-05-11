/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI {

	/// <summary>
	/// RichTextBox control used for editing captions of shapes on the diagram.
	/// </summary>
	[ToolboxItem(false)]
	public partial class InPlaceTextBox : RichTextBox {

		/// <summary>
		/// Creates a new instance of Dataweb.NShape.WinFormsUI.InPlaceTextBox.
		/// </summary>
		public InPlaceTextBox(IDiagramPresenter owner, ICaptionedShape shape, int captionIndex, string currentText)
			: this(owner, shape, captionIndex, currentText, null) {
		}


		/// <summary>
		/// Creates a new instance of Dataweb.NShape.WinFormsUI.InPlaceTextBox.
		/// </summary>
		public InPlaceTextBox(IDiagramPresenter owner, ICaptionedShape shape, int captionIndex, string currentText, string newText) {
			Construct(owner, shape, captionIndex, currentText, newText);
			// Set Text
			originalText = currentText;
			if (string.IsNullOrEmpty(newText)) {
				// Preselect the whole text if the user has not started typing yet
				base.Text = currentText;
				SelectAll();
			} else {
				// Set the types text and place the cursor at the end of the text
				base.Text = newText;
				SelectionStart = Text.Length;
			}
		}


		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		[Browsable(true)]
		public new string ProductVersion {
			get { return base.ProductVersion; }
		}


		///// <override></override>
		//public new Rectangle Bounds {
		//   get { return drawBounds; }
		//   private set {
		//      drawBounds = value;
				
		//      // RichTextbox ignores the Padding property, 
		//      // so we shrink the whole control to maintain the correct padding
		//      value.Inflate(-paragraphStyle.Padding.Horizontal, 0); //-paragraphStyle.Padding.Vertical);
		//      value.Offset(paragraphStyle.Padding.Left, 0); //paragraphStyle.Padding.Top);

		//      SuspendLayout();
		//      this.Size = value.Size;
		//      this.Location = value.Location;
		//      ResumeLayout();
		//   }
		//}


		///// <override></override>
		//public new int Width {
		//   get { return drawBounds.Width; }
		//   private set { base.Width = value - paragraphStyle.Padding.Horizontal; }
		//}


		///// <override></override>
		//public new int Height {
		//   get { return drawBounds.Height; }
		//   private set { base.Height = value - paragraphStyle.Padding.Vertical; }
		//}


		/// <summary>
		/// Specifies the text's alignment
		/// </summary>
		public ContentAlignment TextAlignment {
			get { return ConvertToContentAlignment(SelectionAlignment); }
			set {
				contentAlignment = value;
				this.SuspendLayout();
				SelectAll();
				SelectionAlignment = ConvertToHorizontalAlignment(value);
				DeselectAll();
				SelectionStart = Text.Length;
				this.ResumeLayout();
			}
		}


		/// <summary>
		/// Specifies the initial, unmodified text.
		/// </summary>
		public string OriginalText {
			get { return originalText; }
			set {
				originalText = value;
				if (Text == string.Empty)
					Text = value;
			}
		}
		
		
		/// <summary>
		/// Specifies the text to be edited.
		/// </summary>
		public override string Text {
			get { return base.Text; }
			set {
				base.Text = value;
				if (originalText == string.Empty)
					originalText = value;
				InvalidateEx();
			}
		}

		#endregion


		#region [Protected] Methods

		/// <summary>
		/// Extended version of the Invalidate method, invalidates the control itself and its parent control.
		/// </summary>
		protected void InvalidateEx() {
			if (Parent != null) {
				Rectangle rect = Bounds;
				rect.Inflate(SystemInformation.Border3DSize.Width, SystemInformation.Border3DSize.Height);
				Parent.Invalidate(rect, true);
			} else Invalidate();
		}


		/// <override></override>
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
				return cp;
			}
		}


		/// <override></override>
		protected override Padding DefaultPadding {
			get { return defaultPadding; }
		}


		/// <override></override>
		protected override void OnMove(EventArgs e) {
			InvalidateEx();
			base.OnMove(e);
			InvalidateEx();
		}


		/// <override></override>
		protected override void OnResize(EventArgs eventargs) {
			InvalidateEx();
			base.OnResize(eventargs);
			InvalidateEx();
		}


		/// <override></override>
		protected override void OnHScroll(EventArgs e) {
			base.OnHScroll(e);
			InvalidateEx();
		}


		/// <override></override>
		protected override void OnVScroll(EventArgs e) {
			base.OnVScroll(e);
			InvalidateEx();
		}


		/// <override></override>
		protected override void NotifyInvalidate(Rectangle invalidatedArea) {
			base.NotifyInvalidate(invalidatedArea);
		}


		/// <override></override>
		protected override void OnInvalidated(InvalidateEventArgs e) {
			base.OnInvalidated(e);
		}


		/// <override></override>
		protected override void OnTextChanged(EventArgs e) {
			owner.SuspendUpdate();
			base.OnTextChanged(e);

			shape.SetCaptionText(captionIndex, Text);
			DoUpdateBounds();

			InvalidateEx();
			owner.ResumeUpdate();
		}


		/// <override></override>
		protected override void OnSelectionChanged(EventArgs e) {
			base.OnSelectionChanged(e);
			InvalidateEx();
		}


		/// <override></override>
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			InvalidateEx();
		}

		#endregion


		#region [Private] Methods

		private void Construct(IDiagramPresenter owner, ICaptionedShape shape, int captionIndex, string currentText, string newText) {
			if (owner == null) throw new ArgumentNullException("owner");
			if (shape == null) throw new ArgumentNullException("shape");
			if (captionIndex < 0 || captionIndex >= shape.CaptionCount) throw new ArgumentOutOfRangeException("captionIndex");
			// Set control styles
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
			UpdateStyles();
			
			// Set caption / style specific properties
			this.owner = owner;
			this.shape = shape;
			this.captionIndex = captionIndex;

			// Set general properties
			this.BackColor = Color.Transparent;	// Does not matter - see CreateParams()
			this.AutoScrollOffset = Point.Empty;
			this.ScrollBars = RichTextBoxScrollBars.None;
			this.BorderStyle = BorderStyle.None;
			// Set Styles here because the ParagraphStyle is needed for resizing
			characterStyle = shape.GetCaptionCharacterStyle(captionIndex);
			paragraphStyle = shape.GetCaptionParagraphStyle(captionIndex);


			// Set base' members
			SuspendLayout();
			try {
				this.WordWrap = paragraphStyle.WordWrap;
				this.Font = ToolCache.GetFont(characterStyle);
				this.ZoomFactor = owner.ZoomLevel / 100f;

				// Get line height
				Size textSize = TextRenderer.MeasureText(((IDisplayService)owner).InfoGraphics, "Iq", Font);
				owner.DiagramToControl(textSize, out textSize);
				lineHeight = textSize.Height;

				DoUpdateBounds();

				SelectAll();
				SelectionAlignment = ConvertToHorizontalAlignment(paragraphStyle.Alignment);
				DeselectAll();
			} finally {
				ResumeLayout();
			}
			OnPaddingChanged(EventArgs.Empty);
		}


		private void DoUpdateBounds() {
			Debug.Assert(shape != null);
			Debug.Assert(paragraphStyle != null);
			Debug.Assert(captionIndex >= 0);

			// Get rotated caption bounds from the shape
			Point tl = Point.Empty, tr = Point.Empty, bl = Point.Empty, br = Point.Empty;
			shape.GetCaptionBounds(captionIndex, out tl, out tr, out br, out bl);
			// Calculate unrotated caption bounds
			float angle;
			Point center = Geometry.VectorLinearInterpolation(tl, br, 0.5f);
			angle = Geometry.RadiansToDegrees(Geometry.Angle(tl.X, tl.Y, tr.X, tr.Y));
			tl = Geometry.RotatePoint(center, -angle, tl);
			br = Geometry.RotatePoint(center, -angle, br);
			
			// Calculate unrotated layout area for the text
			Rectangle layoutArea  = Rectangle.Empty;
			layoutArea.Location = tl;
			layoutArea.Width = br.X - tl.X;
			layoutArea.Height = br.Y - tl.Y;
			// Measure the current size of the text
			Size textSize = TextMeasurer.MeasureText(Text, Font, layoutArea.Size, paragraphStyle);

			// Transform layout area and text size to control coordinates
			owner.DiagramToControl(layoutArea, out layoutArea);
			owner.DiagramToControl(textSize, out textSize);
			
			// Calculate new bounds for the text editor based on the shape's caption layout rectangle
			Rectangle newBounds = Rectangle.Empty;
			newBounds.X = layoutArea.X;
			newBounds.Width = layoutArea.Width;
			newBounds.Height = Math.Max(textSize.Height, lineHeight);
			// Move text editor depending on the vertical text alignment
			switch (paragraphStyle.Alignment) {
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomRight:
					newBounds.Y = layoutArea.Bottom - newBounds.Height;
					break;
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleRight:
					newBounds.Y = layoutArea.Y + (int)Math.Round((layoutArea.Height - newBounds.Height) / 2f);
					break;
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopRight:
					newBounds.Y = layoutArea.Y;
					break;
				default:
					newBounds.Y = layoutArea.Y;
					Debug.Fail(string.Format("Unhandled {0} '{1}'.", paragraphStyle.Alignment.GetType().Name, paragraphStyle.Alignment));
					break;
			}
			// Correct textbox size by padding
			newBounds.X += paragraphStyle.Padding.Left;
			newBounds.Y += paragraphStyle.Padding.Top;
			newBounds.Width -= paragraphStyle.Padding.Horizontal;
			newBounds.Height -= paragraphStyle.Padding.Vertical;
			newBounds.Inflate(1, 1);
			
			// Update control bounds 
			// (No need to Invalidate here, this is done by OnResize())
			SuspendLayout();
			Bounds = newBounds;
			defaultPadding = new Padding(paragraphStyle.Padding.Left, paragraphStyle.Padding.Top, paragraphStyle.Padding.Right, paragraphStyle.Padding.Bottom);
			ResumeLayout();
		}


		private HorizontalAlignment ConvertToHorizontalAlignment(ContentAlignment contentAlignment) {
			switch (contentAlignment) {
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					return HorizontalAlignment.Center;
				case ContentAlignment.BottomLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
					return HorizontalAlignment.Left;
				case ContentAlignment.BottomRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
					return HorizontalAlignment.Right;
				default: throw new NShapeUnsupportedValueException(typeof(ContentAlignment), contentAlignment);
			}
		}


		private ContentAlignment ConvertToContentAlignment(HorizontalAlignment horizontalAlignment) {
			switch (horizontalAlignment) {
				case HorizontalAlignment.Center:
					switch(contentAlignment) {
						case ContentAlignment.BottomCenter:
							return ContentAlignment.BottomCenter;
						case ContentAlignment.MiddleCenter:
							return ContentAlignment.MiddleCenter;
						case ContentAlignment.TopCenter:
							return ContentAlignment.TopCenter;
						default: throw new NShapeUnsupportedValueException(typeof(ContentAlignment), contentAlignment);
					}
				case HorizontalAlignment.Left:
					switch(contentAlignment) {
						case ContentAlignment.BottomLeft:
							return ContentAlignment.BottomLeft;
						case ContentAlignment.MiddleLeft:
							return ContentAlignment.MiddleLeft;
						case ContentAlignment.TopLeft:
							return ContentAlignment.TopLeft;
						default: throw new NShapeUnsupportedValueException(typeof(ContentAlignment), contentAlignment);
					}
				case HorizontalAlignment.Right:
					switch(contentAlignment) {
						case ContentAlignment.BottomRight:
							return ContentAlignment.BottomRight;
						case ContentAlignment.MiddleRight:
							return ContentAlignment.MiddleRight;
						case ContentAlignment.TopRight:
							return ContentAlignment.TopRight;
						default: throw new NShapeUnsupportedValueException(typeof(ContentAlignment), contentAlignment);
					}
				default: throw new NShapeUnsupportedValueException(typeof(HorizontalAlignment), horizontalAlignment);
			}
		}

		#endregion


		#region Fields
		private IDiagramPresenter owner;
		private ICaptionedShape shape;
		private int captionIndex;
		
		private string originalText = string.Empty;
		private IParagraphStyle paragraphStyle;
		private ICharacterStyle characterStyle;

		private int lineHeight;

		private Padding defaultPadding = Padding.Empty;
		private ContentAlignment contentAlignment;
		private TextPaddingTypeConverter paddingConverter = new TextPaddingTypeConverter();
		#endregion
	}

}
