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
			this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutButtons = new System.Windows.Forms.FlowLayoutPanel();
			this.btnEnableAll = new System.Windows.Forms.Button();
			this.btnDisableAll = new System.Windows.Forms.Button();
			this.networkListView = new Common.Controls.ListViewEx();
			this.controllerColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.channelsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.startColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.endColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tableLayoutMain.SuspendLayout();
			this.flowLayoutButtons.SuspendLayout();
			this.SuspendLayout();
			//
			// lblConfigureOutput
			//
			this.lblConfigureOutput.AutoSize = true;
			this.lblConfigureOutput.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblConfigureOutput.Name = "lblConfigureOutput";
			this.lblConfigureOutput.Size = new System.Drawing.Size(269, 15);
			this.lblConfigureOutput.TabIndex = 1;
			this.lblConfigureOutput.Text = "Step 3:  Configure the required outputs and order.";
			//
			// tableLayoutMain
			//
			this.tableLayoutMain.ColumnCount = 1;
			this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutMain.Controls.Add(this.flowLayoutButtons, 0, 0);
			this.tableLayoutMain.Controls.Add(this.networkListView, 0, 1);
			this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutMain.Name = "tableLayoutMain";
			this.tableLayoutMain.RowCount = 2;
			this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
			this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutMain.TabIndex = 3;
			//
			// flowLayoutButtons
			//
			this.flowLayoutButtons.AutoSize = true;
			this.flowLayoutButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutButtons.Controls.Add(this.btnEnableAll);
			this.flowLayoutButtons.Controls.Add(this.btnDisableAll);
			this.flowLayoutButtons.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
			this.flowLayoutButtons.Name = "flowLayoutButtons";
			this.flowLayoutButtons.TabIndex = 0;
			this.flowLayoutButtons.WrapContents = false;
			//
			// btnEnableAll
			//
			this.btnEnableAll.AutoSize = true;
			this.btnEnableAll.Enabled = false;
			this.btnEnableAll.Margin = new System.Windows.Forms.Padding(3);
			this.btnEnableAll.Name = "btnEnableAll";
			this.btnEnableAll.TabIndex = 0;
			this.btnEnableAll.Text = "Enable All";
			this.btnEnableAll.UseVisualStyleBackColor = true;
			//
			// btnDisableAll
			//
			this.btnDisableAll.AutoSize = true;
			this.btnDisableAll.Enabled = false;
			this.btnDisableAll.Margin = new System.Windows.Forms.Padding(3);
			this.btnDisableAll.Name = "btnDisableAll";
			this.btnDisableAll.TabIndex = 1;
			this.btnDisableAll.Text = "Disable All";
			this.btnDisableAll.UseVisualStyleBackColor = true;
			//
			// networkListView
			//
			this.networkListView.AllowDrop = true;
			this.networkListView.AllowRowReorder = true;
			this.networkListView.CheckBoxes = true;
			this.networkListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.controllerColumn,
            this.channelsColumn,
            this.startColumn,
            this.endColumn});
			this.networkListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.networkListView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.networkListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			listViewItem1.StateImageIndex = 0;
			this.networkListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
			this.networkListView.MultiSelect = true;
			this.networkListView.Name = "networkListView";
			this.networkListView.OwnerDraw = true;
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
			this.Controls.Add(this.tableLayoutMain);
			this.Controls.Add(this.lblConfigureOutput);
			this.Name = "BulkExportControllersStage";
			this.Size = new System.Drawing.Size(573, 355);
			this.flowLayoutButtons.ResumeLayout(false);
			this.flowLayoutButtons.PerformLayout();
			this.tableLayoutMain.ResumeLayout(false);
			this.tableLayoutMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblConfigureOutput;
		private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutButtons;
		private System.Windows.Forms.Button btnEnableAll;
		private System.Windows.Forms.Button btnDisableAll;
		private Common.Controls.ListViewEx networkListView;
		private System.Windows.Forms.ColumnHeader controllerColumn;
		private System.Windows.Forms.ColumnHeader channelsColumn;
		private System.Windows.Forms.ColumnHeader startColumn;
		private System.Windows.Forms.ColumnHeader endColumn;
	}
}
