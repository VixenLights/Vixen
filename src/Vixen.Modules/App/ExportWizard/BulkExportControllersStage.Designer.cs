namespace VixenModules.App.ExportWizard
{
	partial class BulkExportControllersStage
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Test");
			this.lblConfigureOutput = new System.Windows.Forms.Label();
			this.networkListView = new Common.Controls.ListViewEx();
			this.controllerColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.channelsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.startColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.endColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// lblConfigureOutput
			// 
			this.lblConfigureOutput.AutoSize = true;
			this.lblConfigureOutput.Location = new System.Drawing.Point(10, 13);
			this.lblConfigureOutput.Name = "lblConfigureOutput";
			this.lblConfigureOutput.Size = new System.Drawing.Size(269, 15);
			this.lblConfigureOutput.TabIndex = 1;
			this.lblConfigureOutput.Text = "Step 3:  Configure the required outputs and order.";
			// 
			// networkListView
			// 
			this.networkListView.AllowDrop = true;
			this.networkListView.AllowRowReorder = true;
			this.networkListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.networkListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.networkListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.networkListView.CheckBoxes = true;
			this.networkListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.controllerColumn,
            this.channelsColumn,
            this.startColumn,
            this.endColumn});
			this.networkListView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.networkListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewItem1.StateImageIndex = 0;
			this.networkListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
			this.networkListView.Location = new System.Drawing.Point(13, 41);
			this.networkListView.MultiSelect = false;
			this.networkListView.Name = "networkListView";
			this.networkListView.OwnerDraw = true;
			this.networkListView.Size = new System.Drawing.Size(546, 296);
			this.networkListView.TabIndex = 2;
			this.networkListView.UseCompatibleStateImageBehavior = false;
			this.networkListView.View = System.Windows.Forms.View.Details;
			// 
			// controllerColumn
			// 
			this.controllerColumn.Text = "Controller";
			this.controllerColumn.Width = 176;
			// 
			// channelsColumn
			// 
			this.channelsColumn.Text = "Channels";
			this.channelsColumn.Width = 76;
			// 
			// startColumn
			// 
			this.startColumn.Text = "Start";
			// 
			// endColumn
			// 
			this.endColumn.Text = "End";
			this.endColumn.Width = 234;
			// 
			// BulkExportControllersStage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.networkListView);
			this.Controls.Add(this.lblConfigureOutput);
			this.Name = "BulkExportControllersStage";
			this.Size = new System.Drawing.Size(573, 355);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblConfigureOutput;
		private Common.Controls.ListViewEx networkListView;
		private System.Windows.Forms.ColumnHeader controllerColumn;
		private System.Windows.Forms.ColumnHeader channelsColumn;
		private System.Windows.Forms.ColumnHeader startColumn;
		private System.Windows.Forms.ColumnHeader endColumn;
	}
}
