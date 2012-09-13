namespace VixenModules.Preview.TestPreview {
	partial class ChannelColorStateControl {
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
			this.labelChannelName = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.lightingValueControl1 = new VixenModules.Preview.TestPreview.LightingValueControl();
			this.colorValueControl1 = new VixenModules.Preview.TestPreview.ColorValueControl();
			this.commandValueControl1 = new VixenModules.Preview.TestPreview.CommandValueControl();
			this.positionValueControl1 = new VixenModules.Preview.TestPreview.PositionValueControl();
			this.panel1.SuspendLayout();
			this.flowLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelChannelName
			// 
			this.labelChannelName.AutoSize = true;
			this.labelChannelName.Location = new System.Drawing.Point(3, 10);
			this.labelChannelName.Name = "labelChannelName";
			this.labelChannelName.Size = new System.Drawing.Size(35, 13);
			this.labelChannelName.TabIndex = 1;
			this.labelChannelName.Text = "label1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.labelChannelName);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(147, 41);
			this.panel1.TabIndex = 2;
			// 
			// flowLayoutPanel
			// 
			this.flowLayoutPanel.Controls.Add(this.lightingValueControl1);
			this.flowLayoutPanel.Controls.Add(this.colorValueControl1);
			this.flowLayoutPanel.Controls.Add(this.commandValueControl1);
			this.flowLayoutPanel.Controls.Add(this.positionValueControl1);
			this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel.Location = new System.Drawing.Point(147, 0);
			this.flowLayoutPanel.Name = "flowLayoutPanel";
			this.flowLayoutPanel.Size = new System.Drawing.Size(687, 41);
			this.flowLayoutPanel.TabIndex = 3;
			// 
			// lightingValueControl1
			// 
			this.lightingValueControl1.BackColor = System.Drawing.Color.DarkRed;
			this.lightingValueControl1.Location = new System.Drawing.Point(3, 3);
			this.lightingValueControl1.Name = "lightingValueControl1";
			this.lightingValueControl1.Size = new System.Drawing.Size(150, 23);
			this.lightingValueControl1.TabIndex = 0;
			// 
			// colorValueControl1
			// 
			this.colorValueControl1.Location = new System.Drawing.Point(159, 3);
			this.colorValueControl1.Name = "colorValueControl1";
			this.colorValueControl1.Size = new System.Drawing.Size(127, 23);
			this.colorValueControl1.TabIndex = 1;
			// 
			// commandValueControl1
			// 
			this.commandValueControl1.Location = new System.Drawing.Point(292, 3);
			this.commandValueControl1.Name = "commandValueControl1";
			this.commandValueControl1.Size = new System.Drawing.Size(186, 20);
			this.commandValueControl1.TabIndex = 2;
			// 
			// positionValueControl1
			// 
			this.positionValueControl1.Location = new System.Drawing.Point(484, 3);
			this.positionValueControl1.Name = "positionValueControl1";
			this.positionValueControl1.Size = new System.Drawing.Size(199, 24);
			this.positionValueControl1.TabIndex = 3;
			// 
			// ChannelColorStateControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel);
			this.Controls.Add(this.panel1);
			this.Name = "ChannelColorStateControl";
			this.Size = new System.Drawing.Size(834, 41);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.flowLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelChannelName;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
		private LightingValueControl lightingValueControl1;
		private ColorValueControl colorValueControl1;
		private CommandValueControl commandValueControl1;
		private PositionValueControl positionValueControl1;

	}
}
