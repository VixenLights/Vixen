namespace VixenModules.Analysis.BeatsAndBars
{
	partial class BeatsAndBarsSettings
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
			this.m_paramsGroupBox = new System.Windows.Forms.GroupBox();
			this.m_vampParamCtrl = new QMLibrary.VampParamCtrl();
			this.m_outputGroupBox = new System.Windows.Forms.GroupBox();
			this.m_vampOutputCtrl = new QMLibrary.VampOutputCtrl();
			this.button1 = new System.Windows.Forms.Button();
			this.m_paramsGroupBox.SuspendLayout();
			this.m_outputGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_paramsGroupBox
			// 
			this.m_paramsGroupBox.Controls.Add(this.m_vampParamCtrl);
			this.m_paramsGroupBox.Location = new System.Drawing.Point(12, 12);
			this.m_paramsGroupBox.Name = "m_paramsGroupBox";
			this.m_paramsGroupBox.Size = new System.Drawing.Size(350, 24);
			this.m_paramsGroupBox.TabIndex = 2;
			this.m_paramsGroupBox.TabStop = false;
			this.m_paramsGroupBox.Text = "Parameters";
			// 
			// m_vampParamCtrl
			// 
			this.m_vampParamCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_vampParamCtrl.AutoSize = true;
			this.m_vampParamCtrl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_vampParamCtrl.Location = new System.Drawing.Point(6, 19);
			this.m_vampParamCtrl.Name = "m_vampParamCtrl";
			this.m_vampParamCtrl.Size = new System.Drawing.Size(344, 4);
			this.m_vampParamCtrl.TabIndex = 0;
			// 
			// m_outputGroupBox
			// 
			this.m_outputGroupBox.Controls.Add(this.m_vampOutputCtrl);
			this.m_outputGroupBox.Location = new System.Drawing.Point(13, 64);
			this.m_outputGroupBox.Name = "m_outputGroupBox";
			this.m_outputGroupBox.Size = new System.Drawing.Size(349, 42);
			this.m_outputGroupBox.TabIndex = 3;
			this.m_outputGroupBox.TabStop = false;
			this.m_outputGroupBox.Text = "Collections to Generate";
			// 
			// m_vampOutputCtrl
			// 
			this.m_vampOutputCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_vampOutputCtrl.AutoSize = true;
			this.m_vampOutputCtrl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_vampOutputCtrl.Location = new System.Drawing.Point(11, 19);
			this.m_vampOutputCtrl.Name = "m_vampOutputCtrl";
			this.m_vampOutputCtrl.Size = new System.Drawing.Size(332, 4);
			this.m_vampOutputCtrl.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(122, 112);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// BeatsAndBarsSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(404, 147);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.m_outputGroupBox);
			this.Controls.Add(this.m_paramsGroupBox);
			this.Name = "BeatsAndBarsSettings";
			this.Text = "BeatsAndBarsSettings";
			this.m_paramsGroupBox.ResumeLayout(false);
			this.m_paramsGroupBox.PerformLayout();
			this.m_outputGroupBox.ResumeLayout(false);
			this.m_outputGroupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private QMLibrary.VampParamCtrl m_vampParamCtrl;
		private QMLibrary.VampOutputCtrl m_vampOutputCtrl;
		private System.Windows.Forms.GroupBox m_paramsGroupBox;
		private System.Windows.Forms.GroupBox m_outputGroupBox;
		private System.Windows.Forms.Button button1;

	}
}