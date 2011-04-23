namespace TestEditor {
	partial class LiveEditor {
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
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxChannel = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxCommand = new System.Windows.Forms.ComboBox();
			this.buttonCommandParameters = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonExecution = new System.Windows.Forms.Button();
			this.buttonInject = new System.Windows.Forms.Button();
			this.panelEditorControlContainer = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxController = new System.Windows.Forms.ComboBox();
			this.buttonOff = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Channel";
			// 
			// comboBoxChannel
			// 
			this.comboBoxChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxChannel.FormattingEnabled = true;
			this.comboBoxChannel.Location = new System.Drawing.Point(102, 22);
			this.comboBoxChannel.Name = "comboBoxChannel";
			this.comboBoxChannel.Size = new System.Drawing.Size(154, 21);
			this.comboBoxChannel.TabIndex = 1;
			this.comboBoxChannel.SelectedIndexChanged += new System.EventHandler(this.comboBoxChannel_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Command";
			// 
			// comboBoxCommand
			// 
			this.comboBoxCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxCommand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCommand.FormattingEnabled = true;
			this.comboBoxCommand.Location = new System.Drawing.Point(102, 56);
			this.comboBoxCommand.Name = "comboBoxCommand";
			this.comboBoxCommand.Size = new System.Drawing.Size(154, 21);
			this.comboBoxCommand.TabIndex = 3;
			this.comboBoxCommand.SelectedIndexChanged += new System.EventHandler(this.comboBoxCommand_SelectedIndexChanged);
			// 
			// buttonCommandParameters
			// 
			this.buttonCommandParameters.Enabled = false;
			this.buttonCommandParameters.Location = new System.Drawing.Point(102, 83);
			this.buttonCommandParameters.Name = "buttonCommandParameters";
			this.buttonCommandParameters.Size = new System.Drawing.Size(75, 23);
			this.buttonCommandParameters.TabIndex = 4;
			this.buttonCommandParameters.Text = "Parameters";
			this.buttonCommandParameters.UseVisualStyleBackColor = true;
			this.buttonCommandParameters.Click += new System.EventHandler(this.buttonCommandParameters_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(20, 219);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Execution";
			// 
			// buttonExecution
			// 
			this.buttonExecution.Location = new System.Drawing.Point(102, 214);
			this.buttonExecution.Name = "buttonExecution";
			this.buttonExecution.Size = new System.Drawing.Size(75, 23);
			this.buttonExecution.TabIndex = 6;
			this.buttonExecution.Text = "Start";
			this.buttonExecution.UseVisualStyleBackColor = true;
			this.buttonExecution.Click += new System.EventHandler(this.buttonExecution_Click);
			// 
			// buttonInject
			// 
			this.buttonInject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonInject.Enabled = false;
			this.buttonInject.Location = new System.Drawing.Point(102, 322);
			this.buttonInject.Name = "buttonInject";
			this.buttonInject.Size = new System.Drawing.Size(46, 23);
			this.buttonInject.TabIndex = 7;
			this.buttonInject.Text = "On";
			this.buttonInject.UseVisualStyleBackColor = true;
			this.buttonInject.Click += new System.EventHandler(this.buttonInject_Click);
			this.buttonInject.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonInject_MouseDown);
			this.buttonInject.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonInject_MouseUp);
			// 
			// panelEditorControlContainer
			// 
			this.panelEditorControlContainer.AutoSize = true;
			this.panelEditorControlContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelEditorControlContainer.Location = new System.Drawing.Point(102, 112);
			this.panelEditorControlContainer.Name = "panelEditorControlContainer";
			this.panelEditorControlContainer.Size = new System.Drawing.Size(0, 0);
			this.panelEditorControlContainer.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(20, 148);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(49, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Patching";
			// 
			// comboBoxController
			// 
			this.comboBoxController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxController.FormattingEnabled = true;
			this.comboBoxController.Location = new System.Drawing.Point(102, 145);
			this.comboBoxController.Name = "comboBoxController";
			this.comboBoxController.Size = new System.Drawing.Size(154, 21);
			this.comboBoxController.TabIndex = 10;
			// 
			// buttonOff
			// 
			this.buttonOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonOff.Enabled = false;
			this.buttonOff.Location = new System.Drawing.Point(154, 322);
			this.buttonOff.Name = "buttonOff";
			this.buttonOff.Size = new System.Drawing.Size(46, 23);
			this.buttonOff.TabIndex = 11;
			this.buttonOff.Text = "Off";
			this.buttonOff.UseVisualStyleBackColor = true;
			this.buttonOff.Click += new System.EventHandler(this.buttonOff_Click);
			// 
			// LiveEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(294, 357);
			this.Controls.Add(this.buttonOff);
			this.Controls.Add(this.comboBoxController);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.panelEditorControlContainer);
			this.Controls.Add(this.buttonInject);
			this.Controls.Add(this.buttonExecution);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonCommandParameters);
			this.Controls.Add(this.comboBoxCommand);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxChannel);
			this.Controls.Add(this.label1);
			this.Name = "LiveEditor";
			this.Text = "LiveEditor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxChannel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxCommand;
		private System.Windows.Forms.Button buttonCommandParameters;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonExecution;
		private System.Windows.Forms.Button buttonInject;
		private System.Windows.Forms.Panel panelEditorControlContainer;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxController;
		private System.Windows.Forms.Button buttonOff;
	}
}