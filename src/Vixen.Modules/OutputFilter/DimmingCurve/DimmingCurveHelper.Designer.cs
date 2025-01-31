using Common.Resources.Properties;
using Common.Resources;
using Common.Controls.Scaling;
using System.Resources;

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
			label1 = new Label();
			label2 = new Label();
			buttonSetupCurve = new Button();
			radioButtonExistingDoNothing = new RadioButton();
			lblQuestion = new Label();
			radioButtonExistingUpdate = new RadioButton();
			radioButtonExistingAddNew = new RadioButton();
			buttonHelp = new Button();
			buttonCancel = new Button();
			buttonOk = new Button();
			label4 = new Label();
			radioButtonInsertAfter = new RadioButton();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(28, 35);
			label1.Name = "label1";
			label1.Size = new Size(318, 15);
			label1.TabIndex = 0;
			label1.Text = "This will set up a dimming curve for each selected element.";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(28, 67);
			label2.Name = "label2";
			label2.Size = new Size(430, 15);
			label2.TabIndex = 1;
			label2.Text = "Use the button below to configure the dimming curve for the selected elements.";
			// 
			// buttonSetupCurve
			// 
			buttonSetupCurve.Location = new Point(189, 110);
			buttonSetupCurve.Name = "buttonSetupCurve";
			buttonSetupCurve.Size = new Size(152, 35);
			buttonSetupCurve.TabIndex = 2;
			buttonSetupCurve.Text = "Setup Dimming Curve";
			buttonSetupCurve.UseVisualStyleBackColor = true;
			buttonSetupCurve.Click += buttonSetupCurve_Click;
			// 
			// radioButtonExistingDoNothing
			// 
			radioButtonExistingDoNothing.AutoSize = true;
			radioButtonExistingDoNothing.Checked = true;
			radioButtonExistingDoNothing.Location = new Point(31, 188);
			radioButtonExistingDoNothing.Name = "radioButtonExistingDoNothing";
			radioButtonExistingDoNothing.Size = new Size(237, 19);
			radioButtonExistingDoNothing.TabIndex = 3;
			radioButtonExistingDoNothing.TabStop = true;
			radioButtonExistingDoNothing.Text = "Leave the existing dimming curve alone.";
			radioButtonExistingDoNothing.UseVisualStyleBackColor = true;
			// 
			// lblQuestion
			// 
			lblQuestion.AutoSize = true;
			lblQuestion.Location = new Point(28, 159);
			lblQuestion.Name = "lblQuestion";
			lblQuestion.Size = new Size(447, 15);
			lblQuestion.TabIndex = 4;
			lblQuestion.Text = "If some elements are already patched to a dimming curve, what do you want to do?";
			// 
			// radioButtonExistingUpdate
			// 
			radioButtonExistingUpdate.AutoSize = true;
			radioButtonExistingUpdate.Location = new Point(31, 215);
			radioButtonExistingUpdate.Name = "radioButtonExistingUpdate";
			radioButtonExistingUpdate.Size = new Size(311, 19);
			radioButtonExistingUpdate.TabIndex = 5;
			radioButtonExistingUpdate.TabStop = true;
			radioButtonExistingUpdate.Text = "Update the existing dimming curve with the new data.";
			radioButtonExistingUpdate.UseVisualStyleBackColor = true;
			// 
			// radioButtonExistingAddNew
			// 
			radioButtonExistingAddNew.AutoSize = true;
			radioButtonExistingAddNew.Location = new Point(31, 265);
			radioButtonExistingAddNew.Name = "radioButtonExistingAddNew";
			radioButtonExistingAddNew.Size = new Size(321, 19);
			radioButtonExistingAddNew.TabIndex = 6;
			radioButtonExistingAddNew.TabStop = true;
			radioButtonExistingAddNew.Text = "Add a new dimming curve anyway (not recommended).";
			radioButtonExistingAddNew.UseVisualStyleBackColor = true;
			// 
			// buttonHelp
			// 
			this.buttonHelp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonHelp.Location = new System.Drawing.Point(14, 319);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(70, 27);
			this.buttonHelp.TabIndex = 63;
			this.buttonHelp.Tag = Common.VixenHelp.VixenHelp.HelpStrings.Patching;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonHelp.UseVisualStyleBackColor = true;
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			this.buttonHelp.Image = Common.Resources.Tools.GetIcon(Resources.help, (int)(16 * ScalingTools.GetScaleFactor()));
			this.buttonHelp.Click += new System.EventHandler(this.DimmingCurveHelper_HelpButtonClicked);
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.Location = new Point(399, 319);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(105, 29);
			buttonCancel.TabIndex = 24;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonOk.DialogResult = DialogResult.OK;
			buttonOk.Enabled = false;
			buttonOk.Location = new Point(287, 319);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new Size(105, 29);
			buttonOk.TabIndex = 23;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(138, 92);
			label4.Name = "label4";
			label4.Size = new Size(254, 15);
			label4.TabIndex = 25;
			label4.Text = "Hint: Use a library curve to make editing easier.";
			// 
			// radioButtonInsertAfter
			// 
			radioButtonInsertAfter.AutoSize = true;
			radioButtonInsertAfter.Location = new Point(31, 240);
			radioButtonInsertAfter.Name = "radioButtonInsertAfter";
			radioButtonInsertAfter.Size = new Size(268, 19);
			radioButtonInsertAfter.TabIndex = 26;
			radioButtonInsertAfter.TabStop = true;
			radioButtonInsertAfter.Text = "Insert a new dimming curve after the element.";
			radioButtonInsertAfter.UseVisualStyleBackColor = true;
			// 
			// DimmingCurveHelper
			// 
			AcceptButton = buttonOk;
			CancelButton = buttonCancel;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			ClientSize = new Size(518, 363);
			Controls.Add(radioButtonInsertAfter);
			Controls.Add(label4);
			Controls.Add(buttonCancel);
			Controls.Add(buttonHelp);
			Controls.Add(buttonOk);
			Controls.Add(radioButtonExistingAddNew);
			Controls.Add(radioButtonExistingUpdate);
			Controls.Add(lblQuestion);
			Controls.Add(radioButtonExistingDoNothing);
			Controls.Add(buttonSetupCurve);
			Controls.Add(label2);
			Controls.Add(label1);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(534, 376);
			Name = "DimmingCurveHelper";
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Dimming Curve Configuration";
			HelpButtonClicked += DimmingCurveHelper_HelpButtonClicked;
			Load += DimmingCurveHelper_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonSetupCurve;
		private System.Windows.Forms.RadioButton radioButtonExistingDoNothing;
		private System.Windows.Forms.Label lblQuestion;
		private System.Windows.Forms.RadioButton radioButtonExistingUpdate;
		private System.Windows.Forms.RadioButton radioButtonExistingAddNew;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton radioButtonInsertAfter;
	}
}