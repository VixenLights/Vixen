namespace Dataweb.NShape.WinFormsUI {

	partial class DesignEditorDialog {

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
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.designPresenter = new Dataweb.NShape.WinFormsUI.DesignPresenter();
			this.designController = new Dataweb.NShape.Controllers.DesignController();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.designsComboBox = new System.Windows.Forms.ToolStripComboBox();
			this.activateButton = new System.Windows.Forms.ToolStripButton();
			this.designsSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.newDesignButton = new System.Windows.Forms.ToolStripButton();
			this.deleteDesignButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.createStyleButton = new System.Windows.Forms.ToolStripButton();
			this.deleteStyleButton = new System.Windows.Forms.ToolStripButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.closeButton = new System.Windows.Forms.Button();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.toolStrip2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.designPresenter);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(661, 372);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(661, 397);
			this.toolStripContainer1.TabIndex = 7;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip2);
			// 
			// designPresenter
			// 
			this.designPresenter.DesignController = this.designController;
			this.designPresenter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.designPresenter.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(220)))));
			this.designPresenter.FocusedItemColor = System.Drawing.Color.Beige;
			this.designPresenter.HighlightedItemColor = System.Drawing.SystemColors.ControlLightLight;
			this.designPresenter.HighlightItems = true;
			this.designPresenter.InactiveItemBackgroundColor = System.Drawing.SystemColors.Control;
			this.designPresenter.InactiveItemBorderColor = System.Drawing.SystemColors.Window;
			this.designPresenter.InactiveItemTextColor = System.Drawing.SystemColors.ControlDarkDark;
			this.designPresenter.Location = new System.Drawing.Point(0, 0);
			this.designPresenter.Name = "designPresenter";
			this.designPresenter.SelectedDesign = null;
			this.designPresenter.SelectedItemColor = System.Drawing.SystemColors.Window;
			this.designPresenter.SelectedItemTextColor = System.Drawing.SystemColors.ControlText;
			this.designPresenter.SelectedStyleCategory = Dataweb.NShape.StyleCategory.CapStyle;
			this.designPresenter.Size = new System.Drawing.Size(661, 372);
			this.designPresenter.TabIndex = 0;
			// 
			// designController
			// 
			this.designController.Project = null;
			// 
			// toolStrip2
			// 
			this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.designsComboBox,
            this.activateButton,
            this.designsSeparator,
            this.newDesignButton,
            this.deleteDesignButton,
            this.toolStripSeparator1,
            this.createStyleButton,
            this.deleteStyleButton});
			this.toolStrip2.Location = new System.Drawing.Point(3, 0);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new System.Drawing.Size(544, 25);
			this.toolStrip2.TabIndex = 1;
			// 
			// designsComboBox
			// 
			this.designsComboBox.Name = "designsComboBox";
			this.designsComboBox.Size = new System.Drawing.Size(121, 25);
			this.designsComboBox.SelectedIndexChanged += new System.EventHandler(this.designsComboBox_SelectedIndexChanged);
			// 
			// activateButton
			// 
			this.activateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.activateButton.Image = global::Dataweb.NShape.WinFormsUI.Properties.Resources.ActivateDesign;
			this.activateButton.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.activateButton.Name = "activateButton";
			this.activateButton.Size = new System.Drawing.Size(23, 22);
			this.activateButton.Text = "Activate";
			this.activateButton.Click += new System.EventHandler(this.activateButton_Click);
			// 
			// designsSeparator
			// 
			this.designsSeparator.Name = "designsSeparator";
			this.designsSeparator.Size = new System.Drawing.Size(6, 25);
			// 
			// newDesignButton
			// 
			this.newDesignButton.Image = global::Dataweb.NShape.WinFormsUI.Properties.Resources.NewItemsBtn;
			this.newDesignButton.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.newDesignButton.Name = "newDesignButton";
			this.newDesignButton.Size = new System.Drawing.Size(99, 22);
			this.newDesignButton.Text = "New Design...";
			this.newDesignButton.Click += new System.EventHandler(this.newDesignButton_Click);
			// 
			// deleteDesignButton
			// 
			this.deleteDesignButton.Image = global::Dataweb.NShape.WinFormsUI.Properties.Resources.DeleteBtn;
			this.deleteDesignButton.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.deleteDesignButton.Name = "deleteDesignButton";
			this.deleteDesignButton.Size = new System.Drawing.Size(99, 22);
			this.deleteDesignButton.Text = "Delete Design";
			this.deleteDesignButton.Click += new System.EventHandler(this.deleteDesignButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// createStyleButton
			// 
			this.createStyleButton.Image = global::Dataweb.NShape.WinFormsUI.Properties.Resources.NewBtn;
			this.createStyleButton.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.createStyleButton.Name = "createStyleButton";
			this.createStyleButton.Size = new System.Drawing.Size(88, 22);
			this.createStyleButton.Text = "New Style...";
			this.createStyleButton.Click += new System.EventHandler(this.newStyleButton_Click);
			// 
			// deleteStyleButton
			// 
			this.deleteStyleButton.Image = global::Dataweb.NShape.WinFormsUI.Properties.Resources.DeleteBtn;
			this.deleteStyleButton.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.deleteStyleButton.Name = "deleteStyleButton";
			this.deleteStyleButton.Size = new System.Drawing.Size(88, 22);
			this.deleteStyleButton.Text = "Delete Style";
			this.deleteStyleButton.Click += new System.EventHandler(this.deleteStyleButton_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Transparent;
			this.panel1.Controls.Add(this.closeButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 397);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(661, 35);
			this.panel1.TabIndex = 7;
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(574, 6);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(75, 23);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// DesignEditorDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(661, 432);
			this.Controls.Add(this.toolStripContainer1);
			this.Controls.Add(this.panel1);
			this.DoubleBuffered = true;
			this.Name = "DesignEditorDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Design Editor";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DesignEditorDialog_FormClosed);
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button closeButton;
		private Dataweb.NShape.WinFormsUI.DesignPresenter designPresenter;
		private Dataweb.NShape.Controllers.DesignController designController;
		private System.Windows.Forms.ToolStrip toolStrip2;
		private System.Windows.Forms.ToolStripComboBox designsComboBox;
		private System.Windows.Forms.ToolStripButton activateButton;
		private System.Windows.Forms.ToolStripSeparator designsSeparator;
		private System.Windows.Forms.ToolStripButton newDesignButton;
		private System.Windows.Forms.ToolStripButton deleteDesignButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton createStyleButton;
		private System.Windows.Forms.ToolStripButton deleteStyleButton;
	}

}