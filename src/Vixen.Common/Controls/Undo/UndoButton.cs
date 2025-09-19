namespace Common.Controls
{
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	public class UndoButton : ToolStripSplitButton
	{
		private ToolStripDropDown m_toolDrop = null;
		private ToolStripControlHost m_toolHost = null;
		private UndoDropDownControl m_dropControl = null;

		private const int SetWidth = 200;
		private const int SetHeight = 200;

		public UndoButton()
		{
			// Initialize the custom control
			m_dropControl = new UndoDropDownControl()
			                	{
			                		MinimumSize = new Size(SetWidth, SetHeight) // <- important
			                	};
			m_dropControl.ItemChosen += m_dropControl_ItemChosen;
			m_dropControl.ButtonType = ButtonType;

			// ...hosted by a ToolStripControlHost
			m_toolHost = new ToolStripControlHost(m_dropControl)
			             	{
			             		Size = new Size(SetWidth, SetHeight),
			             		Margin = new Padding(0)
			             	};

			// ... and shown in a ToolStripDropDown.
			m_toolDrop = new ToolStripDropDown()
			             	{
			             		Padding = new Padding(0)
			             	};
			m_toolDrop.Items.Add(m_toolHost);


			this.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.BackgroundImageLayout = ImageLayout.Stretch;

			// There is no OnDropDownOpening to override, so I guess we have to do it this way.
			this.DropDownOpening += UndoButton_DropDownOpening;
		}

		public ListBox.ObjectCollection UndoItems
		{
			get { return m_dropControl.Items; }
		}


		private void UndoButton_DropDownOpening(object sender, EventArgs e)
		{
			m_toolDrop.Show(this.Parent, new Point(this.Bounds.Left, this.Bounds.Bottom));
			m_dropControl.Reset();
			m_dropControl.Focus();
		}

		private void m_dropControl_ItemChosen(object sender, UndoMultipleItemsEventArgs e)
		{
			m_toolDrop.Hide();
		}

		public event EventHandler<UndoMultipleItemsEventArgs> ItemChosen
		{
			add { m_dropControl.ItemChosen += value; }
			remove { m_dropControl.ItemChosen -= value; }
		}

		private UndoButtonType buttonType;
		public UndoButtonType ButtonType {
			get {
				return buttonType;
			}
			set {
				buttonType = value;
				m_dropControl.ButtonType = buttonType;
			}
		}

	}

	public enum UndoButtonType
	{
		UndoButton,
		RedoButton,
	}
}