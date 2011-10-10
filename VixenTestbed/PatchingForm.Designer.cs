namespace VixenTestbed {
	partial class PatchingForm {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonPatch = new System.Windows.Forms.Button();
			this.comboBoxChannel = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxOutput = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxController = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.treeViewPatching = new System.Windows.Forms.TreeView();
			this.buttonDone = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.buttonPatch);
			this.groupBox1.Controls.Add(this.comboBoxChannel);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.comboBoxOutput);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.comboBoxController);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(21, 20);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(507, 122);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// buttonPatch
			// 
			this.buttonPatch.Enabled = false;
			this.buttonPatch.Location = new System.Drawing.Point(330, 84);
			this.buttonPatch.Name = "buttonPatch";
			this.buttonPatch.Size = new System.Drawing.Size(75, 23);
			this.buttonPatch.TabIndex = 13;
			this.buttonPatch.Text = "Patch";
			this.buttonPatch.UseVisualStyleBackColor = true;
			this.buttonPatch.Click += new System.EventHandler(this.buttonPatch_Click);
			// 
			// comboBoxChannel
			// 
			this.comboBoxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChannel.FormattingEnabled = true;
			this.comboBoxChannel.Location = new System.Drawing.Point(76, 19);
			this.comboBoxChannel.Name = "comboBoxChannel";
			this.comboBoxChannel.Size = new System.Drawing.Size(161, 21);
			this.comboBoxChannel.TabIndex = 8;
			this.comboBoxChannel.SelectedIndexChanged += new System.EventHandler(this.comboBoxChannel_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(10, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Channel";
			// 
			// comboBoxOutput
			// 
			this.comboBoxOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOutput.FormattingEnabled = true;
			this.comboBoxOutput.Location = new System.Drawing.Point(330, 47);
			this.comboBoxOutput.Name = "comboBoxOutput";
			this.comboBoxOutput.Size = new System.Drawing.Size(161, 21);
			this.comboBoxOutput.TabIndex = 12;
			this.comboBoxOutput.SelectedIndexChanged += new System.EventHandler(this.comboBoxOutput_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(264, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Output";
			// 
			// comboBoxController
			// 
			this.comboBoxController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxController.FormattingEnabled = true;
			this.comboBoxController.Location = new System.Drawing.Point(330, 20);
			this.comboBoxController.Name = "comboBoxController";
			this.comboBoxController.Size = new System.Drawing.Size(161, 21);
			this.comboBoxController.TabIndex = 10;
			this.comboBoxController.SelectedIndexChanged += new System.EventHandler(this.comboBoxController_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(264, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Controller";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.buttonRemove);
			this.groupBox2.Controls.Add(this.treeViewPatching);
			this.groupBox2.Location = new System.Drawing.Point(21, 148);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(507, 259);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Current Patching";
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Location = new System.Drawing.Point(330, 19);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonRemove.TabIndex = 1;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// treeViewPatching
			// 
			this.treeViewPatching.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.treeViewPatching.Location = new System.Drawing.Point(13, 19);
			this.treeViewPatching.Name = "treeViewPatching";
			this.treeViewPatching.Size = new System.Drawing.Size(290, 223);
			this.treeViewPatching.TabIndex = 0;
			this.treeViewPatching.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewPatching_AfterSelect);
			// 
			// buttonDone
			// 
			this.buttonDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonDone.Location = new System.Drawing.Point(453, 413);
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.Size = new System.Drawing.Size(75, 23);
			this.buttonDone.TabIndex = 2;
			this.buttonDone.Text = "Done";
			this.buttonDone.UseVisualStyleBackColor = true;
			// 
			// PatchingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonDone;
			this.ClientSize = new System.Drawing.Size(550, 448);
			this.Controls.Add(this.buttonDone);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PatchingForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Patching";
			this.Load += new System.EventHandler(this.PatchingForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonDone;
		private System.Windows.Forms.Button buttonPatch;
		private System.Windows.Forms.ComboBox comboBoxChannel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxOutput;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxController;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.TreeView treeViewPatching;

	}
}