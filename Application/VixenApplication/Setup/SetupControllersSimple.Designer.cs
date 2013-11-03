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
			this.labelOutputCount = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelControllerType = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonDeleteController = new System.Windows.Forms.Button();
			this.buttonAddController = new System.Windows.Forms.Button();
			this.comboBoxNewControllerType = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonRenameController = new System.Windows.Forms.Button();
			this.buttonConfigureController = new System.Windows.Forms.Button();
			this.buttonNumberChannelsController = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonSelectSourceElements = new System.Windows.Forms.Button();
			this.controllerTree = new Common.Controls.ControllerTree();
			this.groupBoxSelectedController.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxSelectedController
			// 
			this.groupBoxSelectedController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelectedController.Controls.Add(this.labelOutputCount);
			this.groupBoxSelectedController.Controls.Add(this.label2);
			this.groupBoxSelectedController.Controls.Add(this.labelControllerType);
			this.groupBoxSelectedController.Controls.Add(this.label1);
			this.groupBoxSelectedController.Location = new System.Drawing.Point(3, 410);
			this.groupBoxSelectedController.Name = "groupBoxSelectedController";
			this.groupBoxSelectedController.Size = new System.Drawing.Size(244, 87);
			this.groupBoxSelectedController.TabIndex = 32;
			this.groupBoxSelectedController.TabStop = false;
			this.groupBoxSelectedController.Text = "Selected Controller";
			// 
			// labelOutputCount
			// 
			this.labelOutputCount.AutoSize = true;
			this.labelOutputCount.Location = new System.Drawing.Point(65, 49);
			this.labelOutputCount.Name = "labelOutputCount";
			this.labelOutputCount.Size = new System.Drawing.Size(70, 13);
			this.labelOutputCount.TabIndex = 3;
			this.labelOutputCount.Text = "Output Count";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Outputs:";
			// 
			// labelControllerType
			// 
			this.labelControllerType.AutoSize = true;
			this.labelControllerType.Location = new System.Drawing.Point(65, 26);
			this.labelControllerType.Name = "labelControllerType";
			this.labelControllerType.Size = new System.Drawing.Size(78, 13);
			this.labelControllerType.TabIndex = 1;
			this.labelControllerType.Text = "Controller Type";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Type:";
			// 
			// buttonDeleteController
			// 
			this.buttonDeleteController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDeleteController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonDeleteController.Location = new System.Drawing.Point(100, 375);
			this.buttonDeleteController.Name = "buttonDeleteController";
			this.buttonDeleteController.Size = new System.Drawing.Size(24, 24);
			this.buttonDeleteController.TabIndex = 30;
			this.buttonDeleteController.Text = "-";
			this.toolTip1.SetToolTip(this.buttonDeleteController, "Delete");
			this.buttonDeleteController.UseVisualStyleBackColor = true;
			this.buttonDeleteController.Click += new System.EventHandler(this.buttonDeleteController_Click);
			// 
			// buttonAddController
			// 
			this.buttonAddController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonAddController.Enabled = false;
			this.buttonAddController.Location = new System.Drawing.Point(214, 10);
			this.buttonAddController.Name = "buttonAddController";
			this.buttonAddController.Size = new System.Drawing.Size(24, 24);
			this.buttonAddController.TabIndex = 35;
			this.buttonAddController.Text = "+";
			this.toolTip1.SetToolTip(this.buttonAddController, "Add");
			this.buttonAddController.UseVisualStyleBackColor = true;
			this.buttonAddController.Click += new System.EventHandler(this.buttonAddController_Click);
			// 
			// comboBoxNewControllerType
			// 
			this.comboBoxNewControllerType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxNewControllerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNewControllerType.FormattingEnabled = true;
			this.comboBoxNewControllerType.Location = new System.Drawing.Point(47, 12);
			this.comboBoxNewControllerType.Name = "comboBoxNewControllerType";
			this.comboBoxNewControllerType.Size = new System.Drawing.Size(152, 21);
			this.comboBoxNewControllerType.TabIndex = 34;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(29, 13);
			this.label5.TabIndex = 33;
			this.label5.Text = "Add:";
			// 
			// buttonRenameController
			// 
			this.buttonRenameController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonRenameController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonRenameController.Location = new System.Drawing.Point(70, 375);
			this.buttonRenameController.Name = "buttonRenameController";
			this.buttonRenameController.Size = new System.Drawing.Size(24, 24);
			this.buttonRenameController.TabIndex = 37;
			this.buttonRenameController.Text = "R";
			this.toolTip1.SetToolTip(this.buttonRenameController, "Rename");
			this.buttonRenameController.UseVisualStyleBackColor = true;
			this.buttonRenameController.Click += new System.EventHandler(this.buttonRenameController_Click);
			// 
			// buttonConfigureController
			// 
			this.buttonConfigureController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonConfigureController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonConfigureController.Location = new System.Drawing.Point(10, 375);
			this.buttonConfigureController.Name = "buttonConfigureController";
			this.buttonConfigureController.Size = new System.Drawing.Size(24, 24);
			this.buttonConfigureController.TabIndex = 38;
			this.buttonConfigureController.Text = "C";
			this.toolTip1.SetToolTip(this.buttonConfigureController, "Configure");
			this.buttonConfigureController.UseVisualStyleBackColor = true;
			this.buttonConfigureController.Click += new System.EventHandler(this.buttonConfigureController_Click);
			// 
			// buttonNumberChannelsController
			// 
			this.buttonNumberChannelsController.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNumberChannelsController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonNumberChannelsController.Location = new System.Drawing.Point(40, 375);
			this.buttonNumberChannelsController.Name = "buttonNumberChannelsController";
			this.buttonNumberChannelsController.Size = new System.Drawing.Size(24, 24);
			this.buttonNumberChannelsController.TabIndex = 39;
			this.buttonNumberChannelsController.Text = "N";
			this.toolTip1.SetToolTip(this.buttonNumberChannelsController, "Channel Count");
			this.buttonNumberChannelsController.UseVisualStyleBackColor = true;
			this.buttonNumberChannelsController.Click += new System.EventHandler(this.buttonNumberChannelsController_Click);
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
			this.buttonSelectSourceElements.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonSelectSourceElements.Location = new System.Drawing.Point(130, 375);
			this.buttonSelectSourceElements.Name = "buttonSelectSourceElements";
			this.buttonSelectSourceElements.Size = new System.Drawing.Size(24, 24);
			this.buttonSelectSourceElements.TabIndex = 40;
			this.buttonSelectSourceElements.Text = "S";
			this.toolTip1.SetToolTip(this.buttonSelectSourceElements, "Find elements patched to these outputs");
			this.buttonSelectSourceElements.UseVisualStyleBackColor = true;
			this.buttonSelectSourceElements.Click += new System.EventHandler(this.buttonSelectSourceElements_Click);
			// 
			// controllerTree
			// 
			this.controllerTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.controllerTree.Location = new System.Drawing.Point(3, 45);
			this.controllerTree.Name = "controllerTree";
			this.controllerTree.Size = new System.Drawing.Size(244, 317);
			this.controllerTree.TabIndex = 36;
			// 
			// SetupControllersSimple
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonSelectSourceElements);
			this.Controls.Add(this.buttonNumberChannelsController);
			this.Controls.Add(this.buttonConfigureController);
			this.Controls.Add(this.buttonRenameController);
			this.Controls.Add(this.controllerTree);
			this.Controls.Add(this.buttonAddController);
			this.Controls.Add(this.comboBoxNewControllerType);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.groupBoxSelectedController);
			this.Controls.Add(this.buttonDeleteController);
			this.DoubleBuffered = true;
			this.Name = "SetupControllersSimple";
			this.Size = new System.Drawing.Size(250, 500);
			this.groupBoxSelectedController.ResumeLayout(false);
			this.groupBoxSelectedController.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxSelectedController;
		private System.Windows.Forms.Button buttonDeleteController;
		private System.Windows.Forms.Button buttonAddController;
		private System.Windows.Forms.ComboBox comboBoxNewControllerType;
		private System.Windows.Forms.Label label5;
		private Common.Controls.ControllerTree controllerTree;
		private System.Windows.Forms.Button buttonRenameController;
		private System.Windows.Forms.Button buttonConfigureController;
		private System.Windows.Forms.Button buttonNumberChannelsController;
		private System.Windows.Forms.Label labelOutputCount;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelControllerType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonSelectSourceElements;
	}
}
