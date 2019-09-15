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
			this.lblQuestion = new System.Windows.Forms.Label();
			this.radioButtonExistingUpdate = new System.Windows.Forms.RadioButton();
			this.radioButtonExistingAddNew = new System.Windows.Forms.RadioButton();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.radioButtonInsertAfter = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(319, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "This will set up a dimming curve for each selected element.";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(28, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(430, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Use the button below to configure the dimming curve for the selected elements.";
			// 
			// buttonSetupCurve
			// 
			this.buttonSetupCurve.Location = new System.Drawing.Point(189, 110);
			this.buttonSetupCurve.Name = "buttonSetupCurve";
			this.buttonSetupCurve.Size = new System.Drawing.Size(152, 35);
			this.buttonSetupCurve.TabIndex = 2;
			this.buttonSetupCurve.Text = "Setup Dimming Curve";
			this.buttonSetupCurve.UseVisualStyleBackColor = true;
			this.buttonSetupCurve.Click += new System.EventHandler(this.buttonSetupCurve_Click);
			this.buttonSetupCurve.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonSetupCurve.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// radioButtonExistingDoNothing
			// 
			this.radioButtonExistingDoNothing.AutoSize = true;
			this.radioButtonExistingDoNothing.Checked = true;
			this.radioButtonExistingDoNothing.Location = new System.Drawing.Point(31, 188);
			this.radioButtonExistingDoNothing.Name = "radioButtonExistingDoNothing";
			this.radioButtonExistingDoNothing.Size = new System.Drawing.Size(237, 19);
			this.radioButtonExistingDoNothing.TabIndex = 3;
			this.radioButtonExistingDoNothing.TabStop = true;
			this.radioButtonExistingDoNothing.Text = "Leave the existing dimming curve alone.";
			this.radioButtonExistingDoNothing.UseVisualStyleBackColor = true;
			// 
			// lblQuestion
			// 
			this.lblQuestion.AutoSize = true;
			this.lblQuestion.Location = new System.Drawing.Point(28, 159);
			this.lblQuestion.Name = "lblQuestion";
			this.lblQuestion.Size = new System.Drawing.Size(447, 15);
			this.lblQuestion.TabIndex = 4;
			this.lblQuestion.Text = "If some elements are already patched to a dimming curve, what do you want to do?";
			// 
			// radioButtonExistingUpdate
			// 
			this.radioButtonExistingUpdate.AutoSize = true;
			this.radioButtonExistingUpdate.Location = new System.Drawing.Point(31, 215);
			this.radioButtonExistingUpdate.Name = "radioButtonExistingUpdate";
			this.radioButtonExistingUpdate.Size = new System.Drawing.Size(310, 19);
			this.radioButtonExistingUpdate.TabIndex = 5;
			this.radioButtonExistingUpdate.TabStop = true;
			this.radioButtonExistingUpdate.Text = "Update the existing dimming curve with the new data.";
			this.radioButtonExistingUpdate.UseVisualStyleBackColor = true;
			// 
			// radioButtonExistingAddNew
			// 
			this.radioButtonExistingAddNew.AutoSize = true;
			this.radioButtonExistingAddNew.Location = new System.Drawing.Point(31, 265);
			this.radioButtonExistingAddNew.Name = "radioButtonExistingAddNew";
			this.radioButtonExistingAddNew.Size = new System.Drawing.Size(321, 19);
			this.radioButtonExistingAddNew.TabIndex = 6;
			this.radioButtonExistingAddNew.TabStop = true;
			this.radioButtonExistingAddNew.Text = "Add a new dimming curve anyway (not recommended).";
			this.radioButtonExistingAddNew.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(399, 319);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 24;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Enabled = false;
			this.buttonOk.Location = new System.Drawing.Point(287, 319);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 23;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(138, 92);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(254, 15);
			this.label4.TabIndex = 25;
			this.label4.Text = "Hint: Use a library curve to make editing easier.";
			// 
			// radioButtonInsertAfter
			// 
			this.radioButtonInsertAfter.AutoSize = true;
			this.radioButtonInsertAfter.Location = new System.Drawing.Point(31, 240);
			this.radioButtonInsertAfter.Name = "radioButtonInsertAfter";
			this.radioButtonInsertAfter.Size = new System.Drawing.Size(268, 19);
			this.radioButtonInsertAfter.TabIndex = 26;
			this.radioButtonInsertAfter.TabStop = true;
			this.radioButtonInsertAfter.Text = "Insert a new dimming curve after the element.";
			this.radioButtonInsertAfter.UseVisualStyleBackColor = true;
			// 
			// DimmingCurveHelper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(518, 363);
			this.Controls.Add(this.radioButtonInsertAfter);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.radioButtonExistingAddNew);
			this.Controls.Add(this.radioButtonExistingUpdate);
			this.Controls.Add(this.lblQuestion);
			this.Controls.Add(this.radioButtonExistingDoNothing);
			this.Controls.Add(this.buttonSetupCurve);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(534, 376);
			this.Name = "DimmingCurveHelper";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Dimming Curve Configuration";
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.DimmingCurveHelper_HelpButtonClicked);
			this.Load += new System.EventHandler(this.DimmingCurveHelper_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonSetupCurve;
		private System.Windows.Forms.RadioButton radioButtonExistingDoNothing;
		private System.Windows.Forms.Label lblQuestion;
		private System.Windows.Forms.RadioButton radioButtonExistingUpdate;
		private System.Windows.Forms.RadioButton radioButtonExistingAddNew;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton radioButtonInsertAfter;
	}
}