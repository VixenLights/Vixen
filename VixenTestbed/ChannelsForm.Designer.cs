namespace VixenTestbed {
	partial class ChannelsForm {
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
			this.buttonAddChannel = new System.Windows.Forms.Button();
			this.buttonRemoveChannel = new System.Windows.Forms.Button();
			this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageChannels = new System.Windows.Forms.TabPage();
			this.listBoxChannels = new System.Windows.Forms.ListBox();
			this.tabPageNodes = new System.Windows.Forms.TabPage();
			this.buttonProperties = new System.Windows.Forms.Button();
			this.treeViewNodes = new System.Windows.Forms.TreeView();
			this.buttonDone = new System.Windows.Forms.Button();
			this.tabControl.SuspendLayout();
			this.tabPageChannels.SuspendLayout();
			this.tabPageNodes.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonAddChannel
			// 
			this.buttonAddChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddChannel.Location = new System.Drawing.Point(329, 24);
			this.buttonAddChannel.Name = "buttonAddChannel";
			this.buttonAddChannel.Size = new System.Drawing.Size(109, 23);
			this.buttonAddChannel.TabIndex = 1;
			this.buttonAddChannel.Text = "Add Channel";
			this.buttonAddChannel.UseVisualStyleBackColor = true;
			this.buttonAddChannel.Click += new System.EventHandler(this.buttonAddChannel_Click);
			// 
			// buttonRemoveChannel
			// 
			this.buttonRemoveChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemoveChannel.Enabled = false;
			this.buttonRemoveChannel.Location = new System.Drawing.Point(329, 60);
			this.buttonRemoveChannel.Name = "buttonRemoveChannel";
			this.buttonRemoveChannel.Size = new System.Drawing.Size(109, 23);
			this.buttonRemoveChannel.TabIndex = 2;
			this.buttonRemoveChannel.Text = "Remove Channel";
			this.buttonRemoveChannel.UseVisualStyleBackColor = true;
			this.buttonRemoveChannel.Click += new System.EventHandler(this.buttonRemoveChannel_Click);
			// 
			// checkBoxEnabled
			// 
			this.checkBoxEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxEnabled.AutoSize = true;
			this.checkBoxEnabled.Location = new System.Drawing.Point(329, 138);
			this.checkBoxEnabled.Name = "checkBoxEnabled";
			this.checkBoxEnabled.Size = new System.Drawing.Size(65, 17);
			this.checkBoxEnabled.TabIndex = 3;
			this.checkBoxEnabled.Text = "Enabled";
			this.checkBoxEnabled.UseVisualStyleBackColor = true;
			this.checkBoxEnabled.Visible = false;
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPageChannels);
			this.tabControl.Controls.Add(this.tabPageNodes);
			this.tabControl.Location = new System.Drawing.Point(12, 12);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(473, 314);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageChannels
			// 
			this.tabPageChannels.Controls.Add(this.listBoxChannels);
			this.tabPageChannels.Controls.Add(this.checkBoxEnabled);
			this.tabPageChannels.Controls.Add(this.buttonAddChannel);
			this.tabPageChannels.Controls.Add(this.buttonRemoveChannel);
			this.tabPageChannels.Location = new System.Drawing.Point(4, 22);
			this.tabPageChannels.Name = "tabPageChannels";
			this.tabPageChannels.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageChannels.Size = new System.Drawing.Size(465, 288);
			this.tabPageChannels.TabIndex = 0;
			this.tabPageChannels.Text = "Channel List";
			this.tabPageChannels.UseVisualStyleBackColor = true;
			// 
			// listBoxChannels
			// 
			this.listBoxChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxChannels.FormattingEnabled = true;
			this.listBoxChannels.Location = new System.Drawing.Point(6, 6);
			this.listBoxChannels.Name = "listBoxChannels";
			this.listBoxChannels.Size = new System.Drawing.Size(291, 277);
			this.listBoxChannels.TabIndex = 0;
			// 
			// tabPageNodes
			// 
			this.tabPageNodes.Controls.Add(this.buttonProperties);
			this.tabPageNodes.Controls.Add(this.treeViewNodes);
			this.tabPageNodes.Location = new System.Drawing.Point(4, 22);
			this.tabPageNodes.Name = "tabPageNodes";
			this.tabPageNodes.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageNodes.Size = new System.Drawing.Size(465, 288);
			this.tabPageNodes.TabIndex = 1;
			this.tabPageNodes.Text = "Group Nodes";
			this.tabPageNodes.UseVisualStyleBackColor = true;
			// 
			// buttonProperties
			// 
			this.buttonProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonProperties.Enabled = false;
			this.buttonProperties.Location = new System.Drawing.Point(329, 24);
			this.buttonProperties.Name = "buttonProperties";
			this.buttonProperties.Size = new System.Drawing.Size(109, 23);
			this.buttonProperties.TabIndex = 1;
			this.buttonProperties.Text = "Properties";
			this.buttonProperties.UseVisualStyleBackColor = true;
			this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
			// 
			// treeViewNodes
			// 
			this.treeViewNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeViewNodes.Location = new System.Drawing.Point(6, 6);
			this.treeViewNodes.Name = "treeViewNodes";
			this.treeViewNodes.Size = new System.Drawing.Size(291, 313);
			this.treeViewNodes.TabIndex = 0;
			this.treeViewNodes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewNodes_AfterSelect);
			this.treeViewNodes.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewNodes_DragDrop);
			this.treeViewNodes.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewNodes_DragOver);
			this.treeViewNodes.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.treeViewNodes_QueryContinueDrag);
			this.treeViewNodes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewNodes_MouseDown);
			this.treeViewNodes.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeViewNodes_MouseMove);
			// 
			// buttonDone
			// 
			this.buttonDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDone.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonDone.Location = new System.Drawing.Point(410, 334);
			this.buttonDone.Name = "buttonDone";
			this.buttonDone.Size = new System.Drawing.Size(75, 23);
			this.buttonDone.TabIndex = 1;
			this.buttonDone.Text = "Done";
			this.buttonDone.UseVisualStyleBackColor = true;
			// 
			// ChannelsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonDone;
			this.ClientSize = new System.Drawing.Size(497, 369);
			this.Controls.Add(this.buttonDone);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChannelsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Channels";
			this.Load += new System.EventHandler(this.ChannelsForm_Load);
			this.tabControl.ResumeLayout(false);
			this.tabPageChannels.ResumeLayout(false);
			this.tabPageChannels.PerformLayout();
			this.tabPageNodes.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonAddChannel;
		private System.Windows.Forms.Button buttonRemoveChannel;
		private System.Windows.Forms.CheckBox checkBoxEnabled;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageChannels;
		private System.Windows.Forms.TabPage tabPageNodes;
		private System.Windows.Forms.TreeView treeViewNodes;
		private System.Windows.Forms.Button buttonProperties;
		private System.Windows.Forms.ListBox listBoxChannels;
		private System.Windows.Forms.Button buttonDone;
	}
}