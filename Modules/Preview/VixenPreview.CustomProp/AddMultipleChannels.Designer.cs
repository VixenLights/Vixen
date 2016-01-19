namespace VixenModules.Preview.VixenPreview.CustomProp
{
	partial class AddMultipleChannels
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
			if (disposing && (components != null))
			{
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
			this.txtTemplate = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.numChannelsToAdd = new System.Windows.Forms.NumericUpDown();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numChannelsToAdd)).BeginInit();
			this.SuspendLayout();
			// 
			// txtTemplate
			// 
			this.txtTemplate.Location = new System.Drawing.Point(26, 93);
			this.txtTemplate.Name = "txtTemplate";
			this.txtTemplate.Size = new System.Drawing.Size(123, 20);
			this.txtTemplate.TabIndex = 11;
			this.txtTemplate.Text = "Channel_";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Name Template";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(137, 13);
			this.label1.TabIndex = 9;
			this.label1.Text = "Number of Channels to Add";
			// 
			// numChannelsToAdd
			// 
			this.numChannelsToAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numChannelsToAdd.Location = new System.Drawing.Point(26, 26);
			this.numChannelsToAdd.Name = "numChannelsToAdd";
			this.numChannelsToAdd.Size = new System.Drawing.Size(101, 44);
			this.numChannelsToAdd.TabIndex = 8;
			this.numChannelsToAdd.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(168, 45);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 34);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(168, 10);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 29);
			this.btnOk.TabIndex = 6;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// AddMultipleChannels
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(258, 122);
			this.ControlBox = false;
			this.Controls.Add(this.txtTemplate);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numChannelsToAdd);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Name = "AddMultipleChannels";
			this.Text = "Add Multiple Channels";
			((System.ComponentModel.ISupportInitialize)(this.numChannelsToAdd)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtTemplate;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		internal System.Windows.Forms.NumericUpDown numChannelsToAdd;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
	}
}