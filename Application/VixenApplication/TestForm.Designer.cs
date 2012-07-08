namespace VixenApplication {
	partial class TestForm {
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
			this.comboBoxEffect = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkedListBox = new System.Windows.Forms.CheckedListBox();
			this.trackBar = new System.Windows.Forms.TrackBar();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.buttonSetEffectParameters = new System.Windows.Forms.Button();
			this.panelSlider = new System.Windows.Forms.Panel();
			this.buttonClose = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.panelSlider.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(271, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Using effect:";
			// 
			// comboBoxEffect
			// 
			this.comboBoxEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxEffect.FormattingEnabled = true;
			this.comboBoxEffect.Location = new System.Drawing.Point(344, 12);
			this.comboBoxEffect.Name = "comboBoxEffect";
			this.comboBoxEffect.Size = new System.Drawing.Size(137, 21);
			this.comboBoxEffect.TabIndex = 3;
			this.comboBoxEffect.TabStop = false;
			this.comboBoxEffect.SelectedIndexChanged += new System.EventHandler(this.comboBoxEffect_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(155, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Select channels/groups to test:";
			// 
			// checkedListBox
			// 
			this.checkedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkedListBox.CheckOnClick = true;
			this.checkedListBox.FormattingEnabled = true;
			this.checkedListBox.Location = new System.Drawing.Point(15, 33);
			this.checkedListBox.Name = "checkedListBox";
			this.checkedListBox.Size = new System.Drawing.Size(218, 229);
			this.checkedListBox.TabIndex = 1;
			this.checkedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
			// 
			// trackBar
			// 
			this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.trackBar.Enabled = false;
			this.trackBar.LargeChange = 1;
			this.trackBar.Location = new System.Drawing.Point(19, 3);
			this.trackBar.Name = "trackBar";
			this.trackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar.Size = new System.Drawing.Size(45, 201);
			this.trackBar.TabIndex = 0;
			this.trackBar.TabStop = false;
			this.trackBar.TickFrequency = 10;
			this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// buttonSetEffectParameters
			// 
			this.buttonSetEffectParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSetEffectParameters.Location = new System.Drawing.Point(487, 12);
			this.buttonSetEffectParameters.Name = "buttonSetEffectParameters";
			this.buttonSetEffectParameters.Size = new System.Drawing.Size(50, 21);
			this.buttonSetEffectParameters.TabIndex = 4;
			this.buttonSetEffectParameters.Text = "Setup";
			this.buttonSetEffectParameters.UseVisualStyleBackColor = true;
			this.buttonSetEffectParameters.Click += new System.EventHandler(this.buttonSetEffectParameters_Click);
			// 
			// panelSlider
			// 
			this.panelSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelSlider.BackColor = System.Drawing.Color.DarkGray;
			this.panelSlider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelSlider.Controls.Add(this.trackBar);
			this.panelSlider.Location = new System.Drawing.Point(370, 53);
			this.panelSlider.Name = "panelSlider";
			this.panelSlider.Size = new System.Drawing.Size(84, 209);
			this.panelSlider.TabIndex = 5;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(462, 277);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 6;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			// 
			// TestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(549, 312);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.panelSlider);
			this.Controls.Add(this.buttonSetEffectParameters);
			this.Controls.Add(this.checkedListBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxEffect);
			this.Controls.Add(this.label1);
			this.Name = "TestForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Channel/Group Test";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestForm_FormClosing);
			this.Load += new System.EventHandler(this.TestForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.panelSlider.ResumeLayout(false);
			this.panelSlider.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxEffect;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckedListBox checkedListBox;
		private System.Windows.Forms.TrackBar trackBar;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.Button buttonSetEffectParameters;
		private System.Windows.Forms.Panel panelSlider;
		private System.Windows.Forms.Button buttonClose;
	}
}