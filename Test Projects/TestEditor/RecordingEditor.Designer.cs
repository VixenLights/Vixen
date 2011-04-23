namespace TestEditor {
	partial class RecordingEditor {
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
			this.buttonOff = new System.Windows.Forms.Button();
			this.comboBoxController = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.panelEditorControlContainer = new System.Windows.Forms.Panel();
			this.buttonInject = new System.Windows.Forms.Button();
			this.buttonExecution = new System.Windows.Forms.Button();
			this.buttonCommandParameters = new System.Windows.Forms.Button();
			this.comboBoxCommand = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxChannel = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOff
			// 
			this.buttonOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonOff.Enabled = false;
			this.buttonOff.Location = new System.Drawing.Point(151, 268);
			this.buttonOff.Name = "buttonOff";
			this.buttonOff.Size = new System.Drawing.Size(46, 23);
			this.buttonOff.TabIndex = 23;
			this.buttonOff.Text = "Off";
			this.buttonOff.UseVisualStyleBackColor = true;
			this.buttonOff.Click += new System.EventHandler(this.buttonOff_Click);
			// 
			// comboBoxController
			// 
			this.comboBoxController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxController.FormattingEnabled = true;
			this.comboBoxController.Location = new System.Drawing.Point(103, 16);
			this.comboBoxController.Name = "comboBoxController";
			this.comboBoxController.Size = new System.Drawing.Size(154, 21);
			this.comboBoxController.TabIndex = 22;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(21, 19);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(49, 13);
			this.label4.TabIndex = 21;
			this.label4.Text = "Patching";
			// 
			// panelEditorControlContainer
			// 
			this.panelEditorControlContainer.AutoSize = true;
			this.panelEditorControlContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelEditorControlContainer.Location = new System.Drawing.Point(113, 117);
			this.panelEditorControlContainer.Name = "panelEditorControlContainer";
			this.panelEditorControlContainer.Size = new System.Drawing.Size(0, 0);
			this.panelEditorControlContainer.TabIndex = 20;
			// 
			// buttonInject
			// 
			this.buttonInject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonInject.Enabled = false;
			this.buttonInject.Location = new System.Drawing.Point(99, 268);
			this.buttonInject.Name = "buttonInject";
			this.buttonInject.Size = new System.Drawing.Size(46, 23);
			this.buttonInject.TabIndex = 19;
			this.buttonInject.Text = "On";
			this.buttonInject.UseVisualStyleBackColor = true;
			this.buttonInject.Click += new System.EventHandler(this.buttonInject_Click);
			// 
			// buttonExecution
			// 
			this.buttonExecution.Location = new System.Drawing.Point(103, 54);
			this.buttonExecution.Name = "buttonExecution";
			this.buttonExecution.Size = new System.Drawing.Size(75, 23);
			this.buttonExecution.TabIndex = 18;
			this.buttonExecution.Text = "Start";
			this.buttonExecution.UseVisualStyleBackColor = true;
			this.buttonExecution.Click += new System.EventHandler(this.buttonExecution_Click);
			// 
			// buttonCommandParameters
			// 
			this.buttonCommandParameters.Enabled = false;
			this.buttonCommandParameters.Location = new System.Drawing.Point(113, 88);
			this.buttonCommandParameters.Name = "buttonCommandParameters";
			this.buttonCommandParameters.Size = new System.Drawing.Size(75, 23);
			this.buttonCommandParameters.TabIndex = 16;
			this.buttonCommandParameters.Text = "Parameters";
			this.buttonCommandParameters.UseVisualStyleBackColor = true;
			// 
			// comboBoxCommand
			// 
			this.comboBoxCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxCommand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCommand.FormattingEnabled = true;
			this.comboBoxCommand.Location = new System.Drawing.Point(113, 61);
			this.comboBoxCommand.Name = "comboBoxCommand";
			this.comboBoxCommand.Size = new System.Drawing.Size(154, 21);
			this.comboBoxCommand.TabIndex = 15;
			this.comboBoxCommand.SelectedIndexChanged += new System.EventHandler(this.comboBoxCommand_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(31, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 14;
			this.label2.Text = "Effect";
			// 
			// comboBoxChannel
			// 
			this.comboBoxChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChannel.FormattingEnabled = true;
			this.comboBoxChannel.Location = new System.Drawing.Point(113, 27);
			this.comboBoxChannel.Name = "comboBoxChannel";
			this.comboBoxChannel.Size = new System.Drawing.Size(154, 21);
			this.comboBoxChannel.TabIndex = 13;
			this.comboBoxChannel.SelectedIndexChanged += new System.EventHandler(this.comboBoxChannel_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(31, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "Channel";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBoxController);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.buttonExecution);
			this.groupBox1.Location = new System.Drawing.Point(10, 165);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(279, 97);
			this.groupBox1.TabIndex = 24;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Execution";
			// 
			// flowLayoutPanel
			// 
			this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel.AutoScroll = true;
			this.flowLayoutPanel.Location = new System.Drawing.Point(10, 304);
			this.flowLayoutPanel.Name = "flowLayoutPanel";
			this.flowLayoutPanel.Size = new System.Drawing.Size(277, 60);
			this.flowLayoutPanel.TabIndex = 25;
			// 
			// RecordingEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(299, 376);
			this.Controls.Add(this.flowLayoutPanel);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonOff);
			this.Controls.Add(this.panelEditorControlContainer);
			this.Controls.Add(this.buttonInject);
			this.Controls.Add(this.buttonCommandParameters);
			this.Controls.Add(this.comboBoxCommand);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxChannel);
			this.Controls.Add(this.label1);
			this.Name = "RecordingEditor";
			this.Text = "RecordingEditor";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOff;
		private System.Windows.Forms.ComboBox comboBoxController;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel panelEditorControlContainer;
		private System.Windows.Forms.Button buttonInject;
		private System.Windows.Forms.Button buttonExecution;
		private System.Windows.Forms.Button buttonCommandParameters;
		private System.Windows.Forms.ComboBox comboBoxCommand;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxChannel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
	}
}