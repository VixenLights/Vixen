namespace VixenApplication.Controls {
	partial class ChannelsToControllerRuleEditor {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listBoxChannels = new System.Windows.Forms.ListBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonSelectNoChannels = new System.Windows.Forms.Button();
			this.buttonSelectAllChannels = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.nudOutputsPerChannels = new System.Windows.Forms.NumericUpDown();
			this.comboBoxStartingOutput = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxController = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonPreview = new System.Windows.Forms.Button();
			this.listViewPreview = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudOutputsPerChannels)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.listBoxChannels);
			this.groupBox1.Controls.Add(this.panel1);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 407);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Channels";
			// 
			// listBoxChannels
			// 
			this.listBoxChannels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxChannels.FormattingEnabled = true;
			this.listBoxChannels.Location = new System.Drawing.Point(3, 16);
			this.listBoxChannels.Name = "listBoxChannels";
			this.listBoxChannels.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxChannels.Size = new System.Drawing.Size(194, 355);
			this.listBoxChannels.TabIndex = 2;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonSelectNoChannels);
			this.panel1.Controls.Add(this.buttonSelectAllChannels);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(3, 371);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(194, 33);
			this.panel1.TabIndex = 1;
			// 
			// buttonSelectNoChannels
			// 
			this.buttonSelectNoChannels.Location = new System.Drawing.Point(84, 3);
			this.buttonSelectNoChannels.Name = "buttonSelectNoChannels";
			this.buttonSelectNoChannels.Size = new System.Drawing.Size(75, 23);
			this.buttonSelectNoChannels.TabIndex = 1;
			this.buttonSelectNoChannels.Text = "Select None";
			this.buttonSelectNoChannels.UseVisualStyleBackColor = true;
			this.buttonSelectNoChannels.Click += new System.EventHandler(this.buttonSelectNoChannels_Click);
			// 
			// buttonSelectAllChannels
			// 
			this.buttonSelectAllChannels.Location = new System.Drawing.Point(3, 3);
			this.buttonSelectAllChannels.Name = "buttonSelectAllChannels";
			this.buttonSelectAllChannels.Size = new System.Drawing.Size(75, 23);
			this.buttonSelectAllChannels.TabIndex = 0;
			this.buttonSelectAllChannels.Text = "Select All";
			this.buttonSelectAllChannels.UseVisualStyleBackColor = true;
			this.buttonSelectAllChannels.Click += new System.EventHandler(this.buttonSelectAllChannels_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.nudOutputsPerChannels);
			this.groupBox2.Controls.Add(this.comboBoxStartingOutput);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.comboBoxController);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(225, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(249, 259);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Target Outputs";
			// 
			// nudOutputsPerChannels
			// 
			this.nudOutputsPerChannels.Enabled = false;
			this.nudOutputsPerChannels.Location = new System.Drawing.Point(83, 219);
			this.nudOutputsPerChannels.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudOutputsPerChannels.Name = "nudOutputsPerChannels";
			this.nudOutputsPerChannels.Size = new System.Drawing.Size(83, 20);
			this.nudOutputsPerChannels.TabIndex = 4;
			this.nudOutputsPerChannels.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// comboBoxStartingOutput
			// 
			this.comboBoxStartingOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxStartingOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxStartingOutput.Enabled = false;
			this.comboBoxStartingOutput.FormattingEnabled = true;
			this.comboBoxStartingOutput.Location = new System.Drawing.Point(13, 144);
			this.comboBoxStartingOutput.Name = "comboBoxStartingOutput";
			this.comboBoxStartingOutput.Size = new System.Drawing.Size(206, 21);
			this.comboBoxStartingOutput.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(7, 177);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(233, 39);
			this.label3.TabIndex = 2;
			this.label3.Text = "How many outputs is each channel going to be patched to?";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(10, 101);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(233, 39);
			this.label2.TabIndex = 2;
			this.label2.Text = "Start patching the selected channels to outputs starting at which output?";
			// 
			// comboBoxController
			// 
			this.comboBoxController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxController.FormattingEnabled = true;
			this.comboBoxController.Location = new System.Drawing.Point(10, 57);
			this.comboBoxController.Name = "comboBoxController";
			this.comboBoxController.Size = new System.Drawing.Size(209, 21);
			this.comboBoxController.TabIndex = 1;
			this.comboBoxController.SelectedIndexChanged += new System.EventHandler(this.comboBoxController_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(7, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(236, 34);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select the controller that the channels will be patched to.";
			// 
			// buttonPreview
			// 
			this.buttonPreview.Enabled = false;
			this.buttonPreview.Location = new System.Drawing.Point(6, 19);
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.Size = new System.Drawing.Size(104, 23);
			this.buttonPreview.TabIndex = 2;
			this.buttonPreview.Text = "Preview Results";
			this.buttonPreview.UseVisualStyleBackColor = true;
			this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
			// 
			// listViewPreview
			// 
			this.listViewPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewPreview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewPreview.Location = new System.Drawing.Point(10, 58);
			this.listViewPreview.Name = "listViewPreview";
			this.listViewPreview.Size = new System.Drawing.Size(249, 339);
			this.listViewPreview.TabIndex = 3;
			this.listViewPreview.UseCompatibleStateImageBehavior = false;
			this.listViewPreview.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Channel";
			this.columnHeader1.Width = 149;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Output";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.buttonPreview);
			this.groupBox3.Controls.Add(this.listViewPreview);
			this.groupBox3.Location = new System.Drawing.Point(480, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(267, 404);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Results";
			// 
			// ChannelsToControllerRuleEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "ChannelsToControllerRuleEditor";
			this.Size = new System.Drawing.Size(750, 413);
			this.Load += new System.EventHandler(this.ChannelsToControllerRuleEditor_Load);
			this.groupBox1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudOutputsPerChannels)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonSelectNoChannels;
		private System.Windows.Forms.Button buttonSelectAllChannels;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox comboBoxStartingOutput;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxController;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudOutputsPerChannels;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonPreview;
		private System.Windows.Forms.ListView listViewPreview;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListBox listBoxChannels;
		private System.Windows.Forms.GroupBox groupBox3;
	}
}
