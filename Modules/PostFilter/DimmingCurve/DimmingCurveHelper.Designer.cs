namespace VixenModules.OutputFilter.DimmingCurve
{
	partial class DimmingCurveHelper
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSetupCurve = new System.Windows.Forms.Button();
            this.radioButtonExistingDoNothing = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonExistingUpdate = new System.Windows.Forms.RadioButton();
            this.radioButtonExistingAddNew = new System.Windows.Forms.RadioButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This will set up a dimming curve filter for each selected element.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(259, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Use the button below to configure the dimming curve.";
            // 
            // buttonSetupCurve
            // 
            this.buttonSetupCurve.Location = new System.Drawing.Point(156, 87);
            this.buttonSetupCurve.Name = "buttonSetupCurve";
            this.buttonSetupCurve.Size = new System.Drawing.Size(130, 30);
            this.buttonSetupCurve.TabIndex = 2;
            this.buttonSetupCurve.Text = "Setup Dimming Curve";
            this.buttonSetupCurve.UseVisualStyleBackColor = true;
            this.buttonSetupCurve.Click += new System.EventHandler(this.buttonSetupCurve_Click);
            // 
            // radioButtonExistingDoNothing
            // 
            this.radioButtonExistingDoNothing.AutoSize = true;
            this.radioButtonExistingDoNothing.Checked = true;
            this.radioButtonExistingDoNothing.Location = new System.Drawing.Point(27, 163);
            this.radioButtonExistingDoNothing.Name = "radioButtonExistingDoNothing";
            this.radioButtonExistingDoNothing.Size = new System.Drawing.Size(214, 17);
            this.radioButtonExistingDoNothing.TabIndex = 3;
            this.radioButtonExistingDoNothing.TabStop = true;
            this.radioButtonExistingDoNothing.Text = "Leave the existing dimming curve alone.";
            this.radioButtonExistingDoNothing.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(398, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "If some elements are already patched to a dimming curve, what do you want to do?";
            // 
            // radioButtonExistingUpdate
            // 
            this.radioButtonExistingUpdate.AutoSize = true;
            this.radioButtonExistingUpdate.Location = new System.Drawing.Point(27, 186);
            this.radioButtonExistingUpdate.Name = "radioButtonExistingUpdate";
            this.radioButtonExistingUpdate.Size = new System.Drawing.Size(277, 17);
            this.radioButtonExistingUpdate.TabIndex = 5;
            this.radioButtonExistingUpdate.TabStop = true;
            this.radioButtonExistingUpdate.Text = "Update the existing dimming curve with the new data.";
            this.radioButtonExistingUpdate.UseVisualStyleBackColor = true;
            // 
            // radioButtonExistingAddNew
            // 
            this.radioButtonExistingAddNew.AutoSize = true;
            this.radioButtonExistingAddNew.Location = new System.Drawing.Point(27, 209);
            this.radioButtonExistingAddNew.Name = "radioButtonExistingAddNew";
            this.radioButtonExistingAddNew.Size = new System.Drawing.Size(283, 17);
            this.radioButtonExistingAddNew.TabIndex = 6;
            this.radioButtonExistingAddNew.TabStop = true;
            this.radioButtonExistingAddNew.Text = "Add a new dimming curve anyway (not recommended).";
            this.radioButtonExistingAddNew.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(342, 255);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(90, 25);
            this.buttonCancel.TabIndex = 24;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(246, 255);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(90, 25);
            this.buttonOk.TabIndex = 23;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // DimmingCurveHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 292);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.radioButtonExistingAddNew);
            this.Controls.Add(this.radioButtonExistingUpdate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.radioButtonExistingDoNothing);
            this.Controls.Add(this.buttonSetupCurve);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Name = "DimmingCurveHelper";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dimming Curve Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonSetupCurve;
		private System.Windows.Forms.RadioButton radioButtonExistingDoNothing;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton radioButtonExistingUpdate;
		private System.Windows.Forms.RadioButton radioButtonExistingAddNew;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
	}
}