using System.Windows.Forms;

namespace Common.Controls
{
	partial class MessageBoxForm
	{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageBoxForm));
			this.labelPrompt = new System.Windows.Forms.Label();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonNo = new System.Windows.Forms.Button();
			this.messageIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.messageIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// labelPrompt
			// 
			this.labelPrompt.AutoSize = true;
			this.labelPrompt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.labelPrompt.Location = new System.Drawing.Point(106, 27);
			this.labelPrompt.MaximumSize = new System.Drawing.Size(350, 200);
			this.labelPrompt.Name = "labelPrompt";
			this.labelPrompt.Size = new System.Drawing.Size(15, 15);
			this.labelPrompt.TabIndex = 0;
			this.labelPrompt.Text = "[]";
			this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.BackColor = System.Drawing.SystemColors.Control;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOk.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonOk.Location = new System.Drawing.Point(91, 91);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(93, 29);
			this.buttonOk.TabIndex = 2;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = false;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.buttonCancel.Location = new System.Drawing.Point(311, 91);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(93, 29);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "CANCEL";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Visible = false;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonNo
			// 
			this.buttonNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
			this.buttonNo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonNo.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.buttonNo.Location = new System.Drawing.Point(203, 91);
			this.buttonNo.Name = "buttonNo";
			this.buttonNo.Size = new System.Drawing.Size(93, 29);
			this.buttonNo.TabIndex = 4;
			this.buttonNo.Text = "NO";
			this.buttonNo.UseVisualStyleBackColor = true;
			this.buttonNo.Visible = false;
			this.buttonNo.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonNo.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// messageIcon
			// 
			this.messageIcon.Location = new System.Drawing.Point(6, 13);
			this.messageIcon.Margin = new System.Windows.Forms.Padding(2);
			this.messageIcon.Name = "messageIcon";
			this.messageIcon.Size = new System.Drawing.Size(78, 69);
			this.messageIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.messageIcon.TabIndex = 5;
			this.messageIcon.TabStop = false;
			this.messageIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.messageIcon_Paint);
			// 
			// MessageBoxForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(416, 132);
			this.Controls.Add(this.messageIcon);
			this.Controls.Add(this.buttonNo);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.labelPrompt);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(528, 198);
			this.MinimizeBox = false;
			this.Name = "MessageBoxForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select more marks";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MessageBoxForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.messageIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelPrompt;
		private System.Windows.Forms.Button buttonOk;
		private Button buttonCancel;
		private Button buttonNo;
		private PictureBox messageIcon;
	}
}