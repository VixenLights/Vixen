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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.txtMessage = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.messageIcon)).BeginInit();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.AutoSize = true;
			this.buttonOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOk.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonOk.Location = new System.Drawing.Point(3, 3);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(87, 27);
			this.buttonOk.TabIndex = 2;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = false;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.AutoSize = true;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.buttonCancel.Location = new System.Drawing.Point(189, 3);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "CANCEL";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Visible = false;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonNo
			// 
			this.buttonNo.AutoSize = true;
			this.buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
			this.buttonNo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonNo.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.buttonNo.Location = new System.Drawing.Point(96, 3);
			this.buttonNo.Name = "buttonNo";
			this.buttonNo.Size = new System.Drawing.Size(87, 27);
			this.buttonNo.TabIndex = 4;
			this.buttonNo.Text = "NO";
			this.buttonNo.UseVisualStyleBackColor = true;
			this.buttonNo.Visible = false;
			this.buttonNo.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonNo.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// messageIcon
			// 
			this.messageIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.messageIcon.Location = new System.Drawing.Point(2, 2);
			this.messageIcon.Margin = new System.Windows.Forms.Padding(2);
			this.messageIcon.Name = "messageIcon";
			this.messageIcon.Size = new System.Drawing.Size(95, 94);
			this.messageIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.messageIcon.TabIndex = 5;
			this.messageIcon.TabStop = false;
			this.messageIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.messageIcon_Paint);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
			this.flowLayoutPanel1.Controls.Add(this.buttonCancel);
			this.flowLayoutPanel1.Controls.Add(this.buttonNo);
			this.flowLayoutPanel1.Controls.Add(this.buttonOk);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(114, 101);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(279, 33);
			this.flowLayoutPanel1.TabIndex = 7;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
			this.tableLayoutPanel1.Controls.Add(this.messageIcon, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.txtMessage, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(396, 143);
			this.tableLayoutPanel1.TabIndex = 8;
			// 
			// txtMessage
			// 
			this.txtMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtMessage.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.txtMessage.Location = new System.Drawing.Point(101, 5);
			this.txtMessage.Margin = new System.Windows.Forms.Padding(2, 5, 5, 1);
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ReadOnly = true;
			this.txtMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.txtMessage.Size = new System.Drawing.Size(290, 92);
			this.txtMessage.TabIndex = 8;
			this.txtMessage.Text = "";
			// 
			// MessageBoxForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.ClientSize = new System.Drawing.Size(396, 143);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MessageBoxForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select more marks";
			this.TopMost = true;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MessageBoxForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.messageIcon)).EndInit();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private Button buttonCancel;
		private Button buttonNo;
		private PictureBox messageIcon;
		private FlowLayoutPanel flowLayoutPanel1;
		private TableLayoutPanel tableLayoutPanel1;
		private RichTextBox txtMessage;
	}
}