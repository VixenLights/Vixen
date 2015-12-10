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
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonNo = new System.Windows.Forms.Button();
			this.messageIcon = new System.Windows.Forms.PictureBox();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.messageIcon)).BeginInit();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonOk.BackColor = System.Drawing.SystemColors.Control;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOk.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonOk.Location = new System.Drawing.Point(3, 69);
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
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.buttonCancel.Location = new System.Drawing.Point(201, 69);
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
			this.buttonNo.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
			this.buttonNo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonNo.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.buttonNo.Location = new System.Drawing.Point(102, 69);
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
			// txtMessage
			// 
			this.txtMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtMessage.Cursor = System.Windows.Forms.Cursors.Default;
			this.txtMessage.Location = new System.Drawing.Point(3, 3);
			this.txtMessage.MinimumSize = new System.Drawing.Size(290, 60);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ReadOnly = true;
			this.txtMessage.Size = new System.Drawing.Size(291, 60);
			this.txtMessage.TabIndex = 6;
			this.txtMessage.TabStop = false;
			this.txtMessage.Text = "{ Message }";
			this.txtMessage.TextChanged += new System.EventHandler(this.txtMessage_TextChanged);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this.txtMessage);
			this.flowLayoutPanel1.Controls.Add(this.label1);
			this.flowLayoutPanel1.Controls.Add(this.buttonOk);
			this.flowLayoutPanel1.Controls.Add(this.buttonNo);
			this.flowLayoutPanel1.Controls.Add(this.buttonCancel);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(89, 13);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(297, 101);
			this.flowLayoutPanel1.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.flowLayoutPanel1.SetFlowBreak(this.label1, true);
			this.label1.Location = new System.Drawing.Point(297, 0);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 15);
			this.label1.TabIndex = 7;
			// 
			// MessageBoxForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(401, 126);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.messageIcon);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MessageBoxForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select more marks";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MessageBoxForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.messageIcon)).EndInit();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private Button buttonCancel;
		private Button buttonNo;
		private PictureBox messageIcon;
		private TextBox txtMessage;
		private FlowLayoutPanel flowLayoutPanel1;
		private Label label1;
	}
}