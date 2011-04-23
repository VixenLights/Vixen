namespace TestTemplate {
	partial class OutputControllerTemplateSetup {
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
			this.comboBoxTransforms = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxTransforms = new System.Windows.Forms.ListBox();
			this.buttonAddTransform = new System.Windows.Forms.Button();
			this.buttonRemoveTransform = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonTransformSetup = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Data Transforms";
			// 
			// comboBoxTransforms
			// 
			this.comboBoxTransforms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTransforms.FormattingEnabled = true;
			this.comboBoxTransforms.Location = new System.Drawing.Point(113, 20);
			this.comboBoxTransforms.Name = "comboBoxTransforms";
			this.comboBoxTransforms.Size = new System.Drawing.Size(225, 21);
			this.comboBoxTransforms.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(290, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Transforms to apply to every new output controller\'s outputs:";
			// 
			// listBoxTransforms
			// 
			this.listBoxTransforms.FormattingEnabled = true;
			this.listBoxTransforms.Location = new System.Drawing.Point(25, 95);
			this.listBoxTransforms.Name = "listBoxTransforms";
			this.listBoxTransforms.Size = new System.Drawing.Size(394, 69);
			this.listBoxTransforms.TabIndex = 3;
			// 
			// buttonAddTransform
			// 
			this.buttonAddTransform.Location = new System.Drawing.Point(344, 18);
			this.buttonAddTransform.Name = "buttonAddTransform";
			this.buttonAddTransform.Size = new System.Drawing.Size(75, 23);
			this.buttonAddTransform.TabIndex = 4;
			this.buttonAddTransform.Text = "Add";
			this.buttonAddTransform.UseVisualStyleBackColor = true;
			this.buttonAddTransform.Click += new System.EventHandler(this.buttonAddTransform_Click);
			// 
			// buttonRemoveTransform
			// 
			this.buttonRemoveTransform.Location = new System.Drawing.Point(146, 172);
			this.buttonRemoveTransform.Name = "buttonRemoveTransform";
			this.buttonRemoveTransform.Size = new System.Drawing.Size(75, 23);
			this.buttonRemoveTransform.TabIndex = 5;
			this.buttonRemoveTransform.Text = "Remove";
			this.buttonRemoveTransform.UseVisualStyleBackColor = true;
			this.buttonRemoveTransform.Click += new System.EventHandler(this.buttonRemoveTransform_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(285, 172);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(366, 172);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonTransformSetup
			// 
			this.buttonTransformSetup.Location = new System.Drawing.Point(25, 172);
			this.buttonTransformSetup.Name = "buttonTransformSetup";
			this.buttonTransformSetup.Size = new System.Drawing.Size(75, 23);
			this.buttonTransformSetup.TabIndex = 8;
			this.buttonTransformSetup.Text = "Setup";
			this.buttonTransformSetup.UseVisualStyleBackColor = true;
			this.buttonTransformSetup.Click += new System.EventHandler(this.buttonTransformSetup_Click);
			// 
			// TemplateSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(453, 207);
			this.Controls.Add(this.buttonTransformSetup);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonRemoveTransform);
			this.Controls.Add(this.buttonAddTransform);
			this.Controls.Add(this.listBoxTransforms);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxTransforms);
			this.Controls.Add(this.label1);
			this.Name = "TemplateSetup";
			this.Text = "TemplateSetup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxTransforms;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBoxTransforms;
		private System.Windows.Forms.Button buttonAddTransform;
		private System.Windows.Forms.Button buttonRemoveTransform;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonTransformSetup;
	}
}