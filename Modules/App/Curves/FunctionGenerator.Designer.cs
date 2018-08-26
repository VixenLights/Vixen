namespace VixenModules.App.Curves
{
	partial class FunctionGenerator
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionGenerator));
			this.btnGenerate = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtFunction = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnGenerate
			// 
			this.btnGenerate.Location = new System.Drawing.Point(178, 130);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(75, 23);
			this.btnGenerate.TabIndex = 1;
			this.btnGenerate.Text = "Generate";
			this.btnGenerate.UseVisualStyleBackColor = true;
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(267, 130);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// txtFunction
			// 
			this.txtFunction.Location = new System.Drawing.Point(75, 69);
			this.txtFunction.Name = "txtFunction";
			this.txtFunction.Size = new System.Drawing.Size(283, 20);
			this.txtFunction.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Function";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 13);
			this.label2.MaximumSize = new System.Drawing.Size(350, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(350, 39);
			this.label2.TabIndex = 5;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// FunctionGenerator
			// 
			this.AcceptButton = this.btnGenerate;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(370, 175);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtFunction);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnGenerate);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "FunctionGenerator";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Function Curve Generator";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnGenerate;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtFunction;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}