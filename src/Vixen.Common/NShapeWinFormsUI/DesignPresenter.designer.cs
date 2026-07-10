namespace Dataweb.NShape.WinFormsUI {

	partial class DesignPresenter {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.styleListBox = new Dataweb.NShape.WinFormsUI.StyleListBox();
			this.styleCollectionListBox = new Dataweb.NShape.WinFormsUI.VerticalTabControl();
			this.propertyPresenter = new Dataweb.NShape.WinFormsUI.PropertyPresenter();
			this.propertyController = new Dataweb.NShape.Controllers.PropertyController();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Right;
			this.propertyGrid.Location = new System.Drawing.Point(348, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(254, 432);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.SystemColors.Control;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(345, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 432);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// styleListBox
			// 
			this.styleListBox.BackColor = System.Drawing.SystemColors.Window;
			this.styleListBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.styleListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.styleListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.styleListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.styleListBox.FocusBorderColor = System.Drawing.Color.Transparent;
			this.styleListBox.FormattingEnabled = true;
			this.styleListBox.HighlightItems = true;
			this.styleListBox.IntegralHeight = false;
			this.styleListBox.ItemBackgroundColor = System.Drawing.SystemColors.Window;
			this.styleListBox.ItemBorderColor = System.Drawing.Color.Transparent;
			this.styleListBox.ItemFocusedColor = System.Drawing.Color.Transparent;
			this.styleListBox.ItemHighlightedColor = System.Drawing.SystemColors.HighlightText;
			this.styleListBox.ItemSelectedColor = System.Drawing.SystemColors.MenuHighlight;
			this.styleListBox.Location = new System.Drawing.Point(102, 0);
			this.styleListBox.Name = "styleListBox";
			this.styleListBox.SelectedStyle = null;
			this.styleListBox.Size = new System.Drawing.Size(243, 432);
			this.styleListBox.StyleCategory = Dataweb.NShape.StyleCategory.CapStyle;
			this.styleListBox.TabIndex = 6;
			this.styleListBox.TextColor = System.Drawing.SystemColors.WindowText;
			this.styleListBox.SelectedIndexChanged += new System.EventHandler(this.styleListBox_SelectedIndexChanged);
			// 
			// styleCollectionListBox
			// 
			this.styleCollectionListBox.BackColor = System.Drawing.SystemColors.Control;
			this.styleCollectionListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.styleCollectionListBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.styleCollectionListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.styleCollectionListBox.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(220)))));
			this.styleCollectionListBox.FocusedItemColor = System.Drawing.Color.Beige;
			this.styleCollectionListBox.FormattingEnabled = true;
			this.styleCollectionListBox.HighlightedItemColor = System.Drawing.SystemColors.ControlLightLight;
			this.styleCollectionListBox.InactiveItemBackgroundColor = System.Drawing.SystemColors.Control;
			this.styleCollectionListBox.InactiveItemBorderColor = System.Drawing.SystemColors.Window;
			this.styleCollectionListBox.InactiveItemTextColor = System.Drawing.SystemColors.ControlDarkDark;
			this.styleCollectionListBox.IntegralHeight = false;
			this.styleCollectionListBox.Location = new System.Drawing.Point(0, 0);
			this.styleCollectionListBox.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
			this.styleCollectionListBox.Name = "styleCollectionListBox";
			this.styleCollectionListBox.SelectedItemColor = System.Drawing.SystemColors.Window;
			this.styleCollectionListBox.SelectedItemTextColor = System.Drawing.SystemColors.ControlText;
			this.styleCollectionListBox.Size = new System.Drawing.Size(102, 432);
			this.styleCollectionListBox.TabIndex = 5;
			this.styleCollectionListBox.SelectedIndexChanged += new System.EventHandler(this.styleCollectionListBox_SelectedIndexChanged);
			// 
			// propertyPresenter
			// 
			this.propertyPresenter.PrimaryPropertyGrid = this.propertyGrid;
			this.propertyPresenter.PropertyController = this.propertyController;
			this.propertyPresenter.SecondaryPropertyGrid = null;
			// 
			// propertyController
			// 
			this.propertyController.Project = null;
			// 
			// DesignPresenter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.styleListBox);
			this.Controls.Add(this.styleCollectionListBox);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.propertyGrid);
			this.DoubleBuffered = true;
			this.Name = "DesignPresenter";
			this.Size = new System.Drawing.Size(602, 432);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Splitter splitter1;
		private StyleListBox styleListBox;
		private VerticalTabControl styleCollectionListBox;
		private PropertyPresenter propertyPresenter;
		private Dataweb.NShape.Controllers.PropertyController propertyController;
	}
}