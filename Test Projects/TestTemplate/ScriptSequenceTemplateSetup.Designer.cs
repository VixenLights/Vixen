namespace TestTemplate {
	partial class ScriptSequenceTemplateSetup {
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
			this.checkedListBoxRuntimeBehaviors = new System.Windows.Forms.CheckedListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxLength = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxForever = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkedListBoxRuntimeBehaviors
			// 
			this.checkedListBoxRuntimeBehaviors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.checkedListBoxRuntimeBehaviors.CheckOnClick = true;
			this.checkedListBoxRuntimeBehaviors.FormattingEnabled = true;
			this.checkedListBoxRuntimeBehaviors.Location = new System.Drawing.Point(15, 45);
			this.checkedListBoxRuntimeBehaviors.Name = "checkedListBoxRuntimeBehaviors";
			this.checkedListBoxRuntimeBehaviors.Size = new System.Drawing.Size(254, 139);
			this.checkedListBoxRuntimeBehaviors.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(257, 27);
			this.label1.TabIndex = 1;
			this.label1.Text = "Select the runtime behaviors a new script sequence will start with:";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(194, 267);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(113, 267);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 200);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Initial length:";
			// 
			// textBoxLength
			// 
			this.textBoxLength.Location = new System.Drawing.Point(125, 197);
			this.textBoxLength.Name = "textBoxLength";
			this.textBoxLength.Size = new System.Drawing.Size(53, 20);
			this.textBoxLength.TabIndex = 11;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(184, 200);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(20, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "ms";
			// 
			// checkBoxForever
			// 
			this.checkBoxForever.AutoSize = true;
			this.checkBoxForever.Location = new System.Drawing.Point(210, 199);
			this.checkBoxForever.Name = "checkBoxForever";
			this.checkBoxForever.Size = new System.Drawing.Size(62, 17);
			this.checkBoxForever.TabIndex = 16;
			this.checkBoxForever.Text = "Forever";
			this.checkBoxForever.UseVisualStyleBackColor = true;
			this.checkBoxForever.CheckedChanged += new System.EventHandler(this.checkBoxForever_CheckedChanged);
			// 
			// ScriptSequenceTemplateSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(281, 302);
			this.Controls.Add(this.checkBoxForever);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxLength);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkedListBoxRuntimeBehaviors);
			this.Name = "ScriptSequenceTemplateSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ScriptSequenceTemplateSetup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox checkedListBoxRuntimeBehaviors;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxLength;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxForever;
	}
}