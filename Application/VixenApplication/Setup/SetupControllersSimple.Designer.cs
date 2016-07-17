namespace VixenApplication.Setup
{
	partial class SetupControllersSimple
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
			if (disposing && (components != null)) {
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
			this.components = new System.ComponentModel.Container();
			this.groupBoxSelectedController = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelOutputCount = new System.Windows.Forms.Label();
			this.buttonStopController = new System.Windows.Forms.Button();
			this.buttonStartController = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonNumberChannelsController = new System.Windows.Forms.Button();
			this.labelControllerType = new System.Windows.Forms.Label();
			this.buttonConfigureController = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonRenameController = new System.Windows.Forms.Button();
			this.buttonDeleteController = new System.Windows.Forms.Button();
			this.buttonAddController = new System.Windows.Forms.Button();
			this.comboBoxNewControllerType = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonSelectSourceElements = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.controllerTree = new Common.Controls.ControllerTree();
			this.groupBoxSelectedController.SuspendLayout();
			this.panel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxSelectedController
			// 
			this.groupBoxSelectedController.AutoSize = true;
			this.groupBoxSelectedController.Controls.Add(this.panel1);
			this.groupBoxSelectedController.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxSelectedController.Location = new System.Drawing.Point(3, 486);
			this.groupBoxSelectedController.Name = "groupBoxSelectedController";
			this.groupBoxSelectedController.Size = new System.Drawing.Size(249, 112);
			this.groupBoxSelectedController.TabIndex = 32;
			this.groupBoxSelectedController.TabStop = false;
			this.groupBoxSelectedController.Text = "Selected Controller";
			this.groupBoxSelectedController.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.labelOutputCount);
			this.panel1.Controls.Add(this.buttonStopController);
			this.panel1.Controls.Add(this.buttonStartController);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.buttonNumberChannelsController);
			this.panel1.Controls.Add(this.labelControllerType);
			this.panel1.Controls.Add(this.buttonConfigureController);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.buttonRenameController);
			this.panel1.Controls.Add(this.buttonDeleteController);
			this.panel1.Location = new System.Drawing.Point(8, 16);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(235, 77);
			this.panel1.TabIndex = 46;
			// 
			// labelOutputCount
			// 
			this.labelOutputCount.AutoSize = true;
			this.labelOutputCount.Location = new System.Drawing.Point(57, 25);
			this.labelOutputCount.Name = "labelOutputCount";
			this.labelOutputCount.Size = new System.Drawing.Size(70, 13);
			this.labelOutputCount.TabIndex = 3;
			this.labelOutputCount.Text = "Output Count";
			// 
			// buttonStopController
			// 
			this.buttonStopController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonStopController.BackColor = System.Drawing.Color.Transparent;
			this.buttonStopController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.buttonStopController.FlatAppearance.BorderSize = 0;
			this.buttonStopController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonStopController.Location = new System.Drawing.Point(154, 49);
			this.buttonStopController.Name = "buttonStopController";
			this.buttonStopController.Size = new System.Drawing.Size(24, 24);
			this.buttonStopController.TabIndex = 45;
			this.buttonStopController.Text = "S";
			this.toolTip1.SetToolTip(this.buttonStopController, "Stop");
			this.buttonStopController.UseVisualStyleBackColor = false;
			this.buttonStopController.Click += new System.EventHandler(this.buttonStopController_Click);
			// 
			// buttonStartController
			// 
			this.buttonStartController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonStartController.BackColor = System.Drawing.Color.Transparent;
			this.buttonStartController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.buttonStartController.FlatAppearance.BorderSize = 0;
			this.buttonStartController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonStartController.Location = new System.Drawing.Point(124, 49);
			this.buttonStartController.Name = "buttonStartController";
			this.buttonStartController.Size = new System.Drawing.Size(24, 24);
			this.buttonStartController.TabIndex = 44;
			this.buttonStartController.Text = "P";
			this.toolTip1.SetToolTip(this.buttonStartController, "Start");
			this.buttonStartController.UseVisualStyleBackColor = false;
			this.buttonStartController.Click += new System.EventHandler(this.buttonStartController_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(1, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Outputs:";
			// 
			// buttonNumberChannelsController
			// 
			this.buttonNumberChannelsController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNumberChannelsController.BackColor = System.Drawing.Color.Transparent;
			this.buttonNumberChannelsController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.buttonNumberChannelsController.FlatAppearance.BorderSize = 0;
			this.buttonNumberChannelsController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonNumberChannelsController.Location = new System.Drawing.Point(34, 49);
			this.buttonNumberChannelsController.Name = "buttonNumberChannelsController";
			this.buttonNumberChannelsController.Size = new System.Drawing.Size(24, 24);
			this.buttonNumberChannelsController.TabIndex = 43;
			this.buttonNumberChannelsController.Text = "N";
			this.toolTip1.SetToolTip(this.buttonNumberChannelsController, "Channel Count");
			this.buttonNumberChannelsController.UseVisualStyleBackColor = false;
			this.buttonNumberChannelsController.Click += new System.EventHandler(this.buttonNumberChannelsController_Click);
			// 
			// labelControllerType
			// 
			this.labelControllerType.AutoSize = true;
			this.labelControllerType.Location = new System.Drawing.Point(57, 6);
			this.labelControllerType.Name = "labelControllerType";
			this.labelControllerType.Size = new System.Drawing.Size(78, 13);
			this.labelControllerType.TabIndex = 1;
			this.labelControllerType.Text = "Controller Type";
			// 
			// buttonConfigureController
			// 
			this.buttonConfigureController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonConfigureController.BackColor = System.Drawing.Color.Transparent;
			this.buttonConfigureController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.buttonConfigureController.FlatAppearance.BorderSize = 0;
			this.buttonConfigureController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonConfigureController.Location = new System.Drawing.Point(4, 49);
			this.buttonConfigureController.Name = "buttonConfigureController";
			this.buttonConfigureController.Size = new System.Drawing.Size(24, 24);
			this.buttonConfigureController.TabIndex = 42;
			this.buttonConfigureController.Text = "C";
			this.toolTip1.SetToolTip(this.buttonConfigureController, "Configure");
			this.buttonConfigureController.UseVisualStyleBackColor = false;
			this.buttonConfigureController.Click += new System.EventHandler(this.buttonConfigureController_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Type:";
			// 
			// buttonRenameController
			// 
			this.buttonRenameController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonRenameController.BackColor = System.Drawing.Color.Transparent;
			this.buttonRenameController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.buttonRenameController.FlatAppearance.BorderSize = 0;
			this.buttonRenameController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonRenameController.Location = new System.Drawing.Point(64, 49);
			this.buttonRenameController.Name = "buttonRenameController";
			this.buttonRenameController.Size = new System.Drawing.Size(24, 24);
			this.buttonRenameController.TabIndex = 41;
			this.buttonRenameController.Text = "R";
			this.toolTip1.SetToolTip(this.buttonRenameController, "Rename");
			this.buttonRenameController.UseVisualStyleBackColor = false;
			this.buttonRenameController.Click += new System.EventHandler(this.buttonRenameController_Click);
			// 
			// buttonDeleteController
			// 
			this.buttonDeleteController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDeleteController.BackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.buttonDeleteController.FlatAppearance.BorderSize = 0;
			this.buttonDeleteController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDeleteController.Location = new System.Drawing.Point(94, 49);
			this.buttonDeleteController.Name = "buttonDeleteController";
			this.buttonDeleteController.Size = new System.Drawing.Size(24, 24);
			this.buttonDeleteController.TabIndex = 40;
			this.buttonDeleteController.Text = "-";
			this.toolTip1.SetToolTip(this.buttonDeleteController, "Delete");
			this.buttonDeleteController.UseVisualStyleBackColor = false;
			this.buttonDeleteController.Click += new System.EventHandler(this.buttonDeleteController_Click);
			// 
			// buttonAddController
			// 
			this.buttonAddController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddController.BackColor = System.Drawing.Color.Transparent;
			this.buttonAddController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.buttonAddController.Enabled = false;
			this.buttonAddController.FlatAppearance.BorderSize = 0;
			this.buttonAddController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddController.Location = new System.Drawing.Point(201, 6);
			this.buttonAddController.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
			this.buttonAddController.Name = "buttonAddController";
			this.buttonAddController.Size = new System.Drawing.Size(24, 24);
			this.buttonAddController.TabIndex = 35;
			this.buttonAddController.Text = "+";
			this.toolTip1.SetToolTip(this.buttonAddController, "Add");
			this.buttonAddController.UseVisualStyleBackColor = false;
			this.buttonAddController.Click += new System.EventHandler(this.buttonAddController_Click);
			// 
			// comboBoxNewControllerType
			// 
			this.comboBoxNewControllerType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxNewControllerType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxNewControllerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNewControllerType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxNewControllerType.FormattingEnabled = true;
			this.comboBoxNewControllerType.Location = new System.Drawing.Point(38, 8);
			this.comboBoxNewControllerType.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
			this.comboBoxNewControllerType.Name = "comboBoxNewControllerType";
			this.comboBoxNewControllerType.Size = new System.Drawing.Size(157, 21);
			this.comboBoxNewControllerType.TabIndex = 34;
			this.comboBoxNewControllerType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 12);
			this.label5.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(29, 13);
			this.label5.TabIndex = 33;
			this.label5.Text = "Add:";
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 200;
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 200;
			this.toolTip1.ReshowDelay = 40;
			// 
			// buttonSelectSourceElements
			// 
			this.buttonSelectSourceElements.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSelectSourceElements.BackColor = System.Drawing.Color.Transparent;
			this.buttonSelectSourceElements.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.buttonSelectSourceElements.FlatAppearance.BorderSize = 0;
			this.buttonSelectSourceElements.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSelectSourceElements.Location = new System.Drawing.Point(3, 3);
			this.buttonSelectSourceElements.Name = "buttonSelectSourceElements";
			this.buttonSelectSourceElements.Size = new System.Drawing.Size(24, 24);
			this.buttonSelectSourceElements.TabIndex = 40;
			this.buttonSelectSourceElements.Text = "S";
			this.toolTip1.SetToolTip(this.buttonSelectSourceElements, "Find elements patched to these outputs");
			this.buttonSelectSourceElements.UseVisualStyleBackColor = false;
			this.buttonSelectSourceElements.Click += new System.EventHandler(this.buttonSelectSourceElements_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.buttonSelectSourceElements);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 449);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(249, 31);
			this.flowLayoutPanel1.TabIndex = 41;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.Controls.Add(this.label5);
			this.flowLayoutPanel2.Controls.Add(this.comboBoxNewControllerType);
			this.flowLayoutPanel2.Controls.Add(this.buttonAddController);
			this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(244, 34);
			this.flowLayoutPanel2.TabIndex = 42;
			this.flowLayoutPanel2.WrapContents = false;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBoxSelectedController, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.controllerTree, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(255, 601);
			this.tableLayoutPanel1.TabIndex = 43;
			// 
			// controllerTree
			// 
			this.controllerTree.AutoScroll = true;
			this.controllerTree.AutoSize = true;
			this.controllerTree.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.controllerTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controllerTree.Location = new System.Drawing.Point(3, 43);
			this.controllerTree.Name = "controllerTree";
			this.controllerTree.Size = new System.Drawing.Size(249, 400);
			this.controllerTree.TabIndex = 36;
			// 
			// SetupControllersSimple
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.DoubleBuffered = true;
			this.Name = "SetupControllersSimple";
			this.Size = new System.Drawing.Size(255, 601);
			this.groupBoxSelectedController.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxSelectedController;
		private System.Windows.Forms.Button buttonAddController;
		private System.Windows.Forms.ComboBox comboBoxNewControllerType;
		private System.Windows.Forms.Label label5;
		private Common.Controls.ControllerTree controllerTree;
		private System.Windows.Forms.Label labelOutputCount;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelControllerType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonSelectSourceElements;
		private System.Windows.Forms.Button buttonNumberChannelsController;
		private System.Windows.Forms.Button buttonConfigureController;
		private System.Windows.Forms.Button buttonRenameController;
		private System.Windows.Forms.Button buttonDeleteController;
		private System.Windows.Forms.Button buttonStopController;
		private System.Windows.Forms.Button buttonStartController;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
